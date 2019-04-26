using Prism.Mvvm;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace USDTWallet.Models.Common
{
    public class ViewModelBase : BindableBase, INotifyDataErrorInfo
    {
        protected override bool SetProperty<T>(ref T storage, T value, Action onChanged, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            bool flag = base.SetProperty<T>(ref storage, value, onChanged, propertyName);

            this.Validate(propertyName);

            return flag;
        }

        protected override bool SetProperty<T>(ref T storage, T value, [System.Runtime.CompilerServices.CallerMemberName] string propertyName = null)
        {
            bool flag = base.SetProperty<T>(ref storage, value, propertyName);
            this.Validate(propertyName);
            return flag;
        }


        public void TriggerValidation()
        {
            var type = this.GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            var propertyNames = properties.Select(q => q.Name);
            foreach (var p in propertyNames)
            {
                Validate(p);
            }
        }

        public void ResetValidation()
        {
            var type = this.GetType();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
            var propertyNames = properties.Select(q => q.Name);
            foreach (var p in propertyNames)
            {
                SetErrors(p, new List<string>());
            }
        }


        private void Validate(string propertyName)
        {
            var propertyInfo = this.GetType().GetRuntimeProperty(propertyName);
            if (propertyInfo == null)
            {
                var errorString = "InvalidPropertyNameException";
                throw new ArgumentException(errorString, propertyName);
            }

            var propertyErrors = new List<string>();
            bool isValid = TryValidateProperty(propertyInfo, propertyErrors);
            SetErrors(propertyInfo.Name, propertyErrors);
        }


        private bool TryValidateProperty(PropertyInfo propertyInfo, List<string> propertyErrors)
        {
            var results = new List<ValidationResult>();
            var context = new ValidationContext(this) { MemberName = propertyInfo.Name };
            var propertyValue = propertyInfo.GetValue(this);

            // Validate the property
            bool isValid = Validator.TryValidateProperty(propertyValue, context, results);

            if (results.Any())
            {
                propertyErrors.AddRange(results.Select(c => c.ErrorMessage));
            }

            return isValid;
        }

        private Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();

        public bool HasErrors
        {
            get
            {
                return errors.Any(q => q.Value.Count > 0);
            }
        }

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                return errors.Values;
            else
            {
                if (errors.ContainsKey(propertyName))
                    return errors[propertyName];
                else
                    return null;
            }
        }

        private void SetErrors(string propertyName, List<string> propertyErrors)
        {
            errors.Remove(propertyName);

            errors.Add(propertyName, propertyErrors);

            if (ErrorsChanged != null)
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }

        private void ClearErrors(string propertyName)
        {
            errors.Remove(propertyName);
            if (ErrorsChanged != null)
                ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }
}
