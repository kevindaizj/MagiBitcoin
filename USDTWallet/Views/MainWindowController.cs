using Prism.Interactivity.InteractionRequest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Common;

namespace USDTWallet.Views
{
    public class MainWindowController : ControllerBase
    {
        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }

        private string network;
        public string Network
        {
            get { return network; }
            set { SetProperty(ref network, value); }
        }

        public MainWindowController()
        {
            this.Name = "Kevin";
            this.Network = "Localhost:8889";
        }
        

        
    }
}
