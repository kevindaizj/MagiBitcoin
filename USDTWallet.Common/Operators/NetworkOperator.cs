using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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

        private NetworkOperator(NetworkType networkType = NetworkType.Regtest, 
                                string rpcUrl = "http://localhost:8339",
                                string rpcUserName = "kevin", 
                                string rpcPassword = "123456")
        {
            this.Init(networkType, rpcUrl, rpcUserName, rpcPassword);
        }

        private void Init(NetworkType networkType, string rpcUrl, string rpcUserName, string rpcPassword)
        {
            if (networkType == NetworkType.Mainnet)
                this.Network = Network.Main;
            if (networkType == NetworkType.Testnet)
                this.Network = Network.TestNet;
            if (networkType == NetworkType.Regtest)
                this.Network = Network.RegTest;

            this.Credential = new NetworkCredential(rpcUserName, rpcPassword);
            this.RpcUri = new Uri(rpcUrl);
        }

        public void ChangeNetwork(NetworkType networkType, string rpcUrl, string rpcUserName, string rpcPassword)
        {
            this.Init(networkType, rpcUrl, rpcUserName, rpcPassword);
            BTCOperator.Instance.ChangeNetwork(Credential, RpcUri, Network);
        }
    }
}
