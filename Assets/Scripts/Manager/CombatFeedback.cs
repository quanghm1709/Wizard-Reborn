using System.Collections.Generic;
using UnityEngine;

public sealed class CombatFeedback : MonoBehaviour
{
    private const int InitialPoolSize = 12;
    private const int MaxPoolSize = 48;

    private static CombatFeedback instance;
    private readonly Queue<DamagePopupView> popupPool = new Queue<DamagePopupView>();
    private readonly Queue<HitParticleView> particlePool = new Queue<HitParticleView>();
    private int popupCount;
    private int particleCount;
    private Material particleMaterial;
    private Texture2D particleTexture;

    public static void ShowHit(Vector3 worldPosition, int damage, Color accent)
    {
        if (!Application.isPlaying || damage <= 0)
        {
            return;
        }

        EnsureExists().PlayFeedback(worldPosition, damage, accent);
    }

    private static CombatFeedback EnsureExists()
    {
        if (instance != null)
        {
            return instance;
        }

        GameObject root = new GameObject("Combat Feedback");
        instance = root.AddComponent<CombatFeedback>();
        return instance;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        CreateParticleMaterial();
        for (int i = 0; i < InitialPoolSize; i++)
        {
            popupPool.Enqueue(CreatePopup());
            particlePool.Enqueue(CreateParticle());
        }
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }

        if (particleMaterial != null)
        {
            Destroy(particleMaterial);
        }
        if (particleTexture != null)
        {
            Destroy(particleTexture);
        }
    }

    private void PlayFeedback(Vector3 position, int damage, Color accent)
    {
        DamagePopupView popup = popupPool.Count > 0 ? popupPool.Dequeue() : CreatePopup();
        popup.Play(position + new Vector3(Random.Range(-.18f, .18f), .65f, -1f), damage, accent);

        HitParticleView particle = particlePool.Count > 0 ? particlePool.Dequeue() : CreateParticle();
        particle.Play(position + new Vector3(0f, .2f, -1f), accent);
    }

    private DamagePopupView CreatePopup()
    {
        popupCount++;
        GameObject root = new GameObject($"Damage Popup {popupCount}");
        root.transform.SetParent(transform, false);
        DamagePopupView view = root.AddComponent<DamagePopupView>();
        view.Initialize(this);
        root.SetActive(false);
        return view;
    }

    private HitParticleView CreateParticle()
    {
        particleCount++;
        GameObject root = new GameObject($"Hit Particle {particleCount}");
        root.transform.SetParent(transform, false);
        HitParticleView view = root.AddComponent<HitParticleView>();
        view.Initialize(this, particleMaterial);
        root.SetActive(false);
        return view;
    }

    internal void Return(DamagePopupView popup)
    {
        if (popup == null)
        {
            return;
        }
        popup.gameObject.SetActive(false);
        if (popupPool.Count < MaxPoolSize)
        {
            popupPool.Enqueue(popup);
        }
        else
        {
            Destroy(popup.gameObject);
        }
    }

    internal void Return(HitParticleView particle)
    {
        if (particle == null)
        {
            return;
        }
        particle.gameObject.SetActive(false);
        if (particlePool.Count < MaxPoolSize)
        {
            particlePool.Enqueue(particle);
        }
        else
        {
            Destroy(particle.gameObject);
        }
    }

    private void CreateParticleMaterial()
    {
        Shader shader = Shader.Find("Sprites/Default");
        if (shader == null)
        {
            shader = Shader.Find("UI/Default");
        }

        particleTexture = new Texture2D(32, 32, TextureFormat.RGBA32, false)
        {
            name = "Runtime Soft Hit Particle",
            filterMode = FilterMode.Bilinear,
            wrapMode = TextureWrapMode.Clamp
        };

        Color[] pixels = new Color[32 * 32];
        Vector2 center = new Vector2(15.5f, 15.5f);
        for (int y = 0; y < 32; y++)
        {
            for (int x = 0; x < 32; x++)
            {
                float distance = Vector2.Distance(new Vector2(x, y), center) / 15.5f;
                float alpha = Mathf.Clamp01(1f - distance);
                alpha *= alpha;
                pixels[y * 32 + x] = new Color(1f, 1f, 1f, alpha);
            }
        }
        particleTexture.SetPixels(pixels);
        particleTexture.Apply();

        particleMaterial = new Material(shader)
        {
            name = "Runtime Hit Particle Material",
            mainTexture = particleTexture
        };
    }
}

