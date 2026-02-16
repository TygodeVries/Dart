namespace Project.Editor.UI
{
    public class EventClick
    {
        private static List<PrioritizedEventClick> events = new List<PrioritizedEventClick>();

        public static void AddEvent(int priority, Func<bool> action)
        {
            events.Add(new PrioritizedEventClick(priority, action));
            events.Sort((a, b) => { return a.Priority - b.Priority; });
        }

        public static void Click(ClickData clickData)
        {
            foreach (PrioritizedEventClick e in events)
            {
                if (e.Run())
                {
                    return;
                }
            }
        }

        private class PrioritizedEventClick
        {
            public PrioritizedEventClick(int priority, Func<bool> action)
            {
                this.action = action;
                this.Priority = priority;
            }

            public int Priority { get; private set; }
            private Func<bool> action;

            public bool Run()
            {
                bool? result = action?.Invoke();
                return result == null || result.Value;
            }
        }
    }

    public class ClickData
    {
        public int button;

        public ClickData(int button)
        {
            this.button = button;
        }
    }
}
