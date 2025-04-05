using Application.Books.Books.Domain;

using CocoNats.Sample.Shared.Books;

namespace Application.Books.Books.Infrastructure.Persistence;

public class InMemoryBookRepository : IBookRepository
{
    private readonly List<Book> _books =
    [
        Book.Create("1984", BookType.Dystopian, "George Orwell", 1949),
        Book.Create("To Kill a Mockingbird", BookType.Social, "Harper Lee", 1960),
        Book.Create("The Great Gatsby", BookType.Classic, "F. Scott Fitzgerald", 1925),
        Book.Create("Pride and Prejudice", BookType.Romance, "Jane Austen", 1813),
        Book.Create("The Hobbit", BookType.Fantasy, "J.R.R. Tolkien", 1937),
        Book.Create("Brave New World", BookType.Dystopian, "Aldous Huxley", 1932),
        Book.Create("Jane Eyre", BookType.Romance, "Charlotte Brontë", 1847),
        Book.Create("The Catcher in the Rye", BookType.Social, "J.D. Salinger", 1951),
        Book.Create("Moby-Dick", BookType.Classic, "Herman Melville", 1851),
        Book.Create("Les Misérables", BookType.Historical, "Victor Hugo", 1862),
        Book.Create("Harry Potter and the Sorcerer's Stone", BookType.Fantasy, "J.K. Rowling", 1997),
        Book.Create("The Da Vinci Code", BookType.Historical, "Dan Brown", 2003),
        Book.Create("The Hunger Games", BookType.Dystopian, "Suzanne Collins", 2008),
        Book.Create("The Fault in Our Stars", BookType.Romance, "John Green", 2012),
        Book.Create("A Game of Thrones", BookType.Fantasy, "George R.R. Martin", 1996),
    ];

    public Task<Book> CreateBookAsync(Book book)
    {
        _books.Add(book);
        return Task.FromResult(book);
    }

    public Task<Book?> GetBookAsync(Guid id)
    {
        return Task.FromResult(_books.FirstOrDefault(b => b.Id == id));
    }

    public Task<List<Book>> GetBooksAsync()
    {
        return Task.FromResult(_books.ToList());
    }

    public Task<Book?> UpdateBooksAsync(Book book)
    {
        var existingBook = _books.FirstOrDefault(b => b.Id == book.Id);
        if (existingBook == null)
        {
            return Task.FromResult<Book?>(null);
        }

        existingBook.Title = book.Title;
        existingBook.Author = book.Author;
        existingBook.Year = book.Year;

        return Task.FromResult(existingBook)!;
    }

    public Task<bool> DeleteBooksAsync(Guid id)
    {
        var book = _books.FirstOrDefault(b => b.Id == id);
        if (book == null)
        {
            return Task.FromResult(false);
        }

        _books.Remove(book);
        return Task.FromResult(true);
    }
}