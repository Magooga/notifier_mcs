using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SharedMongoDBModelLibrary
{
    public class Course
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Title { get; set; } = null!;
        public string Promotion { get; set; } = null!;
        public string Description { get; set; } = null!;
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public bool IsRemoved { get; set; }
        public List<Lesson> Lessons { get; set; } = null!;
    }
}