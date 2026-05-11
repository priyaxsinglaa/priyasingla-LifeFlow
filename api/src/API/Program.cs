using Microsoft.EntityFrameworkCore;
using API.Repositories;
using API.Services;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();


builder.Services.AddDbContext<API.Models.LifeFlowDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"))
        );
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<AuthService>();

var app = builder.Build();



app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();


app.MapControllers();
app.MapGet("/", () => "LifeFlow API is running!");

app.Run();
