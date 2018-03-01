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
    /// Interaction logic for ShopTab.xaml
    /// </summary>
    public partial class ShopTab : UserControl
    {
        public ShopTab()
        {
            InitializeComponent();
        }

        public void Sell(Window parent, FireBase fbClient)
        {
            if (lbItems.SelectedItem != null)
            {
                ItemPropEntry entry = (ItemPropEntry)lbItems.SelectedItem;
                QuantityWindow qtyW = new QuantityWindow(parent, entry.Qty.Value);
                qtyW.ShowDialog();
                if (qtyW.IsApproved && qtyW.Qty > 0 && qtyW.Qty <= entry.Qty.Value)
                {
                    ItemPropEntryFb x = entry.GetPropEntryFb();
                    fbClient.InsertInFb("transactions", new TransactionPropEntryFb(entry.Id.Value, x.Name, x.Price, qtyW.Qty));
                    fbClient.ModifyInFb("items", entry.Id.Value, new ItemPropEntryFb(x.Name, x.CategoryId, x.Description, x.Price, x.Qty - qtyW.Qty, x.Units, x.SupplierId, x.Image));
                }
            }
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
    }
}
