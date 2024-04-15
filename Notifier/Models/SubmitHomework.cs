namespace Notifier.Models
{
    public class SubmitHomework
    {
        public long StudentId { get; set; }
        public string CourseId { get; set; }
        public string LessonId { get; set; }
        public string Text { get; set; }
        public long HomeworkId { get; set; }
    }
}
