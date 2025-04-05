using CocoNats.Sample.Shared.Books;

namespace Application.Books.Books.Domain;

public class Book
{
    internal Book(Guid id, string title, BookType type, string author, int year)
    {
        Id = id;
        Title = title;
        Type = type;
        Author = author;
        Year = year;
    }

    private Book()
    {
    }

    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public BookType Type { get; set; } = BookType.Unknown;

    public string Author { get; set; } = string.Empty;

    public int Year { get; set; }

    public static Book Create(string title, BookType type, string author, int year)
    {
        return new Book(Guid.NewGuid(), title, type, author, year);
    }
}