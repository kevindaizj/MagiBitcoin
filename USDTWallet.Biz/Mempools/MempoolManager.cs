using NBitcoin;
using NBitcoin.JsonConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Biz.Common;
using USDTWallet.Common.Operators;
using USDTWallet.Models.Models.Transactions;

namespace USDTWallet.Biz.Mempools
{
    public class MempoolManager : BizBase
    {
        public async Task<List<MempoolTxItem>> GetTxsJsonFromMempool()
        {
            var txs = await BTCOperator.Instance.GetTxIdsFromMempool();

            var items = new List<MempoolTxItem>();

            foreach(var tx in txs)
            {
                var senders = GetSendersAddressByInputs(tx.Transaction.Inputs);
                var receiver = GetReceiverAddressByOutputs(tx.Transaction.Outputs);

                var json = Serializer.ToString(tx);
                var txJson = tx.Transaction.ToString();

                var result = "Senders: " + string.Join(",  ", senders) + Environment.NewLine + Environment.NewLine +
                             "Receivers: " + string.Join(",  ", receiver) + Environment.NewLine + Environment.NewLine +
                             json + Environment.NewLine + Environment.NewLine +
                             txJson;
                
                items.Add(new MempoolTxItem { Json = result });
            }

            return items;
        }

       
        private List<string> GetSendersAddressByInputs(TxInList inputs)
        {
            var senders = new List<string>();

            foreach (var input in inputs)
            {
                var signer = input.GetSigner();
                var addr = signer.ScriptPubKey.GetDestinationAddress(NetworkOperator.Instance.Network);
                senders.Add(addr.ToString());
            }

            return senders.Distinct().ToList();
        }

        private List<string> GetReceiverAddressByOutputs(TxOutList outputs)
        {
            var receivers = new List<string>();

            foreach (var output in outputs)
            {
                if(output.ScriptPubKey != null)
                {
                    var addr = output.ScriptPubKey.GetDestinationAddress(NetworkOperator.Instance.Network);
                    if (addr != null)
                        receivers.Add(addr.ToString());
                }
            }

            return receivers.Distinct().ToList();
        }

    }
}
