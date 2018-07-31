using System.Collections.Generic;
using altaik.baseapp.vm;
using altaik.baseapp.vm.command;
using AltaBO;
using System.IO;
using System.Collections.ObjectModel;
using AltaTransport.specifics;

namespace AltaTransport.vm
{
    public class EmailSendViewModel : PanelViewModelBase
    {

        public EmailSendViewModel()
        {
            Traders = ConstantData.Traders;
        }


        protected override List<CommandViewModel> CreateCommands()
        {
            return new List<CommandViewModel> {
                new CommandViewModel("Прикрепить", new DelegateCommand(p=>OnAttach())),
                new CommandViewModel("Удалить", new DelegateCommand(p=>OnDelete())),
                new CommandViewModel("Очистить", new DelegateCommand(p=>OnClear())),
                new CommandViewModel("Отправить", new DelegateCommand(p=>OnSend()))
            };
        }


        private void OnAttach() {
            var fileInfo = Service.GetFile("Выберите файл для прикрепления", "Все файлы (*.*)|*.*").FullName;

            AttachedFiles.Add(new AttachedFiles { fileName = Path.GetFileName(fileInfo), fullName = fileInfo });
        }


        private void OnDelete()
        {
            AttachedFiles.Remove(AttachedFile);
        }


        private void OnClear()
        {
            AttachedFiles.Clear();
        }


        private void OnSend() {
            EmailSender.Send(Trader.email, Trader.pass, Recipient, Subject, Content, AttachedFiles);
        }


        private List<Trader> traders;
        public List<Trader> Traders {
            get { return traders; }
            set { traders = value; RaisePropertyChangedEvent("Traders"); }
        }


        private Trader trader;
        public Trader Trader {
            get { return trader; }
            set { trader = value; RaisePropertyChangedEvent("Trader"); }
        }


        private ObservableCollection<AttachedFiles> attachedFiles = new ObservableCollection<AttachedFiles>();
        public ObservableCollection<AttachedFiles> AttachedFiles {
            get { return attachedFiles; }
            set { attachedFiles = value; RaisePropertyChangedEvent("AttachedFiles"); }
        }


        private AttachedFiles attachedFile = new AttachedFiles();
        public AttachedFiles AttachedFile {
            get { return attachedFile; }
            set { attachedFile = value; RaisePropertyChangedEvent("AttachedFile"); }
        }


        private string recipient;
        public string Recipient {
            get { return recipient; }
            set { recipient = value;RaisePropertyChangedEvent("Recipient"); }
        }


        private string subject;
        public string Subject {
            get { return subject; }
            set { subject = value; RaisePropertyChangedEvent("Subject"); }
        }


        private string content;
        public string Content {
            get { return content; }
            set { content = value; RaisePropertyChangedEvent("Content"); }
        }

    }
}