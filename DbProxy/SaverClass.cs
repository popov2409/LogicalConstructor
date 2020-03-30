using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DbProxy
{
    public class SaverClass
    {
        public static List<Element> Elements;

        public static void Initialize()
        {
            Elements=new List<Element>();
        }

        public static async Task SaveData()
        {
            using (FileStream fs=new FileStream("Data.json", FileMode.OpenOrCreate))
            {
                await JsonSerializer.SerializeAsync<List<Element>>(fs, Elements);
                Console.WriteLine("Save data");
            }
        }

        public static async Task LoadData()
        {
            using (FileStream fs = new FileStream("Data.json", FileMode.OpenOrCreate))
            {
                Elements= await JsonSerializer.DeserializeAsync<List<Element>>(fs);
                Console.WriteLine("Load data");
            }
        }
    }
}
