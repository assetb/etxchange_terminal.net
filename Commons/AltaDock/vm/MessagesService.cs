using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MahApps.Metro.Controls.Dialogs;
using MahApps.Metro.Controls;
using System.Windows;

namespace AltaDock.vm {
    public class MessagesService {
        public static MetroWindow metroWindow = Application.Current.MainWindow as MetroWindow;


        public static async void Show(string title, string message) {
            await ShowDialog(title, message);
        }

        public static async Task<MessageDialogResult> ShowDialog(string title, string message) {
            return await metroWindow.ShowMessageAsync(title, message);
        }


        public static async Task<string> GetInput(string title, string inputLabel) {
            return await ShowInputDialog(title, inputLabel);
        }

        private static async Task<string> ShowInputDialog(string title, string inputLabel) {
            return await metroWindow.ShowInputAsync(title, inputLabel);
        }


        public static async Task<ProgressDialogController> ShowProgress() {
            return await metroWindow.ShowProgressAsync("ОБРАБОТКА", "Пожалуйста подождите...");
        }

        public static async Task<bool> AskDialog(string title, string question) {
            var mySettings = new MetroDialogSettings() {
                AffirmativeButtonText="Да",
                NegativeButtonText="Нет"
            };

            MessageDialogResult result = await metroWindow.ShowMessageAsync(title, question, MessageDialogStyle.AffirmativeAndNegative, mySettings);

            if(result == MessageDialogResult.Affirmative) return true;
            else return false;
        }
    }
}
