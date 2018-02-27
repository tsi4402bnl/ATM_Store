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

        public void ClearFilter()
        {
            CbxSearchSupplier.SelectedIndex = 0;
            TbxSearchName.Clear();
        }

        public void SetSearchCriteria(ItemDatabase db)
        {
            string searchName = TbxSearchName.Text;
            string searchSupplierId = "";
            if (CbxSearchSupplier.SelectedIndex != -1 && CbxSearchSupplier.SelectedValue != null)
                searchSupplierId = CbxSearchSupplier.SelectedValue.ToString();
            db.SetSearchCriteria(searchName, searchSupplierId);
        }

        public void Buy(Window parent, FireBase fbClient)
        {
            if (lbItems.SelectedItem != null)
            {
                ItemPropEntry entry = (ItemPropEntry)lbItems.SelectedItem;
                QuantityWindow qtyW = new QuantityWindow(parent);
                qtyW.ShowDialog();
                if (qtyW.IsApproved && qtyW.Qty > 0)
                {
                    ItemPropEntryFb x = entry.GetPropEntryFb();
                    fbClient.ModifyInFb("items", entry.Id.Value, new ItemPropEntryFb(x.Name, x.CategoryId, x.Description, x.Price, x.Qty + qtyW.Qty, x.Units, x.SupplierId, x.Image));
                }
            }
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

        public void DeleteItem(Window parent, FireBase fb, TransactionDatabase db)
        {
            if (lbItems.SelectedItem != null)
            {
                ItemPropEntry item = (ItemPropEntry)lbItems.SelectedItem;
                if (db.Contain((ItemPropEntry)lbItems.SelectedItem))
                {
                    new MessageWindow(parent, "Item in use!").ShowDialog();
                }
                else if (item.Qty.Value <= 0)
                {
                    YesNoWindow yesNo = new YesNoWindow(parent, "Delete?", "");
                    yesNo.ShowDialog();
                    if (yesNo.IsApproved)
                        fb.DeleteFromFb("items", item.Id.Value);
                }
                else
                {
                    new MessageWindow(parent, "Sell all items before deleting!").ShowDialog();
                }
            }
        }

        private void CreateItemPopupWindow(ItemPropEntry item, MainWindow mw, CategoryDatabase cDb, SupplierDatabase sDb, ItemDatabase iDb)
        {
            new ItemWindow(item, mw, cDb, sDb, iDb).ShowDialog();
        }
    }
}
