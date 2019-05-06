using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace USDTWallet.Views.USDT
{
    /// <summary>
    /// Interaction logic for USDTPage
    /// </summary>
    public partial class USDTPage : UserControl
    {
        private static readonly Regex regex = new Regex("[^0-9.]");
        public USDTPage()
        {
            InitializeComponent();
        }

        private void TextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
