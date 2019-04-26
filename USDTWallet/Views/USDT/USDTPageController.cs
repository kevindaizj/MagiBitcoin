using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Common;

namespace USDTWallet.Views.USDT
{
    public class USDTPageController : ControllerBase
    {
        public string Title { get { return "USDT"; } }
        public string SelectedIconScr { get { return "/Images/token_blue.png"; } }
        public string IconScr { get { return "/Images/token.png"; } }
    }
}
