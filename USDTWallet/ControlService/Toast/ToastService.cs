using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ToastNotifications;
using ToastNotifications.Core;
using ToastNotifications.Lifetime;
using ToastNotifications.Messages;
using ToastNotifications.Position;

namespace USDTWallet.ControlService.Toast
{
    public class ToastService
    {
        private Notifier Instance;

        public ToastService()
        {
            Instance = new Notifier(ConfigNotifier);
        }


        private void ConfigNotifier(NotifierConfiguration cfg)
        {
#if DEBUG
            System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en-US");
#endif

            cfg.PositionProvider = new WindowPositionProvider(
                   parentWindow: Application.Current.MainWindow,
                   corner: Corner.TopRight,
                   offsetX: 10,
                   offsetY: 10);

            cfg.LifetimeSupervisor = new TimeAndCountBasedLifetimeSupervisor(
                notificationLifetime: TimeSpan.FromSeconds(3),
                maximumNotificationCount: MaximumNotificationCount.FromCount(5));

            cfg.Dispatcher = Application.Current.Dispatcher;
        }

        public void Info(string message)
        {
            Instance.ShowInformation(message);
        }

        public void Success(string message)
        {
            Instance.ShowSuccess(message);
        }

        public void Warning(string message)
        {
            Instance.ShowWarning(message);
        }

        public void Error(string message)
        {
            Instance.ShowError(message);
        }

    }
}
