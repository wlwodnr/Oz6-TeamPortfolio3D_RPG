public static class PlayerLevelProgression
{
    public static float GetRequiredTotalExperienceForNextLevel(int currentLevel)
    {
        if (currentLevel <= 1)
        {
            return 50f;
        }
        if (currentLevel == 2)
        {
            return 120f;
        }
        if (currentLevel == 3)
        {
            return 220f;
        }
        if (currentLevel == 4)
        {
            return 370f;
        }
        if (currentLevel == 5)
        {
            return 570f;
        }

        int additionalLevelCount = currentLevel - 5;
        float firstAdditionalRequirement = 250f;
        float lastAdditionalRequirement = 200f + (additionalLevelCount * 50f);
        float additionalTotalExperience = additionalLevelCount * (firstAdditionalRequirement + lastAdditionalRequirement) * 0.5f;
        return 570f + additionalTotalExperience;
    }

    public static int CalculateLevel(float totalExperience)
    {
        if (totalExperience <= 0f || float.IsNaN(totalExperience) || float.IsInfinity(totalExperience))
        {
            return 1;
        }

        int currentLevel = 1;

        while (totalExperience >= GetRequiredTotalExperienceForNextLevel(currentLevel))
        {
            currentLevel++;
        }

        return currentLevel;
    }
}
