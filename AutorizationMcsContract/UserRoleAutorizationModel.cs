using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutorizationMcsContract
{
    /// <summary>
    /// Contract Model between microservices, Autorization Microservice receive User_id, and then reply with List of UserRoleAutorizationModel  
    /// </summary>
    public class UserRoleAutorizationModel
    {
        public long User_Id { get; set; }
        public long Role_Id { get; set; }
        public DateTime UpDate { get; set; }
        public DateTime CreateDate { get; set; }
        public bool Deleted { get; set; } // Deleted // 1 - deleted, 0 - not deleted
    }
}
