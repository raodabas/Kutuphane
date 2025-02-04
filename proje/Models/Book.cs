using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace proje.Models;

public partial class Book
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string? Image { get; set; } 
    public string? Title { get; set; }
    public string? Author { get; set; }
    public int Year { get; set; }
    public string? Genre { get; set; }
    public string? Summary { get; set; }
    public List<string> Comments { get; set; } = new List<string>();
}
