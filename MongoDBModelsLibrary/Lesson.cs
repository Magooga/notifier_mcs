using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedMongoDBModelLibrary
{
    public class Lesson
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Homework { get; set; } = null!;
        public DateTime DateCreated { get; set; }
        public DateTime DateUpdated { get; set; }
        public bool IsRemoved { get; set; }
        public List<Question> Test { get; set; } = null!;
        public List<string> Links { get; set; } = null!;
    }
}
