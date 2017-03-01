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
        private Dictionary<string, string> stickersD = new Dictionary<string, string>();

        public Task ParceCsv(string nameCsv, Table table)
        {
            return Task.Factory.StartNew(() =>
            {
                MyDictionary();
                table.CreateTable();

                string[] values = File.ReadAllLines($"c:/Users/UBI-Note/Downloads/{nameCsv}");

                var query = from line in values
                            let data = line.Split(';')
                            where isSticker(data[9]) != null
                            orderby Double.Parse(data[2]) / 100
                            select new
                            {
                                name = data[10],
                                sticker = isSticker(data[9]),
                                price = Double.Parse(data[2]) / 100,
                                minPrice = CheckMinPrice(data[10], values)
                            };

                foreach (var item in query)
                {
                    if (item.sticker != null)
                    {
                        table.AddToTable(item.price, item.minPrice, item.name, item.sticker);
                    }
                }

                string isSticker(string st)
                {
                    string strNameStick = null;

                    if (st.Equals("0") || st.Equals("c_stickers"))
                    {
                        return null;
                    }

                    string[] stSplit = st.Split('|');

                    foreach (var stSpl in stSplit)
                    {
                        foreach (var itemPair in stickersD)
                        {
                            if (stSpl.Equals(itemPair.Key))
                            {
                                strNameStick += itemPair.Value + " ";
                            }
                        }
                    }

                    if (strNameStick != null)
                    {
                        return strNameStick;
                    }
                    else return null;
                }
            });
        }

        private void MyDictionary()     //заполняет словарь значениями key = id стикера, value - имя 
        {
            string str = Resource1.Stikers;
            string[] stickers = str.Split('\n');
            for (int i = 0; i < stickers.Length; i++)
            {
                string[] stsplit = stickers[i].Split(';');
                stickersD.Add(stsplit[0], stsplit[1]);
            }
        }

        private double CheckMinPrice(string nameSticker, string[] values)
        {
            var query = from line in values
                        let data = line.Split(';')
                        where data[10].Equals(nameSticker)
                        orderby Double.Parse(data[2]) / 100
                        select Double.Parse(data[2]) / 100;

            return query.First();
        }
    }
}
