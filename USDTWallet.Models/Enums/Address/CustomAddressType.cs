using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USDTWallet.Models.Enums.Address
{
    public enum CustomAddressType
    {
        /// <summary>
        /// 根
        /// </summary>
        Root = 0,
        /// <summary>
        /// 公司内部地址
        /// </summary>
        Company = 1,
        /// <summary>
        /// 客户地址
        /// </summary>
        Customer = 2
    }
}
