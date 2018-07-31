using System.Collections.Generic;
using altaik.baseapp.vm;
using altaik.baseapp.vm.command;
using AltaBO;
using System.IO;
using System.Collections.ObjectModel;
using AltaTransport.specifics;

namespace DocumentFormation.vm
{
    public class EmailSendViewModel : PanelViewModelBase
    {
        private EmployesConst employesConst;        
        private MailSending mailSending;
        

        public EmailSendViewModel()
        {
            employesConst = new EmployesConst();
            Employes = employesConst.GetListOfEmployes();
        }


        protected override List<CommandViewModel> CreateCommands()
        {
            return new List<CommandViewModel> {
                new CommandViewModel("Прикрепить", new DelegateCommand(p=>this.OnAttach())),
                new CommandViewModel("Удалить", new DelegateCommand(p=>this.OnDelete())),
                new CommandViewModel("Очистить", new DelegateCommand(p=>this.OnClear())),
                new CommandViewModel("Отправить", new DelegateCommand(p=>this.OnSend()))
            };
        }


        private void OnAttach()
        {
            string fileInfo;
            fileInfo = Service.GetFile("Выберите файл для прикрепления", "Все файлы (*.*)|*.*").FullName;

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
            mailSending = new MailSending();
            mailSending.SendMail(Employe.email, Employe.pass, Recipient, Subject, Content, AttachedFiles);
        }


        private List<Employe> employes;
        public List<Employe> Employes {
            get { return employes; }
            set { employes = value; RaisePropertyChangedEvent("Employes"); }
        }


        private Employe employe;
        public Employe Employe {
            get { return employe; }
            set { employe = value; RaisePropertyChangedEvent("Employe"); }
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