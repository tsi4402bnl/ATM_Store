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
    /// Interaction logic for QuantityWindow.xaml
    /// </summary>
    public partial class QuantityWindow : CenterWindow
    {
        public int Qty
        {
            get
            {
                if ( qty < 0 ) qty = 0;
                else if (maxValue >= 0 && qty > maxValue) qty = maxValue;
                return qty;
            }
            set
            {
                if (value < 0 || (maxValue >= 0 && value > maxValue)) return;
                qty = value;
            }
        }
        public bool IsApproved { get; set; }
        private int qty;
        private readonly int maxValue;

        public QuantityWindow(Window parent, int maxValue = -1) : base(parent)
        {
            this.maxValue = maxValue;
            InitializeComponent();
            DpQty.DataContext = this;
            Qty = maxValue > 0 ? maxValue : 1;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            IsApproved = true;
            Close();
        }

        private void TbxQty_Loaded(object sender, RoutedEventArgs e)
        {
            TextBox tb = (sender as TextBox);
            if (tb != null)
            {
                tb.SelectAll();
            }
        }
    }
}
