namespace CocoNats.Sample.Shared.Books;

public class BookDto
{
    public Guid Id { get; set; }

    public string Title { get; set; } = string.Empty;

    public BookType Type { get; set; } = BookType.Unknown;

    public string Author { get; set; } = string.Empty;

    public int Year { get; set; }
}
