using BookRec.Application.Books.Dtos;
using BookRec.Application.Books.Interface;
using BookRec.Application.Books.Services;
using BookRec.Application.Users.Dtos;
using BookRec.Application.Users.Interface;
using BookRec.Application.Users.Services;
using BookRec.Domain.BookModel;
using BookRec.Infrastructure;
using BookRec.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Library Recommendation Service",
        Version = "v1",
        Description = "API for recommending books to the user based off of books owned.",
    });
});

var connString = builder.Configuration.GetConnectionString("DefaultConnection")
                 ?? "Host=postgres;Port=5432;Database=bookrec;Username=postgres;Password=postgres";

builder.Services.AddInfrastructure(connString);

// Application Services
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IRecommendationService, RecommendationService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    dbContext.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Library Recommendation Service");
    c.RoutePrefix = "swagger";
});

/// Book Endpoints
/// <returns>
/// book in JSON format
/// </returns>
app.MapGet("/books/{id:guid}", async (IBookService service, Guid id) =>
{
    var book = await service.GetByIdAsync(id);
    return book is null ? Results.NotFound() : Results.Ok(book);
});

app.MapGet("/books/by-title/{title}", async (IBookService service, string title) =>
{
    var book = await service.GetByTitleAsync(title);
    return book is null ? Results.NotFound() : Results.Ok(book);
});

app.MapGet("/books/by-author/{author}", async (IBookService service, string author) =>
{
    var books = await service.GetByAuthorAsync(author);
    return books is null ? Results.NotFound() : Results.Ok(books);
});

app.MapGet("/books/by-genre/{genre}", async (IBookService service, string genre) =>
{
    var books = await service.GetByGenreAsync(genre);
    return books is null ? Results.NotFound() : Results.Ok(books);
});

app.MapGet("/books", async (IBookService service) =>
{
    var books = await service.GetAllAsync();
    return books is null ? Results.NotFound() : Results.Ok(books);
});

app.MapPost("/books", async (IBookService service, CreateBookDto dto) =>
{
    var id = await service.AddBook(dto);
    return Results.Created($"/books/{id}", new { id });
});

app.MapPut("/books/{id:guid}", async (IBookService service, UpdateBookDto dto, Guid id) =>
{
    var result = await service.UpdateBook(dto, id);
    return result == Guid.Empty ? Results.NotFound() : Results.NoContent();
});

app.MapDelete("/books/{id:guid}", async (IBookService service, Guid id) =>
{
    await service.DeleteBook(id);
    return Results.NoContent();
});

// User Endpoints
app.MapGet("/users/{id:guid}", async (IUserService service, Guid id) =>
{
    var user = await service.GetByIdAsync(id);
    return user is null ? Results.NotFound() : Results.Ok(user);
});

app.MapGet("/users/by-email/{email}", async (IUserService service, string email) =>
{
    var user = await service.GetByEmailAsync(email);
    return user is null ? Results.NotFound() : Results.Ok(user);
});

app.MapGet("/users/by-username/{username}", async (IUserService service, string username) =>
{
    var user = await service.GetByUserAsync(username);
    return user is null ? Results.NotFound() : Results.Ok(user);
});

app.MapGet("/users/get-user-genres/{id}", async (IUserService service, Guid id) =>
{
    var preferredGenres = await service.GetUserPreferredGenresAsync(id);
    return preferredGenres is null ? Results.NotFound() : Results.Ok(preferredGenres);
});

app.MapGet("/users/{id:guid}/recommendations", async (IRecommendationService recService, Guid id, int limit = 10) =>
{
    var recs = await recService.RecommendBooksForUserAsync(id, limit);
    return recs is null ? Results.NotFound() : Results.Ok(recs);
});

app.MapPost("/users/{id:guid}/read/{bookId:guid}", async (IUserService userService, Guid id, Guid bookId) =>
{
    try
    {
        await userService.MarkBookAsReadAsync(id, bookId);
        return Results.NoContent();
    }
    catch (Exception ex)
    {
        return Results.NotFound(new { error = ex.Message });
    }
});

app.MapGet("/users/{id:guid}/read", async (IUserService userService, IBookService bookService, Guid id) =>
{
    // return list of BookDto for the user's read books
    var readIds = await userService.GetUserReadBookIdsAsync(id);
    var books = new List<BookDto>();
    foreach (var bid in readIds)
    {
        var b = await bookService.GetByIdAsync(bid);
        if (b is not null) books.Add(b);
    }

    return Results.Ok(books);
});

app.MapDelete("/users/{id:guid}/read/{bookId:guid}", async (IUserService userService, Guid id, Guid bookId) =>
{
    try
    {
        await userService.UnmarkBookAsReadAsync(id, bookId);
        return Results.NoContent();
    }
    catch (Exception ex)
    {
        return Results.NotFound(new { error = ex.Message });
    }
});

app.MapPost("/users", async (IUserService service, CreateUserDto dto) =>
{
    var id = await service.AddUser(dto);
    return Results.Created($"/users/{id}", new { id });
});

app.MapPut("/users/{id:guid}", async (IUserService service, UpdateUserDto dto, Guid id) =>
{
    var result = await service.UpdateUser(dto, id);
    return result == Guid.Empty ? Results.NotFound() : Results.NoContent();
});

app.MapDelete("/users/{id:guid}", async (IUserService service, Guid id) =>
{
    await service.DeleteUser(id);
    return Results.NoContent();
});


app.Run();
