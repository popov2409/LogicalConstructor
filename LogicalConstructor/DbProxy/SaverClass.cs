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
        public List<ElementClass> Elements { get; set; }
        public SaverClass()
        {
            Elements = new List<ElementClass>();
        }


        public async Task SaveData(string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Create))
            {
                await JsonSerializer.SerializeAsync<List<ElementClass> >(fs, Elements);
            }
        }

        public void LoadData(string path)
        {
            string jsonString = File.ReadAllText(path);
            Elements = JsonSerializer.Deserialize<List<ElementClass>>(jsonString);
        }
    }
}
