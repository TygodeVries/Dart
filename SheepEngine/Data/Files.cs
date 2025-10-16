using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Runtime.Data
{
    public class Files
    {
        public static T? Load<T>(string file)
        {
            if (!File.Exists(file))
                throw new FileNotFoundException($"The file '{file}' does not exist.");

            string json = File.ReadAllText(file);
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
