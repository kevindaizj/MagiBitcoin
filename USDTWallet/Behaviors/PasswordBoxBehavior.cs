using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace USDTWallet.Behaviors
{
    public class PasswordBoxBehavior : Behavior<PasswordBox>
    {

        protected override void OnAttached()
        {
            AssociatedObject.KeyUp += ChangeBindingValue;
            AssociatedObject.Loaded += ChangeBindingValue;
            base.OnAttached();
        }

        protected override void OnDetaching()
        {
            AssociatedObject.KeyUp -= ChangeBindingValue;
            AssociatedObject.Loaded -= ChangeBindingValue;
            base.OnDetaching();
        }

        public static readonly DependencyProperty TransferPasswordProperty =
            DependencyProperty.Register("TransferPassword", typeof(SecureString), typeof(PasswordBoxBehavior), new FrameworkPropertyMetadata(null) { BindsTwoWayByDefault = true });


        public SecureString TransferPassword
        {
            get
            {
                return (SecureString)GetValue(TransferPasswordProperty);
            }
            set
            {
                SetValue(TransferPasswordProperty, value);
            }
        }

        private static void ChangeBindingValue(object sender, RoutedEventArgs e)
        {
            PasswordBox pwdbox = sender as PasswordBox;
            PasswordBoxBehavior behavior = Interaction.GetBehaviors(pwdbox).OfType<PasswordBoxBehavior>().FirstOrDefault();
            if (behavior != null)
            {
                if (string.IsNullOrWhiteSpace(pwdbox.Password))
                {
                    behavior.TransferPassword = null;
                }
                else
                    behavior.TransferPassword = pwdbox.SecurePassword;
            }
        }
    }
}
