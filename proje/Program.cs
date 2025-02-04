using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// MongoDB bağlantısı
var mongoClient = new MongoClient("mongodb://localhost:27017");
var mongoDatabase = mongoClient.GetDatabase("LibraryDB");
builder.Services.AddSingleton<IMongoDatabase>(mongoDatabase);

builder.Services.AddSingleton<BookService>();
builder.Services.AddSingleton<UserLibraryService>();
builder.Services.AddSingleton<CommentService>();
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Static dosyalar ve gerekli middleware'ler
//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// MVC ve API için gerekli rotaların haritalanması
app.MapDefaultControllerRoute(); // MVC controller'larını haritalar
app.MapControllers(); // API controller'larını haritalar

app.Run();