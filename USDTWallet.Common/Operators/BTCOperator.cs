using NBitcoin;
using NBitcoin.RPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace USDTWallet.Common.Operators
{
    public class BTCOperator
    {
        private static Lazy<BTCOperator> _instance = new Lazy<BTCOperator>(() => new BTCOperator(Network.RegTest));
        public BTCOperator Instance
        {
            get { return _instance.Value; }
        }

        private Uri RpcUri { get; set; }

        private NetworkCredential Credential { get; set; }

        private RPCClient Client { get; set; }


        private BTCOperator(Network network, string rpcUrl = "http://localhost:8339", 
                            string rpcUserName = "kevin", string rpcPwd = "123456")
        {
            this.Credential = new NetworkCredential(rpcUserName, rpcPwd);
            this.RpcUri = new Uri(rpcUrl);
            this.Client = new RPCClient(Credential, RpcUri, network);
        }

        public void ChangeNetwork(Network network, string rpcUrl, string rpcUserName, string rpcPwd)
        {
            if (string.IsNullOrWhiteSpace(rpcUrl))
                throw new ArgumentNullException("RPC Url cannot be null");

            _instance = new Lazy<BTCOperator>(() => new BTCOperator(network, rpcUrl, rpcUserName, rpcPwd));
        }


        public List<AddressGrouping> ListAddressGroupings()
        {
            return Client.ListAddressGroupings().ToList();
        }


    }
}
