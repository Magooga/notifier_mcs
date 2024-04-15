namespace Notifier.Models
{
    public class TeacherMessageHomework
    {
        public string Text { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpDate { get; set; }
        public long TeacherId { get; set; }

        // Сообщение студенту (ToStudentId) от учителя
        public long ToStudentId { get; set; }
        public long HomeworkId { get; set; }
    }
}
