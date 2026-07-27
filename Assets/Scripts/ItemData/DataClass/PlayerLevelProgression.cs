public static class PlayerLevelProgression
{
    public static float GetRequiredExperienceForLevel(int currentLevel)
    {
        if (currentLevel <= 1)
        {
            return 50f;
        }
        if (currentLevel == 2)
        {
            return 70f;
        }
        if (currentLevel == 3)
        {
            return 100f;
        }
        if (currentLevel == 4)
        {
            return 150f;
        }
        if (currentLevel == 5)
        {
            return 200f;
        }

        return 200f + ((currentLevel - 5) * 50f);
    }

    public static int CalculateLevel(float totalExperience)
    {
        if (totalExperience <= 0f || float.IsNaN(totalExperience) || float.IsInfinity(totalExperience))
        {
            return 1;
        }

        float remainingExperience = totalExperience;
        int currentLevel = 1;

        while (remainingExperience >= GetRequiredExperienceForLevel(currentLevel))
        {
            remainingExperience -= GetRequiredExperienceForLevel(currentLevel);
            currentLevel++;
        }

        return currentLevel;
    }

    public static float GetCurrentLevelExperience(float totalExperience)
    {
        if (totalExperience <= 0f || float.IsNaN(totalExperience) || float.IsInfinity(totalExperience))
        {
            return 0f;
        }

        float remainingExperience = totalExperience;
        int currentLevel = 1;

        while (remainingExperience >= GetRequiredExperienceForLevel(currentLevel))
        {
            remainingExperience -= GetRequiredExperienceForLevel(currentLevel);
            currentLevel++;
        }

        return remainingExperience;
    }
}
