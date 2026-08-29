using System;
using System.Collections.Generic;
using UnityEngine;

public enum RelicType
{
    BurningHeart,
    StaticBattery,
    GlassWand,
    BloodContract,
    EchoCrystal,
    ChainCore,
    VitalCore,
    ManaWell
}

public sealed class RelicInfo
{
    public RelicType type;
    public string title;
    public string description;
    public Color color;
}

public sealed class RunManager : MonoBehaviour
{
    public static RunManager Instance { get; private set; }

    private readonly HashSet<RelicType> relics = new HashSet<RelicType>();
    private PlayerController player;
    private bool echoAvailable;

    public float SkillDamageMultiplier => HasRelic(RelicType.GlassWand) ? 1.4f : 1f;
    public float BurnDurationMultiplier => HasRelic(RelicType.BurningHeart) ? 1.5f : 1f;
    public float OverloadDamageMultiplier => HasRelic(RelicType.ChainCore) ? 1.5f : 1f;

    public static RunManager Create(PlayerController playerController)
    {
        if (Instance != null)
        {
            return Instance;
        }

        GameObject gameObject = new GameObject("Run Manager");
        RunManager manager = gameObject.AddComponent<RunManager>();
        manager.player = playerController;
        return manager;
    }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public bool HasRelic(RelicType relic)
    {
        return relics.Contains(relic);
    }

    public bool AcquireRelic(RelicType relic)
    {
        if (!relics.Add(relic))
        {
            return false;
        }

        switch (relic)
        {
            case RelicType.GlassWand:
                player.ApplyPermanentBonus(-.2f, 0f, .25f, 0f);
                break;
            case RelicType.BloodContract:
                player.ApplyPermanentBonus(0f, 0f, .25f, 0f);
                break;
            case RelicType.VitalCore:
                player.ApplyPermanentBonus(.2f, 0f, 0f, 0f);
                break;
            case RelicType.ManaWell:
                player.ApplyPermanentBonus(0f, .2f, 0f, 0f);
                break;
        }
        return true;
    }

    public void OnRoomEntered()
    {
        echoAvailable = HasRelic(RelicType.EchoCrystal);
    }

    public void OnRoomCleared()
    {
        if (HasRelic(RelicType.BloodContract))
        {
            player.TakeDirectDamage(Mathf.Max(1, Mathf.RoundToInt(player.maxHp * .05f)), false);
        }
    }

    public bool ConsumeEcho()
    {
        if (!echoAvailable)
        {
            return false;
        }
        echoAvailable = false;
        return true;
    }

    public void RestoreManaFromShock()
    {
        if (HasRelic(RelicType.StaticBattery))
        {
            player.currentMp = Mathf.Min(player.maxMp, player.currentMp + player.maxMp * .03f);
        }
    }

    public List<string> CaptureRelics()
    {
        List<string> result = new List<string>();
        foreach (RelicType relic in relics)
        {
            result.Add(relic.ToString());
        }
        return result;
    }

    public void ApplyRelics(List<string> savedRelics)
    {
        relics.Clear();
        if (savedRelics == null)
        {
            return;
        }

        foreach (string savedRelic in savedRelics)
        {
            if (Enum.TryParse(savedRelic, out RelicType relic))
            {
                // Stats are already contained in the saved player data.
                relics.Add(relic);
            }
        }
    }

    public List<RelicInfo> GetAvailableRelics()
    {
        List<RelicInfo> result = new List<RelicInfo>();
        foreach (RelicType relic in Enum.GetValues(typeof(RelicType)))
        {
            if (!HasRelic(relic))
            {
                result.Add(GetRelicInfo(relic));
            }
        }
        return result;
    }

    public static RelicInfo GetRelicInfo(RelicType relic)
    {
        switch (relic)
        {
            case RelicType.BurningHeart:
                return Info(relic, "Trái Tim Cháy", "Burn tồn tại lâu hơn 50%.", new Color(.75f, .2f, .08f, 1f));
            case RelicType.StaticBattery:
                return Info(relic, "Pin Tĩnh Điện", "Gây Shock hồi 3% MP tối đa.", new Color(.18f, .38f, .78f, 1f));
            case RelicType.GlassWand:
                return Info(relic, "Đũa Thủy Tinh", "+40% sát thương phép, đổi lại giảm 20% HP tối đa.", new Color(.62f, .25f, .72f, 1f));
            case RelicType.BloodContract:
                return Info(relic, "Khế Ước Máu", "+25% ATK, mất 5% HP sau mỗi phòng.", new Color(.6f, .08f, .12f, 1f));
            case RelicType.EchoCrystal:
                return Info(relic, "Tinh Thể Vọng Âm", "Kỹ năng active đầu tiên mỗi phòng được thi triển hai lần.", new Color(.32f, .62f, .8f, 1f));
            case RelicType.ChainCore:
                return Info(relic, "Lõi Dẫn Truyền", "Overload gây thêm 50% sát thương.", new Color(.25f, .48f, .86f, 1f));
            case RelicType.VitalCore:
                return Info(relic, "Lõi Sinh Mệnh", "+20% HP tối đa.", new Color(.48f, .14f, .2f, 1f));
            default:
                return Info(relic, "Giếng Ma Lực", "+20% MP tối đa.", new Color(.18f, .25f, .7f, 1f));
        }
    }

    private static RelicInfo Info(RelicType type, string title, string description, Color color)
    {
        return new RelicInfo { type = type, title = title, description = description, color = color };
    }
}
