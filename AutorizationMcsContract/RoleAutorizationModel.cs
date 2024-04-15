using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutorizationMcsContract
{
    /// <summary>
    /// Contract Model between microservices. 
    /// - Autorization Microservice receive request for all exist roles and reply with List of RoleAutorizationModel
    /// - Autorization Microservice receive request for Role_id and reply with RoleAutorizationModel 
    /// </summary>
    public class RoleAutorizationModel
    {
        public long Id { get; set; }
        public string ?Name { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpDate { get; set; }
        public bool Deleted { get; set; } // Deleted // 1 - deleted, 0 - not deleted
    }
}
