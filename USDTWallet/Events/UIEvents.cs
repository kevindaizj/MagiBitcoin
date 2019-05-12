using Prism.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace USDTWallet.Events
{
    public class LoginSuccessEvent : PubSubEvent {}

    public class CreateAddressSuccessEvent : PubSubEvent<long> { }

    public class ChangeNetworkEvent: PubSubEvent { }

    public class TransactionCreated : PubSubEvent { }
}
