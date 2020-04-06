using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LogicalConstructor.DbProxy
{
    public static class SaverClass
    {
        public static List<ElementClass> Elements = new List<ElementClass>();

        public static async Task SaveData(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                await JsonSerializer.SerializeAsync(fs, Elements);
            }
        }

        public static void LoadData(string path)
        {
            string jsonString = File.ReadAllText(path);
            Elements = JsonSerializer.Deserialize<List<ElementClass>>(jsonString);
        }
    }
}
