using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LogicalConstructor.DbProxy
{
    public class SaverClass
    {
        public static List<ElementClass> Elements;

        public static void Initialize()
        {
            Elements=new List<ElementClass>();
        }

        public static async Task SaveData(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                await JsonSerializer.SerializeAsync<List<ElementClass> >(fs, Elements);
            }
        }

        public static async Task LoadData(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                Elements = await JsonSerializer.DeserializeAsync<List<ElementClass>>(fs);
            }
        }
    }
}
