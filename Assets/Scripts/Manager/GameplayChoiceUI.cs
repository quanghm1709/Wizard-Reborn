using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class GameplayChoice
{
    public string title;
    public string description;
    public Sprite icon;
    public Color accent = new Color(.35f, .2f, .65f, 1f);
    public Action onSelected;
}

public sealed class GameplayChoiceUI : MonoBehaviour
{
    private sealed class ChoiceRequest
    {
        public string title;
        public string subtitle;
        public List<GameplayChoice> choices;
    }

    public static GameplayChoiceUI Instance { get; private set; }

    private readonly Queue<ChoiceRequest> requests = new Queue<ChoiceRequest>();
    private readonly List<Button> buttons = new List<Button>();
    private readonly List<Text> choiceTitles = new List<Text>();
    private readonly List<Text> choiceDescriptions = new List<Text>();
    private readonly List<Image> choiceIcons = new List<Image>();
    private readonly List<Text> iconPlaceholders = new List<Text>();
    private readonly List<Image> accentBars = new List<Image>();
    private readonly List<Image> iconFrames = new List<Image>();
    private readonly List<Outline> cardOutlines = new List<Outline>();
    private GameObject overlay;
    private Text titleText;
    private Text subtitleText;
    private Texture2D roundedTexture;
    private Sprite roundedSprite;
    private float previousTimeScale = 1f;
    private bool showing;
    private bool initialized;

    public static GameplayChoiceUI EnsureExists()
    {
        if (Instance != null)
        {
            return Instance;
        }

        GameObject root = new GameObject("Gameplay Choice UI");
        GameplayChoiceUI ui = root.AddComponent<GameplayChoiceUI>();
        ui.InitializeAsInstance();
        return ui;
    }

    private void Awake()
    {
        InitializeAsInstance();
    }

