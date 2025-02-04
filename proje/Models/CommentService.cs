using MongoDB.Driver;
using proje.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

public class CommentService
{
    private readonly IMongoCollection<Comments> _comments;

    public CommentService(IMongoDatabase database)
    {
        _comments = database.GetCollection<Comments>("comment");
    }

    public async Task<List<Comments>> GetCommentsByBookIdAsync(string bookId)
{
    return await _comments
        .Find(comment => comment.BookId == bookId)
        .ToListAsync();
}


    public async Task<bool> AddCommentAsync(Comments comment)
    {
        try
        {
            await _comments.InsertOneAsync(comment);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> DeleteCommentAsync(string commentId)
    {
        var result = await _comments.DeleteOneAsync(comment => comment.Id == commentId);
        return result.DeletedCount > 0;
    }
}
