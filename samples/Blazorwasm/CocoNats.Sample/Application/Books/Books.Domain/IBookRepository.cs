namespace Application.Books.Books.Domain;

public interface IBookRepository
{
    Task<Book> CreateBookAsync(Book book);

    Task<List<Book>> GetBooksAsync();

    Task<Book?> GetBookAsync(Guid id);

    Task<Book?> UpdateBooksAsync(Book book);

    Task<bool> DeleteBooksAsync(Guid id);
}