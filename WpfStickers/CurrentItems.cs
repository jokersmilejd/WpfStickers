using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Threading;
using System.Data;

namespace WpfStickers
{
    class CurrentItems
    {
        public string Db { get; set; }
        public Dictionary<string, string> stickersD = new Dictionary<string, string>();
        

        public Task ParceCsv(string nameCsv, Table table)
        {
            char[] separatingChars = { '\r', '\n' };
            string strIsSticker = Resource1.Stikers2;
            string[] idStickers = strIsSticker.Split(separatingChars, StringSplitOptions.RemoveEmptyEntries);

            return Task.Factory.StartNew(() => 
            {
                MyDictionary();
                table.CreateTable();

                string[] values = File.ReadAllLines($"c:/Users/UBI-Note/Downloads/{nameCsv}");

                var query = from line in values
                            let data = line.Split(';')
                            where isSticker(data[9])
                            orderby Double.Parse(data[2]) / 100
                            select new
                            {
                                name = data[10],
                                sticker = data[9],
                                price = Double.Parse(data[2]) / 100
                            };


                //using (StreamWriter file = new StreamWriter(@"D:\listStickers.txt"))
                //{
                    foreach (var item in query)
                    {
                        foreach (var dict in stickersD)
                        {
                            if (item.sticker.Equals(dict.Key))
                            {
                                //file.WriteLine($"{item.price}\t{item.name}\t{dict.Value}");
                                table.AddToTable(item.price, item.name, dict.Value);
                            }
                        }
                    }

                //return table;
                //}

                bool isSticker(string st)
                {
                    if (st.Equals("0") || st.Equals("c_stickers"))
                    {
                        return false;
                    }

                    else
                    {
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
                }
            });
        }

        private void MyDictionary()
        {
            string str = Resource1.Stikers;
            string[] stickers = str.Split('\n');
            for (int i = 0; i < stickers.Length; i++)
            {
                string[] stSplit = stickers[i].Split(';');
                stickersD.Add(stSplit[0], stSplit[1]);
            }
        }
    }
}
