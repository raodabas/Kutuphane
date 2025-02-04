using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace proje.Models;

public partial class Comments
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }
    public string? BookId { get; set; }
    public string? UserId { get; set; }
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }
}
