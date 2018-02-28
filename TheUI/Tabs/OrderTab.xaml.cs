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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TheUI
{
    /// <summary>
    /// Interaction logic for OrderTab.xaml
    /// </summary>
    public partial class OrderTab : UserControl
    {
        public OrderTab()
        {
            InitializeComponent();
        }

        public void NewItem(MainWindow mw, CategoryDatabase cDb, SupplierDatabase sDb, ItemDatabase iDb)
        {
            CreateItemPopupWindow(new ItemPropEntry(), mw, cDb, sDb, iDb);
        }

        public void EditItem(MainWindow mw, CategoryDatabase cDb, SupplierDatabase sDb, ItemDatabase iDb)
        {
            if (lbItems.SelectedItem != null)
                CreateItemPopupWindow((ItemPropEntry)lbItems.SelectedItem, mw, cDb, sDb, iDb);
        }

        public void DeleteItem(Window parent, FireBase fb)
        {
            if (lbItems.SelectedItem != null)
            {
                ItemPropEntry item = (ItemPropEntry)lbItems.SelectedItem;
                YesNoWindow yesNo = new YesNoWindow(parent, "Delete?", "");
                yesNo.ShowDialog();
                if (yesNo.IsApproved)
                    fb.DeleteFromFb("items", item.Id.Value);
            }
        }

        private void CreateItemPopupWindow(ItemPropEntry item, MainWindow mw, CategoryDatabase cDb, SupplierDatabase sDb, ItemDatabase iDb)
        {
            new ItemWindow(item, mw, cDb, sDb, iDb).ShowDialog();
        }
    }
}
