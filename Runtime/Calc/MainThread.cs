namespace Runtime.Calc
{
    public static class MainThread
    {
        static readonly Queue<Action> queue = new Queue<Action>();

        public static void Run(Action action)
        {
            lock (queue)
                queue.Enqueue(action);
        }

        public static void Update()
        {
            lock (queue)
            {
                while (queue.Count > 0)
                    queue.Dequeue().Invoke();
            }
        }
    }

}
