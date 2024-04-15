using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedMongoDBModelLibrary
{
    public class Question
    {
        public string Text { get; set; } = null!;
        public List<Response> Responses { get; set; } = null!;
    }
}
