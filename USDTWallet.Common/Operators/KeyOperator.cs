﻿using NBitcoin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Models.Models.Mnemonics;

namespace USDTWallet.Common.Operators
{
    public class KeyOperator
    {
        private static Lazy<KeyOperator> _instance = new Lazy<KeyOperator>(() => new KeyOperator(Network.RegTest));
        public static KeyOperator Instance
        {
            get { return _instance.Value; }
        }

        private Network Network { get; set; }

        private KeyOperator(Network network)
        {
            this.Network = network;
        }

        public MnemonicResult CreateMnemonicRoot(string password)
        {
            var mnemonic = new Mnemonic(Wordlist.English, WordCount.TwentyFour);
            var rootKey = mnemonic.DeriveExtKey(password);
            var rootAddress = rootKey.GetPublicKey().GetAddress(this.Network).ToString();
            return new MnemonicResult
            {
                Network = (this.Network == Network.Main) ? (int)NetworkType.Mainnet : (int)NetworkType.Testnet,
                RootAddress = rootAddress,
                MnemonicWords = mnemonic.Words
            };
        }
    }
}
