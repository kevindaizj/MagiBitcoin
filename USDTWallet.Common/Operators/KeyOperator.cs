using NBitcoin;
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
        private static Lazy<KeyOperator> _instance = new Lazy<KeyOperator>(() => new KeyOperator());
        public static KeyOperator Instance
        {
            get { return _instance.Value; }
        }
        
        private KeyOperator()
        {
        }
        

        public MnemonicResult CreateMnemonicRoot(string password)
        {
            var mnemonic = new Mnemonic(Wordlist.English, WordCount.TwentyFour);
            return this.GenerateByMnemonic(mnemonic, password);
        }

        public MnemonicResult Recover(string password, string mnemoniWords)
        {
            var mnemonic = new Mnemonic(mnemoniWords, Wordlist.English);
            return this.GenerateByMnemonic(mnemonic, password);
        }

        private MnemonicResult GenerateByMnemonic(Mnemonic mnemonic, string password)
        {
            var rootKey = mnemonic.DeriveExtKey(password);
            var rootPubKey = rootKey.Neuter();
            var rootExtPubKeyWif = rootPubKey.GetWif(NetworkOperator.Instance.Network).ToWif();
            var rootAddress = rootPubKey.GetPublicKey().GetAddress(NetworkOperator.Instance.Network).ToString();
            return new MnemonicResult
            {
                Network = (NetworkOperator.Instance.Network == Network.Main) ? (int)NetworkType.Mainnet : (int)NetworkType.Testnet,
                RootAddress = rootAddress,
                RootExtPrivKeyWif = rootKey.GetWif(NetworkOperator.Instance.Network).ToWif(),
                RootExtPubKeyWif = rootExtPubKeyWif,
                MnemonicWords = mnemonic.Words
            };
        }

        public string DeriveNewAddress(ExtKey rootXPrivKey, KeyPath keyPath)
        {
            var key = rootXPrivKey.Derive(keyPath);
            var address = key.GetPublicKey().GetAddress(NetworkOperator.Instance.Network).ToString();
            return address;
        }

        public string DeriveNewAddress(ExtPubKey rootXPubKey, KeyPath keyPath)
        {
            var key = rootXPubKey.Derive(keyPath);
            var address = key.GetPublicKey().GetAddress(NetworkOperator.Instance.Network).ToString();
            return address;
        }
    }
}
