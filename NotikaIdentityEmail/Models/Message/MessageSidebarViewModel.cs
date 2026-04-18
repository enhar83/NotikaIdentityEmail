namespace NotikaIdentityEmail.Models.Message
{
    public class MessageSidebarViewModel
    {
        public int IncomingMessageCount { get; set; }
        public int OutcomingMessageCount { get; set; }
        public int DraftMessageCount { get; set; }
        public int SendedTrashBinMessageCount { get; set; }
        public int ReceivedTrashBinMessageCount { get; set; }
    }
}
