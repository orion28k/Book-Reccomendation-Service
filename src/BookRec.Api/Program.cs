using BookRec.Infrastructure.Data;
using BookRec.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Swagger/OpenAPI documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Library Reccommendation Service",
        Version = "v1",
        Description = "API for reccomending books to the user based off of books owned.",
    });
}
);

// Configure Postgres DbContext and DI for repositories
var connString = builder.Configuration.GetConnectionString("Default")
                  ?? builder.Configuration["ConnectionStrings:Default"]
                  ?? "Host=localhost;Port=5432;Database=bookrec;Username=postgres;Password=postgres";

builder.Services.AddDbContext<BookRecDbContext>(options =>
    options.UseNpgsql(connString));

builder.Services.AddScoped<IBookRepository, EfBookRepository>();
builder.Services.AddScoped<IUserRepository, EfUserRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

// Minimal Book endpoints (using repository directly for now)
app.MapPost("/books", async (IBookRepository repo, Book book) =>
{
    if (book.Id == Guid.Empty) book.Id = Guid.NewGuid();
    await repo.AddBook(book);
    return Results.Created($"/books/{book.Id}", new { id = book.Id });
});

app.MapGet("/books", async (IBookRepository repo) =>
{
    var books = await repo.GetAllAsync();
    return Results.Ok(books);
});

app.MapGet("/books/{id:guid}", async (IBookRepository repo, Guid id) =>
{
    var book = await repo.GetByIdAsync(id);
    return book is null ? Results.NotFound() : Results.Ok(book);
});

app.MapGet("/books/by-title/{title}", async (IBookRepository repo, string title) =>
{
    var book = await repo.GetByTitleAsync(title);
    return book is null ? Results.NotFound() : Results.Ok(book);
});

app.MapGet("/books/by-author/{author}", async (IBookRepository repo, string author) =>
{
    var books = await repo.GetByAuthor(author);
    return Results.Ok(books);
});

app.MapPut("/books/{id:guid}", async (IBookRepository repo, Guid id, Book book) =>
{
    var existing = await repo.GetByIdAsync(id);
    if (existing is null) return Results.NotFound();
    
    book.Id = id; // Ensure ID matches route
    await repo.UpdateBook(book);
    return Results.NoContent();
});

app.MapDelete("/books/{id:guid}", async (IBookRepository repo, Guid id) =>
{
    var existing = await repo.GetByIdAsync(id);
    if (existing is null) return Results.NotFound();
    await repo.DeleteBook(existing);
    return Results.NoContent();
});

// Enable Swagger API
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Library Reccommendation Service");
    c.RoutePrefix = "swagger";
}
);

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
