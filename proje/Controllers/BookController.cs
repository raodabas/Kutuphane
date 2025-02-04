using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using proje.Models;

[Route("api/[controller]")]
[ApiController]
public class BooksController : ControllerBase
{
    private readonly IMongoCollection<Book> _booksCollection;

    public BooksController(IMongoDatabase database)
    {
        _booksCollection = database.GetCollection<Book>("Books");
    }

    [HttpPost]
    public async Task<ActionResult<Book>> Create(Book book)
    {
        await _booksCollection.InsertOneAsync(book);
        return CreatedAtAction(nameof(GetById), new { id = book.Id }, book);
    }

    [HttpGet]
    public async Task<ActionResult<List<Book>>> Get()
    {
        var books = await _booksCollection.Find(book => true).ToListAsync();
        return Ok(books);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Book>> GetById(string id)
    {
        var book = await _booksCollection.Find(b => b.Id == id).FirstOrDefaultAsync();
        if (book == null) return NotFound();
        return Ok(book);
    }

    [HttpDelete("{id}")]
public async Task<IActionResult> DeleteBook(string id)
{
    
    if (!ObjectId.TryParse(id, out ObjectId objectId))
    {
        return BadRequest(new { Message = "Invalid ID format" });
    }

    
    var deleteResult = await _booksCollection.DeleteOneAsync(book => book.Id != null && book.Id == objectId.ToString());

    if (deleteResult.DeletedCount == 0)
    {
        return NotFound(new { Message = "Book not found" });
    }

    return NoContent();
}
[HttpPut("{id}")]
public async Task<IActionResult> UpdateBook(string id, [FromBody] Book updatedBook)
{
    if (!ObjectId.TryParse(id, out ObjectId objectId))
    {
        return BadRequest(new { Message = "Invalid ID format" });
    }

    var book = await _booksCollection.Find(b => b.Id == id).FirstOrDefaultAsync();
    if (book == null)
    {
        return NotFound(new { Message = "Book not found" });
    }

    updatedBook.Id = id; // ID'yi koruyalım
    var result = await _booksCollection.ReplaceOneAsync(b => b.Id == id, updatedBook);

    if (result.MatchedCount == 0)
    {
        return NotFound(new { Message = "Book not updated" });
    }

    return Ok(updatedBook);
}

 // Kitap türlerini almak için endpoint
        [HttpGet("genres")]
        public async Task<ActionResult<IEnumerable<string>>> GetGenres()
        {
            try
            {
                var genres = await _booksCollection.Distinct<string>("Genre", Builders<Book>.Filter.Empty).ToListAsync();
                return Ok(genres);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Türler alınırken hata oluştu: " + ex.Message);
                return StatusCode(500, "Sunucu hatası");
            }
        }



}
