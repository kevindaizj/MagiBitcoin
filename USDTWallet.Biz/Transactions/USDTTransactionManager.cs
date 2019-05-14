using NBitcoin;
using NBitcoin.RPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Biz.Common;
using USDTWallet.Common.CoinSelectors;
using USDTWallet.Common.Exceptions;
using USDTWallet.Common.Operators;
using USDTWallet.Dao.Transaction;
using USDTWallet.Models.Enums.Transaction;
using USDTWallet.Models.Models.Transactions;
using USDTWallet.Models.Models.Transfer;

namespace USDTWallet.Biz.Transactions
{
    public class USDTTransactionManager : BizBase
    {
        private TransactionDao TransactionDao { get; set; }
        public USDTTransactionManager(TransactionDao txDao)
        {
            this.TransactionDao = txDao;
        }
        
        public async Task<UnsignTransactionResult> BuildUnsignedTransaction(USDTTransferVM transferInfo)
        {
            var network = NetworkOperator.Instance.Network;
            var fromAddress = BitcoinAddress.Create(transferInfo.FromAddress, network);
            var toAddress = BitcoinAddress.Create(transferInfo.ToAddress, network);
            var feeAddress = BitcoinAddress.Create(transferInfo.FeeAddress, network);
            
            var buildInfo = await this.Build(fromAddress, toAddress, feeAddress, transferInfo.Amount, transferInfo.EstimateFeeRate);

            var txInfo = new BaseTransactionInfo
            {
                Id = Guid.NewGuid().ToString("N"),
                TransactionId = buildInfo.Transaction.GetHash().ToString(),
                TransactionType = (short)TransactionType.USDT,
                FromAddress = transferInfo.FromAddress,
                ToAddress = transferInfo.ToAddress,
                ChangeAddress = transferInfo.FeeAddress,
                FeeAddress = transferInfo.FeeAddress,
                FeeRate = transferInfo.EstimateFeeRate.SatoshiPerByte,
                EstimateSize = buildInfo.TransactionSize,
                Amount = transferInfo.Amount.ToDecimal(MoneyUnit.BTC),
                IsSigned = false,
                CreateDate = DateTime.Now
            };

            TransactionDao.Create(txInfo);


            var result = new UnsignTransactionResult
            {
                OperationId = txInfo.Id,
                Transaction = buildInfo.Transaction,
                ToSpentCoins = buildInfo.InputCoins
            };

            return result;

        }

        public async Task<OmniTransactionBuildResult> Build(BitcoinAddress fromAddress,
                                                            BitcoinAddress toAddress,
                                                            BitcoinAddress feeAddress,
                                                            Money omniAmount,
                                                            FeeRate estimateFeeRate)
        {
            if (fromAddress == toAddress)
                throw new WTException(ExceptionCode.FromAddrCouldNotBeSameAsToAddr, "发送地址与接收地址不允许相同");

            var omniOutputs = await this.GetOmniFeaturedOutputs(toAddress, omniAmount);
            var opReturnOutput = omniOutputs.OpReturnOutput;

            var dustCostBTC = omniOutputs.ReferenceOutput.Value;

            var fromAddressUnspentCoins = await BTCOperator.Instance.ListUnspentAsync(fromAddress.ToString());
            var fromCoin = fromAddressUnspentCoins.Where(o => o.Amount >= dustCostBTC)
                                                       .OrderByDescending(o => o.Amount)
                                                       .Select(o => o.AsCoin())
                                                       .FirstOrDefault();
            if (null == fromCoin)
                throw new WTException(ExceptionCode.InsufficientBTC, "发送地址没有足够的BTC：至少需要：" + dustCostBTC.ToString());

            var feeUnspentCoins = await BTCOperator.Instance.ListUnspentAsync(feeAddress.ToString());

            var feeInfo = this.CalculateFinalFee(toAddress, feeAddress, fromCoin, feeUnspentCoins, dustCostBTC, estimateFeeRate, opReturnOutput);

            var builder = NetworkOperator.Instance.Network.CreateTransactionBuilder();
            var tx = builder.AddCoins(feeInfo.InputCoins)
                            .Send(toAddress, dustCostBTC)
                            .SetChange(feeAddress)
                            .SendFees(feeInfo.Fee)
                            .SetCoinSelector(new AllCoinSelector())
                            .BuildTransaction(false);

            this.ReorganizeOutput(tx, toAddress, opReturnOutput);


            var outputAmount = tx.Outputs.Select(o => o.Value).Sum();
            var newTxDetail = tx.ToString();
            var size = builder.EstimateSize(tx);

            return new OmniTransactionBuildResult
            {
                Transaction = tx,
                TransactionSize = size,
                InputCoins = feeInfo.InputCoins.ToList()
            };
        }



