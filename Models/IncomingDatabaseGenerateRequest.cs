namespace RokonoDbManager.Models
{
    public class IncomingDatabaseGenerateRequest
    {
        public string DatabaseName { get; set; }
        public string ConnectionId { get; set; }
        public string FilePath { get; set; }
    }
}