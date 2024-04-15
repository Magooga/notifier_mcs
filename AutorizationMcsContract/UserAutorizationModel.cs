using System.Data;

namespace AutorizationMcsContract
{
    /// <summary>
    /// Contract Model between microservices.
    /// - Autorization Microservice receive request for all exist users and reply with List of UserAutorizationModel
    /// - Autorization Microservice receive request for User_id and reply with UserAutorizationModel
    /// </summary>
    public class UserAutorizationModel
    {
        public string ?FirstName { get; set; }
        public string ?LastName { get; set; }
        public string ?Email { get; set; }
        public string Password { get; set; }
        //public Byte[] Salt { get; set; } = new Byte[20];   // salt // for password
        //public Byte[] Hash { get; set; } = new Byte[20];   // hash // for password
        public DateTime CreateDate { get; set; }
        public DateTime UpDate { get; set; }
        public bool Deleted { get; set; } // Deleted // 1 - deleted, 0 - not deleted
    }
}