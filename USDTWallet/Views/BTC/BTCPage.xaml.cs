using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace USDTWallet.Views.BTC
{
    /// <summary>
    /// Interaction logic for BTCPage
    /// </summary>
    public partial class BTCPage : UserControl
    {
        private static readonly Regex regex = new Regex("[^0-9.]");

        public BTCPage()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
