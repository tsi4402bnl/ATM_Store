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

namespace TheUI
{
    /// <summary>
    /// Interaction logic for wintouch.xaml
    /// </summary>
    public partial class wintouch : Window
    {
        public wintouch(ItemPropEntry i, Delegate saveFunction)
        {
            item = new ItemPropEntry(i.Id.Value, i.GetItemPropEntryFb());
            InitializeComponent();
            gItem.DataContext = item;
            _saveFunction = saveFunction;
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            _saveFunction.DynamicInvoke(item);
            Close();
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private ItemPropEntry item { get; set; }
        private Delegate _saveFunction;
    }
}
