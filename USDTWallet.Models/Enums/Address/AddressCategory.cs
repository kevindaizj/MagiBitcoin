using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USDTWallet.Models.Enums.Address
{
    public enum AddressCategory
    {
        /// <summary>
        /// 默认未分类地址
        /// </summary>
        Default = 0,
        /// <summary>
        /// 付款地址
        /// </summary>
        Payer = 1,
        /// <summary>
        /// 收款地址
        /// </summary>
        Receiver = 2,
    }
}
