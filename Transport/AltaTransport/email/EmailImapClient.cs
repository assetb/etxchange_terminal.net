using LumiSoft.Net;
using LumiSoft.Net.IMAP;
using LumiSoft.Net.IMAP.Client;
using LumiSoft.Net.Mail;
using LumiSoft.Net.MIME;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AltaLog;

namespace AltaTransport
{
    /// <summary>
    /// Creates imap client using provided email settings.
    /// It returns HasNewMessages and GetNewMessageHeads functions to check if new emails exist and to return headers of new emails.
    /// Email itself is returned by GetMessage(uid) function invoke.
    /// </summary>
    internal class EmailImapClient
    {
        #region Local Fields

        private EmailSettings _settings;
        private IMAP_Client _client;
        private long _savedSeqNo = -1;
        private readonly string _seqSet;
        private SeqSetCriteriaEnum _criteria;
        private List<Email> _messages;
        private Email _curMessage;

        #endregion


        #region Constructors

        private void Init(EmailSettings settings) {
            _settings = settings;
            _messages = new List<Email>();
            //_client = new IMAP_Client();
        }


        internal EmailImapClient(EmailSettings settings) {
            Init(settings);
            _criteria = SeqSetCriteriaEnum.Unseen;
        }


        internal EmailImapClient(EmailSettings settings, string seqSet) {
            Init(settings);
            _seqSet = seqSet;
            _criteria = SeqSetCriteriaEnum.Handle;
        }


        internal EmailImapClient(EmailSettings settings, int savedSeqNo) {
            Init(settings);
            _savedSeqNo = savedSeqNo;
            _seqSet = savedSeqNo + ":*";
            _criteria = SeqSetCriteriaEnum.SaveNo;
        }


        internal EmailImapClient(EmailSettings settings, SeqSetCriteriaEnum criteria) {
            Init(settings);
            _criteria = criteria;
        }

        #endregion


        #region API

        internal bool IsConnected() {
            return (_client != null) && _client.IsConnected;
        }


        internal bool IsAuthenticated() {
            return (_client != null) && _client.IsAuthenticated;
        }


        internal bool HasNewMessages() {
            if (_client == null || !_client.IsConnected) if (!ConnectToEmailServer(_settings.Security == SecurityEnum.SSL)) return false;

            if (_client == null) return false;

            _client.SelectFolder(_settings.Folder);

            return _client.SelectedFolder.FirstUnseen != -1;
        }


        internal void Close() {
            Disconnect();
            if (!_client.IsDisposed) _client.Dispose();
            _messages.Clear();
            _messages = null;
            _curMessage = null;
            _settings = null;
        }

        internal void SetLastLoadedMessageUid(long uid) {}


        internal List<Email> GetNewMessageHeads() {
            if (!_client.IsConnected) if (!ConnectToEmailServer(_settings.Security == SecurityEnum.SSL)) return null;

            AppJournal.Write("Connect с почтой прошел.");

            _messages?.Clear();
            _messages = new List<Email>();
            LoadFolder();

            return _messages;
        }


        internal Email GetMessage(long uid) {
            if ((_messages == null) && (GetNewMessageHeads() == null)) return null;
            LoadMessage(uid);
            return _curMessage;
        }


        internal string GetUser() {
            return _settings.User;
        }

        #endregion


        #region Connect to email server

        private void Disconnect()
        {
            try {
                if (_client != null && _client.IsConnected) _client.Disconnect();
            } catch (Exception ex) {
                //Debug.WriteLine(this.GetType().Name + ": " + ex.Message);
                AppJournal.Write(GetType().Name + ": " + ex.Message);
            }
        }


        /// <summary>
        /// Connects to email server.
        /// </summary>
        /// <param name="useSSL">if set to <c>true</c> [use SSL].</param>
        /// <returns></returns>
        private bool ConnectToEmailServer(bool useSSL)
        {
            Disconnect();
            if(_client != null && !_client.IsDisposed) _client.Dispose();
            _client = new IMAP_Client();
            try {
                if (useSSL) _client.Connect(_settings.Server, WellKnownPorts.IMAP4_SSL, true);  //port 993
                else        _client.Connect(_settings.Server, WellKnownPorts.IMAP4, false);     //port 143

                if (_settings.Security == SecurityEnum.TLS) _client.StartTls();

                _client.Login(_settings.User, _settings.Pass);

                return true;
            } catch (Exception ex) {
                //Debug.WriteLine(ex.Message);
                AppJournal.Write("EmailImapTransport: ConnectToEmailServer " + ex.Message);
                return false;
            }
        }

