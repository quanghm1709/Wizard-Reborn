using System;
using System.Collections;
using System.Threading.Tasks;
using Firebase;
using Firebase.Analytics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum FirebaseInitializationState
{
    NotStarted,
    Initializing,
    Ready,
    Unavailable,
    TimedOut
}

public static class FirebaseRuntimeState
{
    public static FirebaseInitializationState State { get; internal set; } = FirebaseInitializationState.NotStarted;
    public static DependencyStatus DependencyStatus { get; internal set; } = DependencyStatus.UnavailableOther;
    public static string LastMessage { get; internal set; } = string.Empty;
    public static bool IsReady => State == FirebaseInitializationState.Ready;
}

[RequireComponent(typeof(Image), typeof(AspectRatioFitter))]
public sealed class BootBackgroundFitter : MonoBehaviour
{
    private Image image;
    private AspectRatioFitter fitter;
    private Sprite lastSprite;

    private void Awake()
    {
        image = GetComponent<Image>();
        fitter = GetComponent<AspectRatioFitter>();
        fitter.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
        RefreshAspectRatio();
    }

    private void Update()
    {
        if (image.sprite != lastSprite)
        {
            RefreshAspectRatio();
        }
    }

    private void RefreshAspectRatio()
    {
        lastSprite = image.sprite;
        fitter.aspectRatio = lastSprite != null && lastSprite.rect.height > 0f
            ? lastSprite.rect.width / lastSprite.rect.height
            : 16f / 9f;
    }
}

public sealed class BootLoader : MonoBehaviour
{
    [Header("Scene Flow")]
    [SerializeField] private string nextSceneName = "HomeScene";
    [SerializeField, Min(.1f)] private float minimumDisplaySeconds = 1.25f;
    [SerializeField, Min(1f)] private float firebaseTimeoutSeconds = 8f;

    [Header("Loading UI")]
    [SerializeField] private Image backgroundImage;
    [SerializeField] private Text statusText;
    [SerializeField] private Image progressFill;
    [SerializeField] private RectTransform spinner;

    private float progress;
    private bool completingFirebaseInBackground;

