namespace RandomCardsGenerators.Utils {
    internal static class LoggerUtils {
#if DEBUG
        private const bool logging = true;
#else
        private const bool logging = false;
#endif

        public static void LogInfo(string message) { if(logging) Main.ModLogger.LogInfo(message); }
        public static void LogWarn(string message) { Main.ModLogger.LogWarning(message); }
        public static void LogError(string message) { Main.ModLogger.LogError(message); }
    }
}
