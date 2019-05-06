using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Models.Enums.Network;

namespace USDTWallet.Common.Operators
{
    public class NetworkOperator
    {
        private static Lazy<NetworkOperator> _instance = new Lazy<NetworkOperator>(() => new NetworkOperator());
        public static NetworkOperator Instance
        {
            get { return _instance.Value; }
        }

        public Network Network { get; private set; }

        public Uri RpcUri { get; private set; }
        

        public NetworkCredential Credential { get; private set; }

        private NetworkOperator(CustomNetworkType networkType = CustomNetworkType.Regtest, 
                                string rpcUrl = "http://localhost:8339",
                                string rpcUserName = "kevin", 
                                string rpcPassword = "123456")
        {
            this.Init(networkType, rpcUrl, rpcUserName, rpcPassword);
        }

        private void Init(CustomNetworkType networkType, string rpcUrl, string rpcUserName, string rpcPassword)
        {
            if (networkType == CustomNetworkType.Mainnet)
                this.Network = Network.Main;
            if (networkType == CustomNetworkType.Testnet)
                this.Network = Network.TestNet;
            if (networkType == CustomNetworkType.Regtest)
                this.Network = Network.RegTest;

            this.Credential = new NetworkCredential(rpcUserName, rpcPassword);
            this.RpcUri = new Uri(rpcUrl);
        }

        public void ChangeNetwork(CustomNetworkType networkType, string rpcUrl, string rpcUserName, string rpcPassword)
        {
            this.Init(networkType, rpcUrl, rpcUserName, rpcPassword);
            BTCOperator.Instance.ChangeNetwork(Credential, RpcUri, Network);
        }

        public async Task<bool> CheckNetwork()
        {
            bool connected = false;

            try
            {
                await BTCOperator.Instance.GetBalance();
                connected = true;
            }
            catch(Exception)
            {

            }

            return connected;
        }
    }
}
