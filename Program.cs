using BlogPostApi.Data;
using BlogPostApi.EndPoints;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add database connection
builder.Services.AddDbContext<BlogDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

var app = builder.Build();

app.MapUserEndPoints();
app.MapPostEndPoints();
// app.UseAuthorization();
// app.MapControllers();

app.Run();
