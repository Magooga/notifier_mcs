using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PostgreSQLModelsLibrary
{
    public class Progress : BaseEntity
    {
        public long UserId { get; set; }
        public string? CourseId { get; set; }
        public string? LessonId { get; set; }
        public bool IsDone { get; set; }
        public bool DoneHomework { get; set; }
    }
}
