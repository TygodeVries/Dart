namespace Runtime.Graphics.Shaders
{
    public class UniformFinder
    {
        public static List<Uniform> FindUniformsInSource(string source)
        {
            List<Uniform> uniforms = new();
            // Get individual lines of code
            string[] lines = source.Split(new char[] { ';', '}' });
            // Possible bug: this ignores the '#version 330 core' header of the files, so any files with uniforms as the first code line will not be parsed correctly.
            // For the sake of itteration speed, I am going to ignore this for now. #TODO

            for (int i = 0; i < lines.Length; i++)
            {
                string line = lines[i].Trim();
                bool isHiddenUniform = line.StartsWith("uniform");
                bool isShownUniform = line.StartsWith("%show");
                if (!isHiddenUniform && !isShownUniform)
                {
                    continue;
                }

                Uniform uniform;
                string[] args = line.Split(' ');
                if (isHiddenUniform)
                {
                    // 0 is for the word 'uniform'
                    string type = args[1];
                    string name = args[2];
                    uniform = new Uniform(type, name, false);
                }
                else
                {
                    // 0 is for the show tag
                    // 1 is for the uniform
                    string type = args[2];
                    string name = args[3];
                    uniform = new Uniform(type, name, true);
                }
                uniforms.Add(uniform);
            }

            return uniforms;
        }
    }

    public class Uniform
    {
        public string type { get; private set; }
        public string name { get; private set; }
        public bool showInInspector { get; private set; }
        public Uniform(string type, string name, bool showInInspector)
        {
            this.type = type;
            this.name = name;
            this.showInInspector = showInInspector;
        }
    }
}