        private async Task<OmniFeaturedOutputs> GetOmniFeaturedOutputs(BitcoinAddress toAddress, Money amount)
        {
            var fakedNetwork = Network.RegTest;
            var builder = fakedNetwork.CreateTransactionBuilder();

            var fakedCost = Money.Parse("100");
            var fakedAddress = BitcoinAddress.Create("n3MmAVgkrTA4ep4rVmppDL2PMH4VafNG5R", fakedNetwork);
            var txOut = new TxOut(fakedCost, fakedAddress);
            var outpoint = new OutPoint(uint256.One, 0);
            var fakedCoin = new Coin(outpoint, txOut);

            var tx = builder.AddCoins(fakedCoin)
                            .Send(toAddress, fakedCost)
                            .BuildTransaction(false);
            tx.Outputs.Clear();

            var amountPayload = await USDTOperator.Instance.CreatePayloadSimpleSend(amount);
            var opreturn = await USDTOperator.Instance.GenerateOpRetrun(tx.ToHex(), amountPayload);
            var receiveRef = await USDTOperator.Instance.GenerateReference(opreturn, toAddress.ToString());

            tx = Transaction.Parse(receiveRef, fakedNetwork);

            var result = new OmniFeaturedOutputs();
            foreach (var o in tx.Outputs)
            {
                var script = o.ScriptPubKey.ToString();
                if (script.StartsWith("OP_RETURN"))
                    result.OpReturnOutput = o;
                else
                    result.ReferenceOutput = o;
            }

            return result;
        }
        

        private CalculatedOmniFeeInfo CalculateFinalFee(BitcoinAddress toAddress, BitcoinAddress feeAddress,
                                      Coin fromCoin, List<UnspentCoin> feeUnspentCoins,
                                      Money dustCostBTC, FeeRate estimateFeeRate, TxOut opReturnOutput)
        {
            var feeCoinTotalAmount = feeUnspentCoins.Select(o => o.Amount).Sum();
            var totalCoinAmount = feeCoinTotalAmount + fromCoin.Amount;

            var totalBTC = Money.Zero;
            var inputAmount = Money.Zero;
            
            List<Coin> inputCoins = null;
            int size = 0;
            Money minerFee = Money.Zero;

            do
            {
                var builder = NetworkOperator.Instance.Network.CreateTransactionBuilder();

                var feeCoins = BTCOperator.Instance.SelectCoinsToSpent(feeUnspentCoins, minerFee);

                var tx = builder.AddCoins(fromCoin)
                                .AddCoins(feeCoins)
                                .Send(toAddress, dustCostBTC)
                                .SetChange(feeAddress)
                                .SendFees(minerFee)
                                .SetCoinSelector(new USDTCoinSelector(fromCoin.Outpoint))
                                .BuildTransaction(false);

                tx.Outputs.Add(opReturnOutput);

                size = builder.EstimateSize(tx);
                var feeAmount = size * estimateFeeRate.SatoshiPerByte;
                minerFee = new Money(feeAmount, MoneyUnit.Satoshi);
                
                totalBTC = dustCostBTC + minerFee;

                inputCoins = builder.FindSpentCoins(tx).Cast<Coin>().ToList();
                inputAmount = inputCoins.Select(o => o.TxOut.Value).Sum();

                if (totalBTC > totalCoinAmount)
                    throw new WTException(ExceptionCode.InsufficientBTC, "发送地址与手续费地址总共的BTC不足够支付本次交易的总额: " + totalBTC.ToString() + " BTC");
            }
            while (totalBTC > inputAmount);

            return new CalculatedOmniFeeInfo
            {
                Fee = minerFee,
                InputCoins = inputCoins,
                TransactionSize = size
            };
        }

        // https://github.com/OmniLayer/omnicore/issues/836#issuecomment-474434143
        private void ReorganizeOutput(Transaction tx, BitcoinAddress to, TxOut opReturnOutput)
        {
            tx.Outputs.Add(opReturnOutput);

            var referenceOutput = tx.Outputs.Where(o => o.ScriptPubKey.GetDestinationAddress(NetworkOperator.Instance.Network) == to)
                                            .OrderBy(o => o.Value)
                                            .First();

            tx.Outputs.Remove(referenceOutput);

            tx.Outputs.Add(referenceOutput);

        }




    }
}