public sealed class DamagePopupView : MonoBehaviour
{
    private CombatFeedback owner;
    private TextMesh valueText;
    private TextMesh shadowText;
    private float elapsed;
    private float duration;
    private Vector3 drift;

    public void Initialize(CombatFeedback feedback)
    {
        owner = feedback;
        shadowText = CreateText("Shadow", new Color(0f, 0f, 0f, .85f), 220);
        shadowText.transform.localPosition = new Vector3(.035f, -.035f, .02f);
        valueText = CreateText("Value", Color.white, 221);
    }

    public void Play(Vector3 position, int damage, Color accent)
    {
        gameObject.SetActive(true);
        transform.position = position;
        transform.localScale = Vector3.one * .65f;
        elapsed = 0f;
        duration = Random.Range(.72f, .9f);
        drift = new Vector3(Random.Range(-.18f, .18f), Random.Range(1.25f, 1.55f), 0f);
        string value = damage.ToString();
        valueText.text = value;
        shadowText.text = value;
        valueText.color = Brighten(accent);
        shadowText.color = new Color(0f, 0f, 0f, .85f);
    }

    private void Update()
    {
        elapsed += Time.deltaTime;
        float progress = Mathf.Clamp01(elapsed / duration);
        transform.position += drift * Time.deltaTime;
        float punch = progress < .16f ? Mathf.Lerp(.65f, 1.15f, progress / .16f) : Mathf.Lerp(1.15f, .88f, (progress - .16f) / .84f);
        transform.localScale = Vector3.one * punch;

        float alpha = 1f - Mathf.SmoothStep(0f, 1f, Mathf.InverseLerp(.48f, 1f, progress));
        Color valueColor = valueText.color;
        valueColor.a = alpha;
        valueText.color = valueColor;
        Color shadowColor = shadowText.color;
        shadowColor.a = alpha * .85f;
        shadowText.color = shadowColor;

        if (elapsed >= duration)
        {
            owner?.Return(this);
        }
    }

    private TextMesh CreateText(string objectName, Color color, int sortingOrder)
    {
        GameObject child = new GameObject(objectName);
        child.transform.SetParent(transform, false);
        TextMesh text = child.AddComponent<TextMesh>();
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = 64;
        text.characterSize = .025f;
        text.fontStyle = FontStyle.Bold;
        text.anchor = TextAnchor.MiddleCenter;
        text.alignment = TextAlignment.Center;
        text.color = color;
        MeshRenderer renderer = child.GetComponent<MeshRenderer>();
        renderer.sortingOrder = sortingOrder;
        return text;
    }

    private static Color Brighten(Color color)
    {
        return Color.Lerp(color, Color.white, .22f);
    }
}

public sealed class HitParticleView : MonoBehaviour
{
    private CombatFeedback owner;
    private ParticleSystem particleSystem;

    public void Initialize(CombatFeedback feedback, Material material)
    {
        owner = feedback;
        particleSystem = gameObject.AddComponent<ParticleSystem>();
        ParticleSystem.MainModule main = particleSystem.main;
        main.duration = .18f;
        main.loop = false;
        main.playOnAwake = false;
        main.simulationSpace = ParticleSystemSimulationSpace.World;
        main.startLifetime = new ParticleSystem.MinMaxCurve(.18f, .34f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(1.8f, 3.8f);
        main.startSize = new ParticleSystem.MinMaxCurve(.06f, .14f);
        main.gravityModifier = .35f;
        main.maxParticles = 20;
        main.stopAction = ParticleSystemStopAction.Callback;

        ParticleSystem.EmissionModule emission = particleSystem.emission;
        emission.enabled = false;
        ParticleSystem.ShapeModule shape = particleSystem.shape;
        shape.enabled = true;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = .14f;

        ParticleSystemRenderer renderer = GetComponent<ParticleSystemRenderer>();
        renderer.renderMode = ParticleSystemRenderMode.Billboard;
        renderer.sortingOrder = 210;
        renderer.sharedMaterial = material;
    }

    public void Play(Vector3 position, Color accent)
    {
        gameObject.SetActive(true);
        transform.position = position;
        ParticleSystem.MainModule main = particleSystem.main;
        main.startColor = new ParticleSystem.MinMaxGradient(Brighten(accent), accent);
        particleSystem.Clear(true);
        particleSystem.Play(true);
        particleSystem.Emit(Random.Range(9, 14));
    }

    private void OnParticleSystemStopped()
    {
        owner?.Return(this);
    }

    private static Color Brighten(Color color)
    {
        return Color.Lerp(color, Color.white, .45f);
    }
}
