using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;
using proje.Models;

public class BookService
{
    private readonly IMongoCollection<Book> _books;

    public BookService(IMongoDatabase database)
    {
        _books = database.GetCollection<Book>("Books");
    }

    public async Task<List<Book>> GetAllBooksAsync()
    {
        return await _books.Find(book => true).ToListAsync();
    }

    public async Task<bool> AddBookAsync(Book book)
    {
        try
        {
            await _books.InsertOneAsync(book);
            return true;
        }
        catch
        {
            return false;}}
            
    public async Task<bool> DeleteBookAsync(string id)
    {
        var result = await _books.DeleteOneAsync(book => book.Id == id);
        return result.DeletedCount > 0;
    }

    public async Task<Book> GetBookByIdAsync(string id)
    {
        return await _books.Find(book => book.Id == id).FirstOrDefaultAsync();
    }
    public async Task<bool> UpdateBookAsync(string id, Book updatedBook)
{
    try
    {
        var result = await _books.ReplaceOneAsync(book => book.Id == id, updatedBook);
        return result.ModifiedCount > 0;
    }
    catch
    {
        return false;
    }
}
}
