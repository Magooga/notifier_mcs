namespace Notifier.Models
{
    public class StudentMessageHomework
    {
        public string Text { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? UpDate { get; set; }
        public long UserId { get; set; }

        // Связь с таблицей Homework
        public string CourseId { get; set; }
        public long HomeworkId { get; set; }
    }
}
