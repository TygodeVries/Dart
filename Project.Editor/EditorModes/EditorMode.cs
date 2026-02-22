namespace Project.Editor.EditorModes
{
    public class EditorMode
    {
        public static void Init()
        {
            WindowSwitcher.Init();
        }

        public static void SwitchModes()
        {
            if (GetMode() == Mode.Build)
                SetMode(Mode.Edit);
            else
                SetMode(Mode.Build);
        }

        private static Mode mode;
        public static Mode GetMode()
        {
            return mode;
        }

        public static void SetMode(Mode mode)
        {
            EditorMode.mode = mode;
            OnModeSwitch?.Invoke(mode);
        }

        public static Action<Mode>? OnModeSwitch;
    }

    public enum Mode
    {
        Edit,
        Build
    }
}
