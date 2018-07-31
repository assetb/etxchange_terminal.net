using System;
using System.Diagnostics;
using System.IO;
using MS.WindowsAPICodePack.Internal;
using Microsoft.WindowsAPICodePack.Shell.PropertySystem;
using Windows.UI.Notifications;

namespace AltaWindowsNotification
{
    public class ToastNotification
    {
        private const string AppId = "AltaIK.ETS.Notification";


        public ToastNotification()
        {
            TryCreateShortcut();
        }

        // In orderDocument to display toasts, a desktop application must have a shortcut on the Start menu.
        // Also, an AppUserModelID must be set on that shortcut.
        // The shortcut should be created as part of the installer. The following code shows how to create
        // a shortcut and assign an AppUserModelID using Windows APIs. You must download and include the 
        // Windows API Code Pack for Microsoft .NET Framework for this code to function
        //
        // Included in this project is a wxs file that be used with the WiX toolkit
        // to make an installer that creates the necessary shortcut. One or the other should be used.
        private void TryCreateShortcut()
        {
            var shortcutPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "\\Microsoft\\Windows\\Start Menu\\Programs\\AltaIK ETS.lnk";
            if (!File.Exists(shortcutPath))
            {
                InstallShortcut(shortcutPath);
            }
        }

        private static void InstallShortcut(string shortcutPath)
        {
            // Find the path to the current executable
            var exePath = Process.GetCurrentProcess().MainModule.FileName;
            // ReSharper disable once SuspiciousTypeConversion.Global
            var newShortcut = (IShellLinkW)new CShellLink();

            // Create a shortcut to the executable file
            ErrorHelper.VerifySucceeded(newShortcut.SetPath(exePath));
            ErrorHelper.VerifySucceeded(newShortcut.SetArguments(""));

            // Open the shortcut property store, set the AppUserModelId property
            // ReSharper disable once SuspiciousTypeConversion.Global
            var newShortcutProperties = (IPropertyStore)newShortcut;

            using (var appId = new PropVariant(AppId))
            {
                ErrorHelper.VerifySucceeded(newShortcutProperties.SetValue(SystemProperties.System.AppUserModel.ID, appId));
                ErrorHelper.VerifySucceeded(newShortcutProperties.Commit());
            }

            // Commit the shortcut to disk
            // ReSharper disable once SuspiciousTypeConversion.Global
            var newShortcutSave = newShortcut as IPersistFile;

            if (newShortcutSave != null) ErrorHelper.VerifySucceeded(newShortcutSave.Save(shortcutPath, true));
        }


        public void Show(string notificationText)
        {
            Show(notificationText, false);
        }


        private Action _exeCmd;
        private Action<object> _exeWithParCmd;
        private object _parForExe;

        public void Show(string notificationText, Action exeCmd)
        {
            _exeCmd = exeCmd;
            Show(notificationText,true);
        }

        public void Show(string notificationText, Action<object> exeCmd, object par)
        {
            _exeWithParCmd = exeCmd;
            _parForExe = par;
            Show(notificationText,true);
        }


        // Create and show the toast.
        private void Show(string notificationText, bool isActions)
        {
            // Get a toast XML template
            //XmlDocument toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText04);
            var toastXml = ToastNotificationManager.GetTemplateContent(ToastTemplateType.ToastImageAndText01);

            // Fill in the text elements
            var stringElements = toastXml.GetElementsByTagName("text");

            //for (int i = 0; i < stringElements.Length; i++)
            //{
            //    stringElements[i].AppendChild(toastXml.CreateTextNode("Line " + i));
            //}

            stringElements[0].AppendChild(toastXml.CreateTextNode(notificationText));

            // Specify the absolute path to an image
            var imagePath = "file:///" + Path.GetFullPath("toastImageAndText.png");
            var imageElements = toastXml.GetElementsByTagName("image");
            imageElements[0].Attributes.GetNamedItem("src").NodeValue = imagePath;

            // Create the toast and attach event listeners
            var toast = new Windows.UI.Notifications.ToastNotification(toastXml);

            if (isActions) {
                toast.Activated += ToastActivated;
                toast.Dismissed += ToastDismissed;
                toast.Failed += ToastFailed;
            }

            // Show the toast. Be sure to specify the AppUserModelId on your application's shortcut!
            ToastNotificationManager.CreateToastNotifier(AppId).Show(toast);
        }


        private void ToastActivated(Windows.UI.Notifications.ToastNotification sender, object e)
        {
            _exeWithParCmd?.Invoke(_parForExe);
            _exeCmd?.Invoke();
        }




        private static void ToastDismissed(Windows.UI.Notifications.ToastNotification sender, ToastDismissedEventArgs e)
        {
            switch (e.Reason)
            {
                case ToastDismissalReason.ApplicationHidden:
                    break;
                case ToastDismissalReason.UserCanceled:
                    break;
                case ToastDismissalReason.TimedOut:
                    break;
            }

        }


        private static void ToastFailed(Windows.UI.Notifications.ToastNotification sender, ToastFailedEventArgs e)
        {
        }

    }
}
