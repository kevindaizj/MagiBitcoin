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
        private static Lazy<BTCOperator> _instance = new Lazy<BTCOperator>(() => new BTCOperator());
        public static BTCOperator Instance
        {
            get { return _instance.Value; }
        }
        
        private RPCClient Client { get; set; }
        
        private BTCOperator()
        {
            this.ChangeNetwork(NetworkOperator.Instance.Credential, 
                               NetworkOperator.Instance.RpcUri,
                               NetworkOperator.Instance.Network);
        }

        public void ChangeNetwork(NetworkCredential credential, Uri rpcUri, Network network)
        {
            this.Client = new RPCClient(credential, rpcUri, network);
        }


        public List<AddressGrouping> ListAddressGroupings()
        {
            return Client.ListAddressGroupings().ToList();
        }

        public async Task<Money> GetBalance()
        {
            var balance = await Client.GetBalanceAsync();
            return balance;
        }


    }
}
