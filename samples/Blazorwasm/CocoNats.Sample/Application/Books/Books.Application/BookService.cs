using Application.Books.Books.Application;
using Application.Books.Books.Domain;

using CocoNats.Core.Services;

using CocoNats.Sample.Shared.Books;

namespace Application.Books.Books.Application;

public static class BookEntityExtensions
{
    public static BookDto ToDto(this Book book)
    {
        return new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Year = book.Year,
        };
    }

    public static Book ToEntity(this BookDto book)
    {
        return new Book(book.Id, book.Title, book.Type, book.Author, book.Year);
    }
}

public class BookService(IBookRepository bookRepository) : NatsService
{
    private readonly IBookRepository _repository = bookRepository;

    public async Task<CreateBookResponse> CreateBook(CreateBookRequest request)
    {
        var book = Book.Create(request.Title, request.Type, request.Author, request.Year);
        var result = await _repository.CreateBookAsync(book) ?? throw new Exception();

        return new CreateBookResponse { Book = result.ToDto() };
    }

    public async Task<GetBookResponse> GetBook(GetBookRequest request)
    {
        var result = await _repository.GetBookAsync(request.Id) ?? throw new Exception();
        return new GetBookResponse { Book = result.ToDto() };
    }

    public async Task<GetBooksResponse> GetBooks()
    {
        var books = await _repository.GetBooksAsync();
        return new GetBooksResponse { Books = books.ConvertAll(book => book.ToDto()) };
    }

    public async Task<UpdateBookResponse> UpdateBook(UpdateBookRequest request)
    {
        var book = await _repository.UpdateBooksAsync(request.Book.ToEntity()) ?? throw new Exception();
        return new UpdateBookResponse { Book = book.ToDto() };
    }

    public async Task<DeleteBookResponse> DeleteBook(DeleteBookRequest request)
    {
        var success = await _repository.DeleteBooksAsync(request.Id);
        return new DeleteBookResponse { Success = success };
    }
}
