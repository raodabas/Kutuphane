using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Teacher> Teachers { get; set; }
    public DbSet<Student> Students { get; set; }
}
public class User
{
    public int UserID { get; set; }
    [Required]
    public string FirstName {get; set;} = null!;
    [Required]
    public string LastName {get; set;} = null!;

    [Required]
    public string Email { get; set; } = null!;
    [Required]
    public string Password { get; set; } = null!;
    [Required]
    public string UserType { get; set; } = null!;}
public class Teacher
{
    public int TeacherID { get; set; }
    public int UserID { get; set; }}
public class Student
{
    public int StudentID { get; set; }
    public int UserID { get; set; }}
