using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WpfStickers
{
    class CurrentItems
    {
        public string Db { get; set; }

        public Task ParceCsv(string nameCsv)
        {

            return Task.Factory.StartNew(() => 
            {
                string[] values = File.ReadAllLines($"c:/Users/UBI-Note/Downloads/{nameCsv}");

                var query = from line in values
                            let data = line.Split(';')
                            where isSticker(data[9])
                            orderby Double.Parse(data[2]) / 100
                            select new
                            {
                                name = data[11],
                                sticker = data[9],
                                price = Double.Parse(data[2]) / 100
                            };

                using (var file = new StreamWriter(@"D:\listStickers.txt"))
                {
                    foreach (var item in query)
                    {
                        Console.WriteLine($"{item.price}\t{item.name}\t{item.sticker}");
                        file.WriteLine($"{item.price}\t{item.name}\t{item.sticker}");
                    }
                }

                bool isSticker(string st)
                {
                    string[] idStickers = File.ReadAllLines(@"D:\Stikers2.txt");

                    for (int i = 0; i < idStickers.Length; i++)
                    {
                        string[] stSplit = st.Split('|');

                        foreach (var item in stSplit)
                        {
                            if (item.Equals(idStickers[i]))
                            {
                                return true;
                            }
                        }
                    }

                    return false;
                }
            });
        }
    }
}
