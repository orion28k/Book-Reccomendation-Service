using BookRec.Application.Books.Dtos;
using BookRec.Application.Books.Interface;
using BookRec.Application.Books.Services;
using BookRec.Application.Users.Dtos;
using BookRec.Application.Users.Interface;
using BookRec.Application.Users.Services;
using BookRec.Domain.BookModel;
using BookRec.Infrastructure;

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
                 ?? "Host=localhost;Port=5432;Database=bookrec;Username=postgres;Password=postgres";

builder.Services.AddInfrastructure(connString);

// Application Services
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<IUserService, UserService>();

var app = builder.Build();

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

// Book Endpoints
app.MapGet("/books/{id:guid}", async (IBookService service, Guid id) =>
{
    var book = await service.GetByIdAsync(id);
    // returns book in JSON format
    return book is null ? Results.NotFound() : Results.Ok(book);
});

app.MapGet("/books/by-title/{title}", async (IBookService service, string title) =>
{
    var book = await service.GetByTitleAsync(title);
    return book is null ? Results.NotFound() : Results.Ok(book);
});

app.MapGet("/books/by-author/{author}", async (IBookService service, string author) =>
{
    var books = await service.GetByAuthor(author);
    return books is null ? Results.NotFound() : Results.Ok(books);
});

app.MapGet("/books/by-genre/{genre}", async (IBookService service, string genre) =>
{
    var books = await service.GetByGenre(genre);
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
    return Results.Created($"/books/{id}", new {id});
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
    // returns book in JSON format
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

app.MapPost("/users", async (IUserService service, CreateUserDto dto) =>
{
    var id = await service.AddUser(dto);
    return Results.Created($"/users/{id}", new {id});
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