using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgreSQLModelsLibrary
{
    public class Access : BaseEntity
    {
        public long UserId { get; set; }
        public string? CourseId { get; set; }
        public bool Read { get; set; }
        public bool Write { get; set; }
        public bool Delete { get; set; }
    }
}
