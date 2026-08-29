#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class BootSceneBuilder
{
    public const string BootScenePath = "Assets/Scenes/BootScene.unity";

    [MenuItem("Wizard Reborn/Regenerate Boot Scene")]
    public static void GenerateBootScene()
    {
        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        GameObject canvasObject = new GameObject("Boot UI", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        Canvas canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;
        CanvasScaler scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = .5f;

        Image background = CreateImage("Background Placeholder", canvasObject.transform, new Color(.045f, .035f, .09f, 1f), Vector2.zero, Vector2.one);
        background.preserveAspect = true;
        background.gameObject.AddComponent<AspectRatioFitter>().aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;
        background.gameObject.AddComponent<BootBackgroundFitter>();

        Image overlay = CreateImage("Background Shade", canvasObject.transform, new Color(.015f, .012f, .04f, .58f), Vector2.zero, Vector2.one);
        overlay.raycastTarget = false;

        Text brand = CreateText("Game Title", canvasObject.transform, "WIZARD REBORN", 64, FontStyle.Bold, TextAnchor.MiddleCenter);
        brand.color = new Color(.92f, .86f, 1f, 1f);
        SetRect(brand.rectTransform, new Vector2(.18f, .61f), new Vector2(.82f, .82f));
        Outline brandOutline = brand.gameObject.AddComponent<Outline>();
        brandOutline.effectColor = new Color(.18f, .08f, .32f, .9f);
        brandOutline.effectDistance = new Vector2(3f, -3f);

        Image panel = CreateImage("Loading Panel", canvasObject.transform, new Color(.04f, .035f, .09f, .92f), new Vector2(.24f, .13f), new Vector2(.76f, .38f));
        Outline panelOutline = panel.gameObject.AddComponent<Outline>();
        panelOutline.effectColor = new Color(.42f, .26f, .72f, .78f);
        panelOutline.effectDistance = new Vector2(2f, -2f);
        Shadow panelShadow = panel.gameObject.AddComponent<Shadow>();
        panelShadow.effectColor = new Color(0f, 0f, 0f, .65f);
        panelShadow.effectDistance = new Vector2(0f, -6f);

        Text spinner = CreateText("Spinner", panel.transform, "✦", 38, FontStyle.Bold, TextAnchor.MiddleCenter);
        spinner.color = new Color(.67f, .48f, 1f, 1f);
        SetRect(spinner.rectTransform, new Vector2(.075f, .54f), new Vector2(.17f, .86f));

        Text status = CreateText("Status", panel.transform, "Preparing the game...", 25, FontStyle.Normal, TextAnchor.MiddleLeft);
        status.color = new Color(.86f, .86f, .94f, 1f);
        SetRect(status.rectTransform, new Vector2(.18f, .54f), new Vector2(.92f, .86f));

        Image progressTrack = CreateImage("Progress Track", panel.transform, new Color(.12f, .1f, .2f, 1f), new Vector2(.08f, .31f), new Vector2(.92f, .42f));
        Image progressFill = CreateImage("Progress Fill", progressTrack.transform, new Color(.58f, .32f, .95f, 1f), Vector2.zero, new Vector2(.04f, 1f));
        progressFill.raycastTarget = false;

        Text offlineHint = CreateText("Offline Hint", panel.transform, "Connection issues will never block game access", 17, FontStyle.Normal, TextAnchor.MiddleCenter);
        offlineHint.color = new Color(.58f, .58f, .72f, 1f);
        SetRect(offlineHint.rectTransform, new Vector2(.08f, .07f), new Vector2(.92f, .25f));

        GameObject controllerObject = new GameObject("Boot Controller");
        BootLoader loader = controllerObject.AddComponent<BootLoader>();
        loader.Configure(background, status, progressFill, spinner.rectTransform);

        EditorSceneManager.SaveScene(scene, BootScenePath);
        PutBootSceneFirstInBuildSettings();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"Boot scene generated at {BootScenePath} and placed at build index 0.");
    }

    private static void PutBootSceneFirstInBuildSettings()
    {
        List<EditorBuildSettingsScene> scenes = new List<EditorBuildSettingsScene>
        {
            new EditorBuildSettingsScene(BootScenePath, true)
        };

        foreach (EditorBuildSettingsScene existing in EditorBuildSettings.scenes)
        {
            if (string.IsNullOrWhiteSpace(existing.path) || existing.path == BootScenePath)
            {
                continue;
            }
            scenes.Add(existing);
        }
        EditorBuildSettings.scenes = scenes.ToArray();
    }

    private static Image CreateImage(string name, Transform parent, Color color, Vector2 anchorMin, Vector2 anchorMax)
    {
        GameObject gameObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
        gameObject.transform.SetParent(parent, false);
        Image image = gameObject.GetComponent<Image>();
        image.color = color;
        SetRect(image.rectTransform, anchorMin, anchorMax);
        return image;
    }

    private static Text CreateText(string name, Transform parent, string value, int fontSize, FontStyle style, TextAnchor alignment)
    {
        GameObject gameObject = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        gameObject.transform.SetParent(parent, false);
        Text text = gameObject.GetComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.text = value;
        text.fontSize = fontSize;
        text.fontStyle = style;
        text.alignment = alignment;
        text.color = Color.white;
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Truncate;
        return text;
    }

    private static void SetRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }
}
#endif
