public static class LevelStats
{
    public static int totalErrors = 0;
    public static int totalWarnings = 0;

    // Panggil fungsi ini di Start() pada setiap script Terminal
    public static void ResetStats()
    {
        totalErrors = 0;
        totalWarnings = 0;
    }
}