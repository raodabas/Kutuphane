using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Linq;
using System.ComponentModel.DataAnnotations;

public class AccountController : Controller
{
    private readonly ApplicationDbContext _context;

    public AccountController(ApplicationDbContext context)
    {
        _context = context;
    }
    [Route("api/account/register")]
    [HttpPost]
    public async Task<IActionResult> Register([FromBody] UserRegistrationModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { Message = "Geçersiz giriş!" });
        }

       
        if (_context.Users.Any(u => u.Email == model.Email))
        {
            return BadRequest(new { Message = "Bu e-posta adresi zaten kayıtlı." });
        }

        // Şifreyi hashle
        var hashedPassword = HashPassword(model.Password);

        // Yeni kullanıcı oluştur
        var user = new User
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            Password = hashedPassword,
            UserType = model.UserType
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Kullanıcı tipine göre öğrenci ya da öğretmen tablosuna ekleme
        if (model.UserType == "Student")
        {
            _context.Students.Add(new Student { UserID = user.UserID });
        }
        else if (model.UserType == "Teacher")
        {
            _context.Teachers.Add(new Teacher { UserID = user.UserID });
        }

        await _context.SaveChangesAsync();

        return Ok(new { Message = "Kayıt başarılı!" });
    }

    [Route("api/account/login")]
    [HttpPost]
    public IActionResult Login([FromBody] LoginModel model)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { Message = "Geçersiz giriş!" });
        }

        
        var user = _context.Users.SingleOrDefault(u => u.Email == model.Email);
        if (user == null)
        {
            return Unauthorized(new { Message = "Geçersiz e-posta adresi veya şifre!" });
        }

        // Şifreyi kontrol et
        if (!VerifyPassword(model.Password, user.Password))
        {
            return Unauthorized(new { Message = "Geçersiz e-posta adresi veya şifre!" });
        }

    
        return Ok(new { Message = "Giriş başarılı!", userType = user.UserType , userId = user.UserID});
    }

    private string HashPassword(string password)
    {
        // Şifre için bir salt oluşturma 
        var salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        // Şifreyi hashle
        var hashedPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: password,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 32));

        // Salt ve hash'i birleştir
        return $"{Convert.ToBase64String(salt)}:{hashedPassword}";
    }

    private bool VerifyPassword(string enteredPassword, string storedPassword)
    {
        var parts = storedPassword.Split(':');
        if (parts.Length != 2)
        {
            return false;
        }

        var salt = Convert.FromBase64String(parts[0]);
        var hashedPassword = parts[1];

        var hashedEnteredPassword = Convert.ToBase64String(KeyDerivation.Pbkdf2(
            password: enteredPassword,
            salt: salt,
            prf: KeyDerivationPrf.HMACSHA256,
            iterationCount: 10000,
            numBytesRequested: 32));

        return hashedEnteredPassword == hashedPassword;
    }
    [Route("api/account/logout")]
[HttpPost]
public IActionResult Logout()
{
    // Oturumu sonlandır (isteğe bağlı olarak kimlik doğrulama verilerini temizle)
    
    return Ok(new { Message = "Oturum kapatıldı" });
}

}

public class UserRegistrationModel
{
    [Required]
    public string FirstName { get; set; } = null!;
    [Required]
    public string LastName { get; set; } = null!;
    [Required]
    public string Email { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
    [Required]
    public string UserType { get; set; } = null!;
}

public class LoginModel
{
    [Required]
    public string Email { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
}