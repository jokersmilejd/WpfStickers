using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Data;

namespace WpfStickers
{
    /// <summary>
    /// Interaction logic for Table.xaml
    /// </summary>
    public partial class Table : Window
    {
        static public DataTable dtable = new DataTable("Stickers");
        DataColumn column;
        DataRow row;

        public Table()
        {
            InitializeComponent(); 
        }

        public void CreateTable()
        {
            column = new DataColumn("Цена");
            column.DataType = Type.GetType("System.Double");
            //column.ReadOnly = true;
            dtable.Columns.Add(column);

            column = new DataColumn("Название");
            //column.ReadOnly = true;
            dtable.Columns.Add(column);

            column = new DataColumn("Стикер");
            //column.ReadOnly = true;
            dtable.Columns.Add(column);

        }

        public void AddToTable(double price, string name, string stickName)
        {
            row = dtable.NewRow();
            row[0] = price;
            row[1] = name;
            row[2] = stickName;
            dtable.Rows.Add(row);
        }
    }
}
