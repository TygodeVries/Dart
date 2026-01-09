namespace Runtime.Data
{
    /// <summary>
    /// Basic settings of our game
    /// </summary>
    public class GameSettings
    {
        public string WindowTitle { get; set; } = "Untitled Game";
        public string? CodePath { get; set; } = null;
        public string[] Plugins { get; set; } = new string[0];
        public string? StartScene { get; set; } = null;
        public static GameSettings GetGameSettings()
        {
            return instance!;
        }

        private static GameSettings? instance;
        public GameSettings()
        {
            instance = this;
        }
    }
}
