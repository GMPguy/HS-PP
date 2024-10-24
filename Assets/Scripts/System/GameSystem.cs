using static Enums;

public static class GameSystem {

    /// <summary>
    /// This function checks currently selected language, and returns either of the strings
    /// </summary>
    public static string GetString (string eng, string pl) => 
        SettingsSystem.MainLanguage switch {
            Language.Polski => pl,
            _ => eng
        };
}
