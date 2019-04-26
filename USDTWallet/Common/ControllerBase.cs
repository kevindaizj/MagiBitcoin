using Prism;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using USDTWallet.Models.Common;

namespace USDTWallet.Common
{
    public class ControllerBase : ViewModelBase, IActiveAware, INavigationAware
    {
        public event EventHandler IsActiveChanged;

        private bool IsInited { get; set; }

        bool _isActive;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                _isActive = value;
                OnIsActiveChanged();
            }
        }

        private void OnIsActiveChanged()
        {
            if (IsActive)
            {
                if (!IsInited)
                {
                    IsInited = true;
                    OnInit();
                }

                OnActivated();
            }
            else
            {
                OnInactivated();
            }

            IsActiveChanged?.Invoke(this, new EventArgs());
        }


        protected virtual void OnInit()
        {
        }

        protected virtual void OnActivated()
        {
        }

        protected virtual void OnInactivated()
        {

        }

        public virtual void OnNavigatedTo(NavigationContext navigationContext)
        {
            return;
        }

        public virtual bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public virtual void OnNavigatedFrom(NavigationContext navigationContext)
        {
            return;
        }
    }
}