    public Image BackgroundImage => backgroundImage;
    public float FirebaseTimeoutSeconds => firebaseTimeoutSeconds;
    public string NextSceneName => nextSceneName;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Screen.orientation = ScreenOrientation.LandscapeLeft;
    }

    private void Update()
    {
        if (spinner != null)
        {
            spinner.Rotate(0f, 0f, -150f * Time.unscaledDeltaTime);
        }
    }

    private IEnumerator Start()
    {
        float bootStartedAt = Time.realtimeSinceStartup;
        SetStatus("Đang chuẩn bị trò chơi...");
        SetProgress(.04f, true);

        AsyncOperation homeLoad = null;
        try
        {
            homeLoad = SceneManager.LoadSceneAsync(nextSceneName);
            if (homeLoad != null)
            {
                homeLoad.allowSceneActivation = false;
            }
        }
        catch (Exception exception)
        {
            Debug.LogWarning($"Cannot preload {nextSceneName}: {exception.Message}");
        }

        yield return InitializeFirebaseWithFallback();

        while (Time.realtimeSinceStartup - bootStartedAt < minimumDisplaySeconds)
        {
            UpdateSceneLoadProgress(homeLoad);
            yield return null;
        }

        if (homeLoad != null)
        {
            while (homeLoad.progress < .9f)
            {
                UpdateSceneLoadProgress(homeLoad);
                yield return null;
            }
        }

        SetProgress(1f, true);
        SetStatus(FirebaseRuntimeState.IsReady
            ? "Hoàn tất"
            : "Tiếp tục ở chế độ ngoại tuyến");
        yield return new WaitForSecondsRealtime(.2f);

        if (homeLoad != null)
        {
            homeLoad.allowSceneActivation = true;
            yield return null;
        }
        else
        {
            try
            {
                SceneManager.LoadScene(nextSceneName);
            }
            catch (Exception exception)
            {
                Debug.LogError($"Cannot enter {nextSceneName}: {exception.Message}");
            }
        }

        if (!completingFirebaseInBackground)
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator InitializeFirebaseWithFallback()
    {
        FirebaseRuntimeState.State = FirebaseInitializationState.Initializing;
        FirebaseRuntimeState.LastMessage = "Checking Firebase dependencies";
        SetStatus("Đang khởi tạo dịch vụ...");

        Task<DependencyStatus> dependencyTask;
        try
        {
            dependencyTask = FirebaseApp.CheckAndFixDependenciesAsync();
        }
        catch (Exception exception)
        {
            MarkFirebaseUnavailable($"Firebase startup failed: {exception.Message}");
            yield break;
        }

        float startedAt = Time.realtimeSinceStartup;
        while (!dependencyTask.IsCompleted && Time.realtimeSinceStartup - startedAt < firebaseTimeoutSeconds)
        {
            float normalized = (Time.realtimeSinceStartup - startedAt) / firebaseTimeoutSeconds;
            SetProgress(Mathf.Lerp(.08f, .72f, normalized), false);
            yield return null;
        }

        if (!dependencyTask.IsCompleted)
        {
            FirebaseRuntimeState.State = FirebaseInitializationState.TimedOut;
            FirebaseRuntimeState.LastMessage = "Firebase initialization timed out; game continued offline";
            Debug.LogWarning(FirebaseRuntimeState.LastMessage);
            SetStatus("Không thể kết nối, đang vào game...");
            completingFirebaseInBackground = true;
            StartCoroutine(CompleteFirebaseInBackground(dependencyTask));
            yield break;
        }

        CompleteFirebaseInitialization(dependencyTask);
    }

    private IEnumerator CompleteFirebaseInBackground(Task<DependencyStatus> dependencyTask)
    {
        const float backgroundWaitLimit = 30f;
        float startedAt = Time.realtimeSinceStartup;
        while (!dependencyTask.IsCompleted && Time.realtimeSinceStartup - startedAt < backgroundWaitLimit)
        {
            yield return null;
        }

        if (dependencyTask.IsCompleted)
        {
            CompleteFirebaseInitialization(dependencyTask);
        }
        else
        {
            FirebaseRuntimeState.LastMessage = "Firebase remained unavailable after background retry";
        }

        completingFirebaseInBackground = false;
        Destroy(gameObject);
    }

    private static void CompleteFirebaseInitialization(Task<DependencyStatus> dependencyTask)
    {
        if (dependencyTask.IsCanceled)
        {
            MarkFirebaseUnavailable("Firebase dependency check was canceled");
            return;
        }

        if (dependencyTask.IsFaulted)
        {
            string message = dependencyTask.Exception != null
                ? dependencyTask.Exception.GetBaseException().Message
                : "Unknown Firebase task error";
            MarkFirebaseUnavailable($"Firebase dependency check failed: {message}");
            return;
        }

        DependencyStatus dependencyStatus;
        try
        {
            dependencyStatus = dependencyTask.Result;
        }
        catch (Exception exception)
        {
            MarkFirebaseUnavailable($"Firebase result could not be read: {exception.Message}");
            return;
        }

        FirebaseRuntimeState.DependencyStatus = dependencyStatus;
        if (dependencyStatus != DependencyStatus.Available)
        {
            MarkFirebaseUnavailable($"Firebase dependencies unavailable: {dependencyStatus}");
            return;
        }

        try
        {
            FirebaseApp app = FirebaseApp.DefaultInstance;
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
            FirebaseAnalytics.LogEvent("app_boot");
            FirebaseRuntimeState.State = FirebaseInitializationState.Ready;
            FirebaseRuntimeState.LastMessage = $"Firebase ready: {app.Name}";
            Debug.Log(FirebaseRuntimeState.LastMessage);
        }
        catch (Exception exception)
        {
            MarkFirebaseUnavailable($"Firebase app creation failed: {exception.Message}");
        }
    }

    private static void MarkFirebaseUnavailable(string message)
    {
        FirebaseRuntimeState.State = FirebaseInitializationState.Unavailable;
        FirebaseRuntimeState.LastMessage = message;
        Debug.LogWarning($"{message}. Continuing without analytics.");
    }

    private void UpdateSceneLoadProgress(AsyncOperation operation)
    {
        if (operation == null)
        {
            return;
        }
        float sceneProgress = Mathf.Clamp01(operation.progress / .9f);
        SetProgress(Mathf.Lerp(.72f, .96f, sceneProgress), false);
    }

    private void SetStatus(string message)
    {
        if (statusText != null)
        {
            statusText.text = message;
        }
    }

    private void SetProgress(float value, bool immediate)
    {
        progress = immediate ? Mathf.Clamp01(value) : Mathf.Max(progress, Mathf.Clamp01(value));
        if (progressFill != null)
        {
            RectTransform fillRect = progressFill.rectTransform;
            Vector2 anchorMax = fillRect.anchorMax;
            anchorMax.x = progress;
            fillRect.anchorMax = anchorMax;
        }
    }

    public void Configure(Image background, Text status, Image fill, RectTransform loadingSpinner)
    {
        backgroundImage = background;
        statusText = status;
        progressFill = fill;
        spinner = loadingSpinner;
    }
}
