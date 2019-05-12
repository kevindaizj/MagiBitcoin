using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Dao.Common;
using USDTWallet.DataContext.Context;
using USDTWallet.Models.Models.Transactions;

namespace USDTWallet.Dao.Transaction
{
    public class TransactionDao : DaoBase
    {

        public BaseTransactionInfo GetTransactionByTxId(string txId)
        {
            using (var db = this.GetWalletContext())
            {
                var query = from t in db.BASE_TRANSACTION
                            where t.TRANSACTION_ID == txId
                            select new BaseTransactionInfo
                            {
                                Id = t.ID,
                                TransactionId = t.TRANSACTION_ID,
                                TransactionType = t.TRANSACTION_TYPE,
                                FromAddress = t.FROM_ADDRESS,
                                ToAddress = t.TO_ADDRESS,
                                ChangeAddress = t.CHANGE_ADDRESS,
                                FeeAddress = t.FEE_ADDRESS,
                                FeeRate = t.FEE_RATE,
                                EstimateSize = t.ESTIMATE_SIZE,
                                Amount = t.AMOUNT,
                                IsSigned = t.IS_SIGNED,
                                BlockHash = t.BLOCK_HASH,
                                Confirmations = t.CONFIRMATIONS,
                                BlockTime = t.BLOCK_TIME,
                                CreateDate = t.CREATE_DATE
                            };

                return query.SingleOrDefault();
            }
        }

        public void Create(BaseTransactionInfo model)
        {
            using (var db = this.GetWalletContext())
            {
                var entity = new BASE_TRANSACTION
                {
                    ID = model.Id,
                    TRANSACTION_ID = model.TransactionId,
                    TRANSACTION_TYPE = model.TransactionType,
                    FROM_ADDRESS = model.FromAddress,
                    TO_ADDRESS = model.ToAddress,
                    CHANGE_ADDRESS = model.ChangeAddress,
                    FEE_ADDRESS = model.FeeAddress,
                    FEE_RATE = model.FeeRate,
                    ESTIMATE_SIZE = model.EstimateSize,
                    AMOUNT = model.Amount,
                    IS_SIGNED = model.IsSigned,
                    BLOCK_HASH = model.BlockHash,
                    CONFIRMATIONS = model.Confirmations,
                    BLOCK_TIME = model.BlockTime,
                    CREATE_DATE = model.CreateDate
                };

                

                db.BASE_TRANSACTION.Add(entity);

                var errors = db.GetValidationErrors();

                db.SaveChanges();
            }
        }

        public void Sign(string unsignedTxId, string txId)
        {
            using (var db = this.GetWalletContext())
            {
                var query = from t in db.BASE_TRANSACTION
                            where t.TRANSACTION_ID == unsignedTxId
                            select t;

                var entity = query.SingleOrDefault();
                if(null != entity)
                {
                    entity.TRANSACTION_ID = txId;
                    entity.IS_SIGNED = true;
                    db.SaveChanges();
                }
            }
        }
    }
}
