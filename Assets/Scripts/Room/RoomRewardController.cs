using System.Collections.Generic;
using UnityEngine;

public sealed class RoomRewardController : MonoBehaviour
{
    private PlayerController player;

    public static RoomRewardController Create(PlayerController playerController)
    {
        GameObject gameObject = new GameObject("Room Reward Controller");
        RoomRewardController controller = gameObject.AddComponent<RoomRewardController>();
        controller.player = playerController;
        GameplayChoiceUI.EnsureExists();
        return controller;
    }

    private void OnEnable()
    {
        this.RegisterListener(EventID.OnRoomReward, HandleRoomReward);
    }

    private void OnDisable()
    {
        this.RemoveListener(EventID.OnRoomReward, HandleRoomReward);
    }

    private void HandleRoomReward(object param)
    {
        if (!(param is RoomController room))
        {
            return;
        }

        RunManager.Instance?.OnRoomCleared();
        switch (room.Category)
        {
            case RoomCategory.Elite:
            case RoomCategory.Treasure:
            case RoomCategory.Boss:
                ShowRelicReward(room.Category);
                break;
            case RoomCategory.Healing:
                ShowRecoveryReward();
                break;
            case RoomCategory.Combat:
                ShowCombatReward(room.IsBonusObjectiveComplete);
                break;
        }
    }

    private void ShowCombatReward(bool bonusObjective)
    {
        int goldReward = bonusObjective ? 10 : 6;
        List<GameplayChoice> choices = new List<GameplayChoice>
        {
            new GameplayChoice
            {
                title = "Recovery",
                description = "Restore 20% max HP.",
                accent = new Color(.48f, .12f, .18f, 1f),
                onSelected = () => player.UsingItem(.2f, 0f, 0f, false)
            },
            new GameplayChoice
            {
                title = "Mana Recharge",
                description = "Restore 30% max MP.",
                accent = new Color(.14f, .25f, .62f, 1f),
                onSelected = () => player.UsingItem(0f, .3f, 0f, false)
            },
            new GameplayChoice
            {
                title = "Gold Pouch",
                description = $"Gain {goldReward} gold{(bonusObjective ? " for completing the bonus objective" : "")}.",
                accent = new Color(.62f, .46f, .1f, 1f),
                onSelected = () => GoldManager.playerGold += goldReward
            }
        };
        GameplayChoiceUI.Instance.RequestChoices("Room Reward", "Choose one advantage before moving on", choices);
    }

    private void ShowRecoveryReward()
    {
        List<GameplayChoice> choices = new List<GameplayChoice>
        {
            new GameplayChoice
            {
                title = "Full Recovery",
                description = "Restore all HP.",
                accent = new Color(.46f, .12f, .2f, 1f),
                onSelected = () => player.currentHp = player.maxHp
            },
            new GameplayChoice
            {
                title = "Full Mana",
                description = "Restore all MP.",
                accent = new Color(.12f, .24f, .62f, 1f),
                onSelected = () => player.currentMp = player.maxMp
            },
            new GameplayChoice
            {
                title = "Enduring Vitality",
                description = "+5% max HP.",
                accent = new Color(.32f, .45f, .18f, 1f),
                onSelected = () => player.ApplyPermanentBonus(.05f, 0f, 0f, 0f)
            }
        };
        GameplayChoiceUI.Instance.RequestChoices("Healing Spring", "Choose one effect", choices);
    }

    private void ShowRelicReward(RoomCategory category)
    {
        List<RelicInfo> available = RunManager.Instance.GetAvailableRelics();
        Shuffle(available);
        List<GameplayChoice> choices = new List<GameplayChoice>();

        for (int i = 0; i < Mathf.Min(3, available.Count); i++)
        {
            RelicInfo relic = available[i];
            choices.Add(new GameplayChoice
            {
                title = relic.title,
                description = relic.description,
                accent = relic.color,
                onSelected = () => RunManager.Instance.AcquireRelic(relic.type)
            });
        }

        while (choices.Count < 3)
        {
            int gold = category == RoomCategory.Boss ? 30 : 15;
            choices.Add(new GameplayChoice
            {
                title = "Gold Cache",
                description = $"Gain {gold} gold.",
                accent = new Color(.62f, .46f, .1f, 1f),
                onSelected = () => GoldManager.playerGold += gold
            });
        }

        GameplayChoiceUI.Instance.RequestChoices(
            category == RoomCategory.Boss ? "Boss Spoils" : "Relic",
            "Relics last for the entire run",
            choices);
    }

    private static void Shuffle<T>(IList<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int swapIndex = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[swapIndex];
            list[swapIndex] = temp;
        }
    }
}
