using Microsoft.AspNetCore.Mvc;
using proje.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

[ApiController]
[Route("api/[controller]")]
public class CommentsController : ControllerBase
{
    private readonly CommentService _commentService;
    private readonly BookService _bookService; 
   

    public CommentsController(CommentService commentService, BookService bookService)
    {
        _commentService = commentService;
        _bookService = bookService;
       
    }

  [HttpPost]
    public async Task<IActionResult> AddComment([FromBody] Comments comment)
    {
        if (comment == null || string.IsNullOrEmpty(comment.BookId))
        {
            return BadRequest("Yorum verisi veya kitap ID'si eksik.");
        }

        // Yorumun doğru kitaba eklenmesi
        var existingBook = await _bookService.GetBookByIdAsync(comment.BookId);
        if (existingBook == null)
        {
            return NotFound("Kitap bulunamadı.");
        }

        await _commentService.AddCommentAsync(comment);
        return Ok(new { comment.Comment, comment.BookId, comment.UserId, comment.CreatedAt });
    }

[HttpGet]
public async Task<ActionResult<List<Comments>>> GetComments([FromQuery] string bookId)
{
    if (string.IsNullOrEmpty(bookId))
    {
        return BadRequest("BookId eksik!");
    }

    // Kitap bulunamadığında kontrol etme
    var existingBook = await _bookService.GetBookByIdAsync(bookId);
    if (existingBook == null)
    {
        return NotFound("Kitap bulunamadı.");
    }

    var comments = await _commentService.GetCommentsByBookIdAsync(bookId);

    // Kullanıcı türünü ve UserId'yi kontrol et
    var userType = HttpContext.Items["UserType"]?.ToString();
    var userId = HttpContext.Items["UserId"]?.ToString();

    // Kullanıcı türü öğrenci ise ve UserId mevcutsa sadece kendi yorumlarını göster
    if (userType == "Student" && userId != null)
    {
        comments = comments.Where(c => c.UserId == userId).ToList();
    }

    return Ok(comments);
}

}
