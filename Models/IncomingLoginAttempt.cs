namespace RokonoDbManager.Models
{
    public class IncomingLoginAttempt
    {
        public string Host { get; set; }    
        public string Username { get; set; }    
        public string Password { get; set; }
        public bool Remember { get; set; }
        public string Database { get; set; }
        public int ConnectionId {get; set;}
    }
}