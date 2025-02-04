using MongoDB.Driver;
using proje.Models;
using MongoDB.Bson;

public class UserLibraryService
{
    private readonly IMongoCollection<UserLibrary> _userLibraries;
    private readonly IMongoCollection<Book> _books;

    public UserLibraryService(IMongoDatabase database)
    {
        _userLibraries = database.GetCollection<UserLibrary>("UserLibraries");
        _books = database.GetCollection<Book>("Books");
    }

    // Kullanıcının kitaplarını getirir
    public async Task<List<Book>> GetUserLibraryBooksAsync(int userId)
    {
        var userIdString = userId.ToString(); 
        var filter = Builders<UserLibrary>.Filter.Eq(ul => ul.UserId, userIdString);

        var userLibraries = await _userLibraries.Find(filter).ToListAsync();

        if (!userLibraries.Any())
        {
            // Eğer bu kullanıcı için kitap yoksa, boş liste döndürüyoruz
            return new List<Book>();
        }

        var bookIds = userLibraries.Select(ul => ul.BookId).ToList();
        var booksFilter = Builders<Book>.Filter.In(b => b.Id, bookIds);

        // Book koleksiyonundan kitap bilgilerini çekiyoruz
        return await _books.Find(booksFilter).ToListAsync();
    }

   public async Task<bool> AddBookToLibraryAsync(int userId, string bookIdlocal)
{
    var userIdString = userId.ToString();
    
    // bookId'yi ObjectId formatına çevirme
    var objectId = ObjectId.TryParse(bookIdlocal, out var validObjectId) ? validObjectId : ObjectId.Empty;

    if (objectId == ObjectId.Empty)
    {
        Console.WriteLine("Geçersiz BookId!");
        return false; // BookId geçersizse işlemi durdur
    }

    var newUserLibraryEntry = new UserLibrary
    {
        UserId = userIdString,
        BookId = objectId.ToString() // ObjectId olarak saklanacak
    };

    try
    {
        Console.WriteLine($"Kitap ekleniyor: UserId: {userIdString}, BookId: {objectId}");
        await _userLibraries.InsertOneAsync(newUserLibraryEntry);
        return true;
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Kitap eklenirken hata: {ex.Message}");
        return false;
    }
}
 public async Task<bool> RemoveBookFromLibraryAsync(string userId, string bookId)
    {
        var filter = Builders<UserLibrary>.Filter.And(
            Builders<UserLibrary>.Filter.Eq(ul => ul.UserId, userId),
            Builders<UserLibrary>.Filter.Eq(ul => ul.BookId, bookId)
        );

        try
        {
            var result = await _userLibraries.DeleteOneAsync(filter);
            return result.DeletedCount > 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Kitap çıkarılırken hata: {ex.Message}");
            return false;
        }
    }
}
