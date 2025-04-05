namespace CocoNats.Sample.Shared.Books;

public class CreateBookRequest
{
    public required string Title { get; set; }

    public BookType Type { get; set; } = BookType.Unknown;

    public required string Author { get; set; }

    public required int Year { get; set; }
}
