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
    /// Interaction logic for YesNoWindow.xaml
    /// </summary>
    public partial class YesNoWindow : CenterWindow
    {
        public bool IsApproved { get; set; }

        public YesNoWindow(Window parent, string message, string title = "") : base(parent)
        {
            InitializeComponent();
            TbxMsg.Text = message;
            Title = title;
        }

        private void BtnYes_Click(object sender, RoutedEventArgs e)
        {
            IsApproved = true;
            Close();
        }

        private void BtnNo_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
