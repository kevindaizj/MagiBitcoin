using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Models.Common;

namespace USDTWallet.Models.Models.Transactions
{
    public class TransactionInfoVM : ViewModelBase
    {
        private string _txId;
        public string TransactionId
        {
            get { return _txId; }
            set { SetProperty(ref _txId, value); }
        }


        private int _type;
        public int TransactionType
        {
            get { return _type; }
            set { SetProperty(ref _type, value); }
        }


        private string _from;
        public string FromAddress
        {
            get { return _from; }
            set { SetProperty(ref _from, value); }
        }

        private string _to;
        public string ToAddress
        {
            get { return _to; }
            set { SetProperty(ref _to, value); }
        }

        private string _changeAddress;
        public string ChangeAddress
        {
            get { return _changeAddress; }
            set { SetProperty(ref _changeAddress, value); }
        }

        private string _feeAddress;
        public string FeeAddress
        {
            get { return _feeAddress; }
            set { SetProperty(ref _feeAddress, value); }
        }

        private FeeRate _feeRate;
        public FeeRate FeeRate
        {
            get { return _feeRate; }
            set { SetProperty(ref _feeRate, value); }
        }

        private int _size;
        public int EstimateSize
        {
            get { return _size; }
            set { SetProperty(ref _size, value); }
        }

        private Money _fee;
        public Money Fee
        {
            get { return _fee; }
            set { SetProperty(ref _fee, value); }
        }

        private Money _amount;
        public Money Amount
        {
            get { return _amount; }
            set { SetProperty(ref _amount, value); }
        }

        private string _blockHash;
        public string BlockHash
        {
            get { return _blockHash; }
            set { SetProperty(ref _blockHash, value); }
        }

        private long _confimations;
        public long Confirmations
        {
            get { return _confimations; }
            set { SetProperty(ref _confimations, value); }
        }

        private DateTime _createDatetime;
        public DateTime CreateDate
        {
            get { return _createDatetime; }
            set { SetProperty(ref _createDatetime, value); }
        }

        private bool _isBTC;
        public bool IsBTC
        {
            get { return _isBTC; }
            set { SetProperty(ref _isBTC, value); }
        }

        private bool _isConf;
        public bool IsConfirmed
        {
            get { return _isConf; }
            set { SetProperty(ref _isConf, value); }
        }

    }
}