        #endregion


        #region method LoadFolder

        private void LoadFolder()
        {
            LoadFolder(_settings.Folder);
        }

        /// <summary>
        /// Gets specified folder messages list from IMAP server.
        /// </summary>
        /// <param name="folder">IMAP folder which messages to load.</param>
        private void LoadFolder(string folder)
        {
            try {
                _client.SelectFolder(folder);

                if(_client.SelectedFolder.MessagesCount == 0) return;

                var seqSet = GetSeqSet();
                if (seqSet == null) throw new ArgumentNullException(nameof(seqSet));
                if (string.IsNullOrWhiteSpace(seqSet)) return;
                AppJournal.Write("EmailImapTransport: LoadFolder: Seq_set = " + GetSeqSet());

                _client.Fetch(false, IMAP_t_SeqSet.Parse(GetSeqSet()),
                                  new IMAP_t_Fetch_i[] {new IMAP_t_Fetch_i_Envelope(),new IMAP_t_Fetch_i_Flags(),new IMAP_t_Fetch_i_InternalDate(),new IMAP_t_Fetch_i_Rfc822Size(),new IMAP_t_Fetch_i_Uid()},
                                  FetchFolderHandler);

                _savedSeqNo = _messages.Last().UID + 1;
                _criteria = SeqSetCriteriaEnum.SaveNo;
            } catch (Exception ex){
                //Debug.WriteLine(ex.Message);
                AppJournal.Write("EmailImapTransport:LoadFolder: " + ex.Message);
            }

        }

        #endregion


        #region method GetSeqSet

        private string GetSeqSet()
        {
            switch (_criteria) {
                case SeqSetCriteriaEnum.SaveNo: return _savedSeqNo + ":*";
                case SeqSetCriteriaEnum.Unseen: var unseenIds = _client.Search(false, Encoding.UTF8, new IMAP_Search_Key_Unseen()); return string.Join(",", unseenIds);
                case SeqSetCriteriaEnum.Handle: return _seqSet;
                case SeqSetCriteriaEnum.Recent: var recentIds = _client.Search(false, Encoding.UTF8, new IMAP_Search_Key_Recent()); return string.Join(",", recentIds);
                default: var defaultIds = _client.Search(false, Encoding.UTF8, new IMAP_Search_Key_Unseen()); return string.Join(",", defaultIds);
            }
        }
        #endregion


        #region method FetchFolderHandler

        /// <summary>
        /// This method is called when FETCH (Envelope Flags InternalDate Rfc822Size Uid) untagged response is received.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event data.</param>
        private void FetchFolderHandler(object sender,EventArgs<IMAP_r_u> e)
        {
            /* NOTE: All IMAP untagged responses may be raised from thread pool thread,
                so all UI operations must use Invoke.
             
               There may be other untagged responses than FETCH, because IMAP server
               may send any untagged response to any command.
            */

            try {
                if (!(e.Value is IMAP_r_u_Fetch)) return;
                var fetchResp = (IMAP_r_u_Fetch)e.Value;

                try {
                    var emailItem = new Email {UID = fetchResp.UID.UID};

                    var from = "";
                    if (fetchResp.Envelope.From != null) {
                        for (var i = 0; i < fetchResp.Envelope.From.Length; i++) {
                            // Don't add ; for last item
                            if (i == fetchResp.Envelope.From.Length - 1) from += fetchResp.Envelope.From[i].ToString();
                            else                                         from += fetchResp.Envelope.From[i] + ";";
                        }
                    } else from = "<none>";

                    emailItem.From = from;
                    emailItem.Subject = fetchResp.Envelope.Subject ?? "<none>";
                    emailItem.Date = (fetchResp.InternalDate.Date.ToString("dd.MM.yyyy HH:mm"));
                    emailItem.Size = (fetchResp.Rfc822Size.Size / (decimal)1000).ToString("f2") + " kb";

                    _messages.Add(emailItem);
                } catch (Exception ex) {
                    //Debug.WriteLine(ex.Message);
                    AppJournal.Write("EmailImapTransport: FetchFolderHandler: " + ex.Message);
                }
            } catch (Exception ex) {
                //Debug.WriteLine(ex.Message);
                AppJournal.Write("EmailImapTransport: FetchFolderHandler: " + ex.Message);
            }
        }

