using ImGuiNET;
using Runtime.Calc;

namespace Project.Editor.UI
{
    public class Jobs
    {
        static List<Job> jobs = new List<Job>();
        public static void AddJob(Job job)
        {
            lock (jobs) jobs.Add(job);
        }

        public static void RemoveJob(Job job)
        {
            lock (jobs) jobs.Remove(job);
        }

        static float time = 0;
        public static void RenderList()
        {
            Job[] snapshot;
            lock (jobs)
            {
                snapshot = jobs.ToArray();
            }

            time += (float)Time.deltaTime;

            char[] loading = "-\\|/".ToCharArray();
            char load = loading[(int)MathF.Floor(time * 10) % loading.Length];

            if (jobs.Count > 0)
                ImGui.Text($"[{load}]: ");
            foreach (Job job in snapshot)
            {
                ImGui.Text($"{job.title}  ");
            }
        }

    }

    // The can be made on other threads
    public class Job
    {
        public string title;
        public Job(string title)
        {
            this.title = title;
            Jobs.AddJob(this);
        }

        public void Done()
        {
            Jobs.RemoveJob(this);
        }
    }
}
