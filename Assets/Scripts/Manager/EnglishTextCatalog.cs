public static class EnglishTextCatalog
{
    public static string GetSkillDescription(SkillCore skill)
    {
        if (skill == null)
        {
            return string.Empty;
        }

        switch (skill.SkillId)
        {
            case "FireArrow":
                return "50%/70%/100% chance to fire 1/2/4 additional fire arrows at random targets when attacking.";
            case "FirePit":
                return "Create a burning area that lasts for 5 seconds.";
            case "Explosion":
                return "Create a powerful explosion centered on the target.";
            case "Ember":
                return "Blast the target with a powerful flame.";
            case "Thunder":
                return "Call down lightning on the selected target, paralyzing it for 1/1.5/2 seconds.";
            case "ThunderCall":
                return "10%/20%/40% chance to call down additional lightning when attacking, without spending mana.";
            case "ThunderArrow":
                return "50%/75%/100% chance to fire 1/2/4 additional lightning arrows at random targets when attacking.";
            case "ThunderBird":
                return "Launch a lightning bird toward the target; greater travel distance deals more damage.";
            default:
                return skill.skillDescription;
        }
    }
}