        #endregion


        #region method LoadMessage

        /// <summary>
        /// Load specified IMAP message.
        /// </summary>
        /// <param name="uid">Message IMAP UID value.</param>
        private void LoadMessage(long uid)
        {
            if (_messages == null) return;
            _curMessage = _messages.First(m => m.UID == uid);
            if (_curMessage == null) return;

            try {
                _client.Fetch(true, IMAP_t_SeqSet.Parse(uid.ToString()),
                    new IMAP_t_Fetch_i[] { new IMAP_t_Fetch_i_Rfc822() },
                    FetchMessageHandler
                );

            } catch (Exception ex) {
                //Debug.WriteLine(ex.Message);
                AppJournal.Write("EmailImapTransport: LoadMessage: " + ex.Message);
            }

            //savedSeqNo = selMessage.UID;
            //criteria = SeqSetCriteriaEnum.SaveNo;
        }

        #endregion


        #region method FetchMessageHandler

        /// <summary>
        /// This method is called when FETCH RFC822 untagged response is received.
        /// </summary>
        /// <param name="sender">Sender.</param>
        /// <param name="e">Event data.</param>
        private void FetchMessageHandler(object sender,EventArgs<IMAP_r_u> e) {
            /* NOTE: All IMAP untagged responses may be raised from thread pool thread,
                so all UI operations must use Invoke.
             
               There may be other untagged responses than FETCH, because IMAP server
               may send any untagged response to any command.
            */

            var resp = e.Value as IMAP_r_u_Fetch;
            if (resp == null) return;
            var fetchResp = resp;

            try {
                if (fetchResp.Rfc822 == null) return;
                fetchResp.Rfc822.Stream.Position = 0;
                var mime = Mail_Message.ParseFromStream(fetchResp.Rfc822.Stream);
                fetchResp.Rfc822.Stream.Dispose();

                _curMessage.TextBody = string.IsNullOrWhiteSpace(mime.BodyText) ? mime.BodyHtmlText : mime.BodyText;

                _curMessage.Attachments.Clear();
                var i = 0;
                foreach (var entity in mime.Attachments) {
                    var attachment = new Attachment();
                    i++;
                    if (entity.ContentDisposition?.Param_FileName != null) attachment.Text = entity.ContentDisposition.Param_FileName;
                    else                                                                                       attachment.Text = "untitled" + i;
                    attachment.Body = ((MIME_b_SinglepartBase)entity.Body).Data;
                    _curMessage.Attachments.Add(attachment);
                }
            } catch (Exception ex) {
                //Debug.WriteLine("EmailImapTransport: FetchMessageHandler: " + ex.Message);
                AppJournal.Write("EmailImapTransport: FetchMessageHandler: " + ex.Message);
            }
        }

        #endregion


        #region method DeleteMessage

        /// <summary>
        /// Deletes specified message.
        /// </summary>
        /// <param name="uid">Message UID.</param>
        private void DeleteMessage(long uid)
        {
            try {
                /* NOTE: In IMAP message deleting is 2 step operation.
                 *  1) You need to mark message deleted, by setting "Deleted" flag.
                 *  2) You need to call Expunge command to force server to dele messages physically.
                */

                var sequenceSet = IMAP_t_SeqSet.Parse(uid.ToString());
                _client.StoreMessageFlags(true,sequenceSet,IMAP_Flags_SetType.Add,new IMAP_t_MsgFlags(IMAP_t_MsgFlags.Deleted));
                _client.Expunge();
            }
            catch(Exception ex){
                //Debug.WriteLine(ex.Message);
                AppJournal.Write("EmailImapTransport: DeleteMessage: " + ex.Message);
            }
        }

        #endregion

    }
}
