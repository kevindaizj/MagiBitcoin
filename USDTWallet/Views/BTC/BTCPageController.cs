using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Common;

namespace USDTWallet.Views.BTC
{
    public class BTCPageController : ControllerBase
    {
        public string Title { get { return "比特币"; } }
        public string IconScr { get { return "/Images/transfer.png"; } }
        public string SelectedIconScr { get { return "/Images/transfer_blue.png"; } }
    }
}
