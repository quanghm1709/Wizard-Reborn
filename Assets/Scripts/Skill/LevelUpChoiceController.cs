using System.Collections.Generic;
using UnityEngine;

public sealed class LevelUpChoiceController : MonoBehaviour
{
    private readonly List<SkillTree> skillTrees = new List<SkillTree>();
    private readonly Queue<int> pendingLevels = new Queue<int>();
    private SkillHolder skillHolder;
    private PlayerController player;
    private bool choiceActive;

    public static LevelUpChoiceController Create(List<SkillTree> trees, SkillHolder holder, PlayerController playerController)
    {
        GameObject gameObject = new GameObject("Level Up Choice Controller");
        LevelUpChoiceController controller = gameObject.AddComponent<LevelUpChoiceController>();
        controller.skillTrees.AddRange(trees);
        controller.skillHolder = holder;
        controller.player = playerController;
        GameplayChoiceUI.EnsureExists();
        return controller;
    }

    private void OnEnable()
    {
        this.RegisterListener(EventID.OnPlayerLevelUp, HandlePlayerLevelUp);
    }

    private void OnDisable()
    {
        this.RemoveListener(EventID.OnPlayerLevelUp, HandlePlayerLevelUp);
    }

    private void HandlePlayerLevelUp(object param)
    {
        int level = param is int value ? value : 1;
        pendingLevels.Enqueue(level);
        if (!choiceActive)
        {
            ShowNextLevelChoice();
        }
    }

    private void ShowNextLevelChoice()
    {
        if (pendingLevels.Count == 0)
        {
            choiceActive = false;
            return;
        }

        choiceActive = true;
        int level = pendingLevels.Dequeue();
        List<SkillLevelUpCandidate> candidates = CollectCandidates();
        Shuffle(candidates);

        List<GameplayChoice> choices = new List<GameplayChoice>();
        int skillChoiceCount = Mathf.Min(3, candidates.Count);
        for (int i = 0; i < skillChoiceCount; i++)
        {
            SkillLevelUpCandidate candidate = candidates[i];
            choices.Add(new GameplayChoice
            {
                title = candidate.skill.skillCore.skillName,
                description = candidate.skill.skillLevel == 0
                    ? $"Mở kỹ năng\n{candidate.skill.skillCore.skillDescription}"
                    : $"Cấp {candidate.skill.skillLevel} → {candidate.skill.skillLevel + 1}\n{candidate.skill.skillCore.skillDescription}",
                icon = candidate.skillUI != null ? candidate.skillUI.skillIcon : null,
                accent = candidate.tree.TreePosition == 0
                    ? new Color(.2f, .38f, .72f, 1f)
                    : new Color(.72f, .25f, .15f, 1f),
                onSelected = () =>
                {
                    if (candidate.tree.ApplyLevelUpChoice(candidate.index))
                    {
                        skillHolder.AutoEquip(candidate.skill, candidate.skillUI);
                    }
                    FinishLevelChoice();
                }
            });
        }

        AddFallbackChoices(choices);
        GameplayChoiceUI.Instance.RequestChoices(
            $"Lên cấp {level}",
            "Chọn một nâng cấp cho lần thám hiểm này",
            choices);
    }

    private List<SkillLevelUpCandidate> CollectCandidates()
    {
        List<SkillLevelUpCandidate> result = new List<SkillLevelUpCandidate>();
        foreach (SkillTree tree in skillTrees)
        {
            tree.GetLevelUpCandidates(result);
        }
        return result;
    }

    private void AddFallbackChoices(List<GameplayChoice> choices)
    {
        if (choices.Count < 3)
        {
            choices.Add(new GameplayChoice
            {
                title = "Sinh lực",
                description = "+8% HP tối đa và hồi lượng HP tương ứng",
                accent = new Color(.55f, .16f, .22f, 1f),
                onSelected = () =>
                {
                    player.ApplyPermanentBonus(.08f, 0f, 0f, 0f);
                    FinishLevelChoice();
                }
            });
        }
        if (choices.Count < 3)
        {
            choices.Add(new GameplayChoice
            {
                title = "Ma lực",
                description = "+10% MP tối đa và hồi lượng MP tương ứng",
                accent = new Color(.18f, .28f, .68f, 1f),
                onSelected = () =>
                {
                    player.ApplyPermanentBonus(0f, .1f, 0f, 0f);
                    FinishLevelChoice();
                }
            });
        }
        if (choices.Count < 3)
        {
            choices.Add(new GameplayChoice
            {
                title = "Sức mạnh phép",
                description = "+6% sức tấn công",
                accent = new Color(.62f, .32f, .12f, 1f),
                onSelected = () =>
                {
                    player.ApplyPermanentBonus(0f, 0f, .06f, 0f);
                    FinishLevelChoice();
                }
            });
        }
    }

    private void FinishLevelChoice()
    {
        choiceActive = false;
        ShowNextLevelChoice();
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