    private void InitializeAsInstance()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        if (initialized)
        {
            return;
        }
        initialized = true;
        BuildUI();
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
        if (roundedSprite != null)
        {
            if (Application.isPlaying) Destroy(roundedSprite);
            else DestroyImmediate(roundedSprite);
        }
        if (roundedTexture != null)
        {
            if (Application.isPlaying) Destroy(roundedTexture);
            else DestroyImmediate(roundedTexture);
        }
    }

    private void Update()
    {
        if (!showing)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) SelectButton(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2)) SelectButton(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3)) SelectButton(2);
    }

    public void RequestChoices(string title, string subtitle, List<GameplayChoice> choices)
    {
        if (choices == null || choices.Count == 0)
        {
            return;
        }

        requests.Enqueue(new ChoiceRequest
        {
            title = title,
            subtitle = subtitle,
            choices = choices
        });

        if (!showing)
        {
            ShowNextRequest();
        }
    }

    private void ShowNextRequest()
    {
        if (requests.Count == 0)
        {
            showing = false;
            overlay.SetActive(false);
            Time.timeScale = previousTimeScale;
            return;
        }

        if (!showing)
        {
            previousTimeScale = Time.timeScale;
            Time.timeScale = 0f;
        }

        showing = true;
        ChoiceRequest request = requests.Dequeue();
        overlay.SetActive(true);
        titleText.text = request.title;
        subtitleText.text = request.subtitle;

        for (int i = 0; i < buttons.Count; i++)
        {
            Button button = buttons[i];
            button.onClick.RemoveAllListeners();
            bool hasChoice = i < request.choices.Count;
            button.gameObject.SetActive(hasChoice);
            if (!hasChoice)
            {
                continue;
            }

            GameplayChoice choice = request.choices[i];
            choiceTitles[i].text = choice.title;
            choiceDescriptions[i].text = choice.description;
            accentBars[i].color = choice.accent;
            iconFrames[i].color = new Color(choice.accent.r, choice.accent.g, choice.accent.b, .24f);
            cardOutlines[i].effectColor = new Color(choice.accent.r, choice.accent.g, choice.accent.b, .7f);

            Image icon = choiceIcons[i];
            icon.sprite = choice.icon;
            icon.gameObject.SetActive(choice.icon != null);
            iconPlaceholders[i].gameObject.SetActive(choice.icon == null);
            iconPlaceholders[i].color = Color.Lerp(choice.accent, Color.white, .35f);

            button.onClick.AddListener(() => Select(choice));
        }
    }

    private void SelectButton(int index)
    {
        if (index >= 0 && index < buttons.Count && buttons[index].gameObject.activeSelf && buttons[index].interactable)
        {
            buttons[index].onClick.Invoke();
        }
    }

    private void Select(GameplayChoice choice)
    {
        foreach (Button button in buttons)
        {
            button.interactable = false;
        }

        try
        {
            choice.onSelected?.Invoke();
        }
        finally
        {
            foreach (Button button in buttons)
            {
                button.interactable = true;
            }
            ShowNextRequest();
        }
    }

    private void BuildUI()
    {
        Canvas canvas = gameObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 500;
        CanvasScaler scaler = gameObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);
        scaler.matchWidthOrHeight = .5f;
        gameObject.AddComponent<GraphicRaycaster>();
        CreateRoundedAssets();

        overlay = CreateRect("Overlay", transform, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        Image backdrop = overlay.AddComponent<Image>();
        backdrop.color = new Color(.005f, .008f, .025f, .92f);

        GameObject frame = CreateRect("Choice Panel", overlay.transform, new Vector2(.035f, .08f), new Vector2(.965f, .93f), Vector2.zero, Vector2.zero);
        Image frameImage = frame.AddComponent<Image>();
        frameImage.color = new Color(.035f, .032f, .075f, .985f);
        ApplyRounded(frameImage);
        Shadow frameShadow = frame.AddComponent<Shadow>();
        frameShadow.effectColor = new Color(0f, 0f, 0f, .72f);
        frameShadow.effectDistance = new Vector2(0f, -7f);
        Outline frameOutline = frame.AddComponent<Outline>();
        frameOutline.effectColor = new Color(.42f, .24f, .75f, .7f);
        frameOutline.effectDistance = new Vector2(3f, -3f);

        Text eyebrow = CreateText("Eyebrow", frame.transform, 17, TextAnchor.MiddleCenter);
        eyebrow.text = "CHỌN NÂNG CẤP";
        eyebrow.fontStyle = FontStyle.Bold;
        eyebrow.color = new Color(.72f, .6f, 1f, 1f);
        SetRect(eyebrow.rectTransform, new Vector2(.39f, .89f), new Vector2(.61f, .96f), Vector2.zero, Vector2.zero);

        titleText = CreateText("Title", frame.transform, 44, TextAnchor.MiddleCenter);
        titleText.fontStyle = FontStyle.Bold;
        Outline titleOutline = titleText.gameObject.AddComponent<Outline>();
        titleOutline.effectColor = new Color(0f, 0f, 0f, .8f);
        titleOutline.effectDistance = new Vector2(2f, -2f);
        SetRect(titleText.rectTransform, new Vector2(.1f, .79f), new Vector2(.9f, .9f), Vector2.zero, Vector2.zero);

        subtitleText = CreateText("Subtitle", frame.transform, 22, TextAnchor.MiddleCenter);
        subtitleText.color = new Color(.72f, .74f, .86f, 1f);
        SetRect(subtitleText.rectTransform, new Vector2(.12f, .72f), new Vector2(.88f, .8f), Vector2.zero, Vector2.zero);

        GameObject divider = CreateRect("Header Divider", frame.transform, new Vector2(.08f, .708f), new Vector2(.92f, .711f), Vector2.zero, Vector2.zero);
        divider.AddComponent<Image>().color = new Color(.42f, .28f, .68f, .7f);

        for (int i = 0; i < 3; i++)
        {
            float minX = .045f + i * .315f;
            GameObject card = CreateRect($"Choice {i + 1}", frame.transform, new Vector2(minX, .145f), new Vector2(minX + .28f, .68f), Vector2.zero, Vector2.zero);
            Image image = card.AddComponent<Image>();
            image.color = new Color(.075f, .072f, .13f, 1f);
            ApplyRounded(image);
            Shadow cardShadow = card.AddComponent<Shadow>();
            cardShadow.effectColor = new Color(0f, 0f, 0f, .55f);
            cardShadow.effectDistance = new Vector2(0f, -5f);
            Outline outline = card.AddComponent<Outline>();
            outline.effectColor = new Color(.45f, .3f, .72f, .7f);
            outline.effectDistance = new Vector2(2f, -2f);
            cardOutlines.Add(outline);
            Button button = card.AddComponent<Button>();
            ColorBlock colors = button.colors;
            colors.normalColor = Color.white;
            colors.highlightedColor = new Color(.84f, .84f, 1f, 1f);
            colors.selectedColor = new Color(.84f, .84f, 1f, 1f);
            colors.pressedColor = new Color(.65f, .62f, .85f, 1f);
            colors.disabledColor = new Color(.35f, .35f, .4f, .6f);
            colors.fadeDuration = .08f;
            button.colors = colors;
            buttons.Add(button);
            ChoiceCardHover hover = card.AddComponent<ChoiceCardHover>();
            hover.Initialize(outline);

            GameObject accentObject = CreateRect("Accent", card.transform, new Vector2(0f, .972f), Vector2.one, Vector2.zero, Vector2.zero);
            Image accent = accentObject.AddComponent<Image>();
            accentBars.Add(accent);

            GameObject numberBadge = CreateRect("Number Badge", card.transform, new Vector2(.045f, .855f), new Vector2(.13f, .95f), Vector2.zero, Vector2.zero);
            Image numberBackground = numberBadge.AddComponent<Image>();
            numberBackground.color = new Color(.14f, .13f, .22f, 1f);
            ApplyRounded(numberBackground);
            Text number = CreateText("Number", numberBadge.transform, 18, TextAnchor.MiddleCenter);
            number.text = (i + 1).ToString();
            number.fontStyle = FontStyle.Bold;
            number.color = new Color(.82f, .82f, .94f, 1f);

            GameObject iconFrameObject = CreateRect("Icon Frame", card.transform, new Vector2(.34f, .64f), new Vector2(.66f, .93f), Vector2.zero, Vector2.zero);
            Image iconFrame = iconFrameObject.AddComponent<Image>();
            ApplyRounded(iconFrame);
            iconFrames.Add(iconFrame);
            Outline iconOutline = iconFrameObject.AddComponent<Outline>();
            iconOutline.effectColor = new Color(1f, 1f, 1f, .15f);
            iconOutline.effectDistance = new Vector2(1f, -1f);

            GameObject iconObject = CreateRect("Icon", iconFrameObject.transform, new Vector2(.12f, .12f), new Vector2(.88f, .88f), Vector2.zero, Vector2.zero);
            Image icon = iconObject.AddComponent<Image>();
            icon.preserveAspect = true;
            choiceIcons.Add(icon);

            Text placeholder = CreateText("Icon Placeholder", iconFrameObject.transform, 48, TextAnchor.MiddleCenter);
            placeholder.text = "✦";
            placeholder.fontStyle = FontStyle.Bold;
            iconPlaceholders.Add(placeholder);

            Text choiceTitle = CreateText("Choice Title", card.transform, 28, TextAnchor.MiddleCenter);
            choiceTitle.fontStyle = FontStyle.Bold;
            choiceTitle.horizontalOverflow = HorizontalWrapMode.Wrap;
            choiceTitle.verticalOverflow = VerticalWrapMode.Truncate;
            SetRect(choiceTitle.rectTransform, new Vector2(.065f, .43f), new Vector2(.935f, .62f), Vector2.zero, Vector2.zero);
            choiceTitles.Add(choiceTitle);

            Text description = CreateText("Choice Description", card.transform, 20, TextAnchor.UpperCenter);
            description.horizontalOverflow = HorizontalWrapMode.Wrap;
            description.verticalOverflow = VerticalWrapMode.Truncate;
            description.color = new Color(.78f, .79f, .88f, 1f);
            description.lineSpacing = 1.08f;
            SetRect(description.rectTransform, new Vector2(.08f, .1f), new Vector2(.92f, .43f), Vector2.zero, Vector2.zero);
            choiceDescriptions.Add(description);
        }

        Text footer = CreateText("Footer", frame.transform, 17, TextAnchor.MiddleCenter);
        footer.text = "NHẤP VÀO THẺ  •  HOẶC PHÍM 1 / 2 / 3";
        footer.color = new Color(.52f, .54f, .68f, 1f);
        SetRect(footer.rectTransform, new Vector2(.25f, .035f), new Vector2(.75f, .115f), Vector2.zero, Vector2.zero);

        overlay.SetActive(false);
    }

    private static GameObject CreateRect(string name, Transform parent, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        GameObject result = new GameObject(name, typeof(RectTransform));
        result.transform.SetParent(parent, false);
        RectTransform rect = result.GetComponent<RectTransform>();
        SetRect(rect, anchorMin, anchorMax, offsetMin, offsetMax);
        return result;
    }

    private static Text CreateText(string name, Transform parent, int size, TextAnchor alignment)
    {
        GameObject result = CreateRect(name, parent, Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
        Text text = result.AddComponent<Text>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = size;
        text.alignment = alignment;
        text.color = Color.white;
        return text;
    }

    private void CreateRoundedAssets()
    {
        const int size = 64;
        const float radius = 12f;
        roundedTexture = new Texture2D(size, size, TextureFormat.RGBA32, false)
        {
            name = "Runtime Rounded UI",
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };

        Color[] pixels = new Color[size * size];
        Vector2 center = new Vector2((size - 1) * .5f, (size - 1) * .5f);
        Vector2 halfSize = new Vector2(size * .5f - radius - 1f, size * .5f - radius - 1f);
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                Vector2 point = new Vector2(Mathf.Abs(x - center.x), Mathf.Abs(y - center.y));
                Vector2 delta = new Vector2(Mathf.Max(0f, point.x - halfSize.x), Mathf.Max(0f, point.y - halfSize.y));
                float signedDistance = delta.magnitude - radius;
                float alpha = Mathf.Clamp01(.5f - signedDistance);
                pixels[y * size + x] = new Color(1f, 1f, 1f, alpha);
            }
        }
        roundedTexture.SetPixels(pixels);
        roundedTexture.Apply();
        roundedSprite = Sprite.Create(
            roundedTexture,
            new Rect(0f, 0f, size, size),
            new Vector2(.5f, .5f),
            100f,
            0,
            SpriteMeshType.FullRect,
            new Vector4(14f, 14f, 14f, 14f));
        roundedSprite.name = "Runtime Rounded UI Sprite";
    }

    private void ApplyRounded(Image image)
    {
        image.sprite = roundedSprite;
        image.type = Image.Type.Sliced;
    }

    private static void SetRect(RectTransform rect, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.offsetMin = offsetMin;
        rect.offsetMax = offsetMax;
    }
}

public sealed class ChoiceCardHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
{
    private Outline outline;
    private Color baseOutlineColor;
    private Vector3 targetScale = Vector3.one;
    private bool highlighted;

    public void Initialize(Outline cardOutline)
    {
        outline = cardOutline;
        baseOutlineColor = cardOutline.effectColor;
    }

    private void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, 14f * Time.unscaledDeltaTime);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        SetHighlighted(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetHighlighted(false);
    }

    public void OnSelect(BaseEventData eventData)
    {
        SetHighlighted(true);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        SetHighlighted(false);
    }

    private void SetHighlighted(bool highlighted)
    {
        if (this.highlighted == highlighted)
        {
            return;
        }
        this.highlighted = highlighted;
        targetScale = highlighted ? Vector3.one * 1.025f : Vector3.one;
        if (outline != null)
        {
            if (highlighted)
            {
                baseOutlineColor = outline.effectColor;
            }
            outline.effectColor = highlighted ? Color.Lerp(outline.effectColor, Color.white, .58f) : baseOutlineColor;
            outline.effectDistance = highlighted ? new Vector2(3f, -3f) : new Vector2(2f, -2f);
        }
    }
}
