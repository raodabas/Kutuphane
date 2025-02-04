using Microsoft.AspNetCore.Mvc;
using proje.Models;

[ApiController]
[Route("api/[controller]")]
public class UserLibraryController : ControllerBase
{
    private readonly UserLibraryService _userLibraryService;

    public UserLibraryController(UserLibraryService userLibraryService)
    {
        _userLibraryService = userLibraryService;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUserLibrary(int userId)
    {
        try
        {
            var books = await _userLibraryService.GetUserLibraryBooksAsync(userId);

            if (books == null || !books.Any())
            {
                // Boş kitaplık döndür
                return Ok(new List<Book>());
            }

            return Ok(books);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"İç sunucu hatası: {ex.Message}");
        }
    }
public class BookIdRequest
{
    public string? BookId { get; set; }
}
  
[HttpPost("{userId}/addBook")]
public async Task<IActionResult> AddBookToLibrary(int userId, [FromBody] BookIdRequest request)
{
    if (string.IsNullOrEmpty(request.BookId))
    {
        return BadRequest(new { Message = "BookId is required." });
    }

    try
    {
        Console.WriteLine($"Kitap ekleme isteği alındı: UserId: {userId}, BookId: {request.BookId}"); 

        var result = await _userLibraryService.AddBookToLibraryAsync(userId, request.BookId); 

        if (result)
        {
            Console.WriteLine("Kitap başarıyla eklendi.");
            return Ok("Kitap başarıyla eklendi.");
        }
        else
        {
            Console.WriteLine("Kitap eklenirken bir hata oluştu.");
            return StatusCode(500, "Kitap eklenirken bir hata oluştu.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"İç sunucu hatası: {ex.Message}");
        return StatusCode(500, $"İç sunucu hatası: {ex.Message}");
    }
}
[HttpDelete("{userId}/removeBook/{bookId}")]
    public async Task<IActionResult> RemoveBookFromLibrary(string userId, string bookId)
    {
        var result = await _userLibraryService.RemoveBookFromLibraryAsync(userId, bookId);
        if (result)
        {
            return Ok("Kitap başarıyla çıkarıldı.");
        }
        return StatusCode(500, "Kitap çıkarılırken bir hata oluştu.");
    }
}




