using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using booksApiDb.Models;

namespace booksApiDb
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddAuthorization();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("NextJsPolicy", policy =>
                {
                    policy.WithOrigins("http://localhost:3000")
                    .AllowAnyMethod()
                    .AllowAnyHeader();
                });
            });

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            builder.Services.AddDbContext<BooksContext>();

            var app = builder.Build();

            app.UseCors("NextJsPolicy");

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.MapScalarApiReference();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            //web pages v̌̌̌̌   

            app.MapGet("/", () => "hello world");

            app.MapGet("/getGenres", async (BooksContext db) =>
            {
                var genres = await db.Genres.ToListAsync();
                return Results.Ok(genres);
            });

            app.MapGet("/getBooksByGenre/{genreId}", async (int genreId, BooksContext db) =>

            {

                var books = await db.Books

                    .Where(b => b.GenreId == genreId)

                    .ToListAsync();


                return Results.Ok(books);

            });

            app.MapPost("/createBook", async (BooksContext db, Book book) => {

                db.Books.Add(book);
                await db.SaveChangesAsync();
                return Results.Created($"/createBook/{book.Id}", book);

            });

            app.MapDelete("/deleteBook/{id}", async (BooksContext db, int id) =>
            {
                var book = await db.Books.FindAsync(id);

                if (book is null)
                {
                    return Results.NotFound($"Book with ID {id} was not found.");
                }

                db.Books.Remove(book);
                await db.SaveChangesAsync();

                return Results.NoContent();
            });

            app.MapPut("/updateBook/{id}", async (BooksContext db, int id, Book updatedBook) =>
            {
                // Find the existing book in the database
                var existingBook = await db.Books.FindAsync(id);

                // Return 404 if the book doesn't exist
                if (existingBook is null)
                {
                    return Results.NotFound($"Book with ID {id} was not found.");
                }

                // Update the scalar properties
                existingBook.Title = updatedBook.Title;
                existingBook.Description = updatedBook.Description;
                existingBook.GenreId = updatedBook.GenreId;

                //  Save changes to SQLite
                await db.SaveChangesAsync();

                // Return 204 No Content (or Results.Ok(existingBook))
                return Results.NoContent();
            });


            app.Run();

          
        }
    }
}
