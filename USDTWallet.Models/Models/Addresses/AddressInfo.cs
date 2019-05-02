using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USDTWallet.Models.Models.Addresses
{
    public class AddressInfo
    {
        public string Id { get; set; }
        public string Address { get; set; }
        public string ExtPubKeyWif { get; set; }
        public string WalletId { get; set; }
        public long Network { get; set; }
        public string KeyPath { get; set; }
        public string ParentKeyPath { get; set; }
        public long PathIndex { get; set; }
        public long AddressType { get; set; }
        public long AddressCategory { get; set; }
        public string Name { get; set; }
        public double Balance { get; set; }
        public string Account { get; set; }
        public string Description { get; set; }

    }
}
