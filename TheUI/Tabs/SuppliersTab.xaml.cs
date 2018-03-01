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
    /// Interaction logic for SuppliersTab.xaml
    /// </summary>
    public partial class SuppliersTab : UserControl
    {
        public SuppliersTab()
        {
            InitializeComponent();
        }

        public void CreateSupplier(MainWindow mainWindow)
        {
            CreateSupplierPopupWindow(mainWindow, new SupplierPropEntry());
        }

        public void EditSupplier(MainWindow mainWindow)
        {
            if (lbSuppliers.SelectedItem != null)
                CreateSupplierPopupWindow(mainWindow, (SupplierPropEntry)lbSuppliers.SelectedItem);
        }

        public void DeleteSupplier(Window parent, ItemDatabase db, FireBase fb)
        {
            if (lbSuppliers.SelectedItem != null)
            {
                if (!db.Contain((SupplierPropEntry)lbSuppliers.SelectedItem))
                {
                    YesNoWindow yesNo = new YesNoWindow(parent, "Delete?", "");
                    yesNo.ShowDialog();
                    if (yesNo.IsApproved)
                        fb.DeleteFromFb("suppliers", ((SupplierPropEntry)lbSuppliers.SelectedItem).Id.Value);
                }
                else
                {
                    new MessageWindow(parent, "Supplier in use!").ShowDialog();
                }
            }
        }

        private void CreateSupplierPopupWindow(MainWindow mainWindow, SupplierPropEntry supplier)
        {
            new SupplierWindow(supplier, mainWindow).ShowDialog();
        }
    }
}
