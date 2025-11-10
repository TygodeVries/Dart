using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Runtime.Data
{

    /// <summary>
    /// Simple wrapper for the text.json, just in case we need to replace this one day with something else
    /// </summary>
    public class Files
    {

        public static T? Load<T>(string file)
        {
            if (!File.Exists(file))
            {
               Logging.Debug.Error($"The file: {file} does not exist");
               return default(T);
            }

            string json = File.ReadAllText(file);
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
