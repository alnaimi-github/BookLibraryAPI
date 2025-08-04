using BookLibraryAPI.Application.Common.DTOs.Books;
using BookLibraryAPI.Application.Common.Mappers.Books;
using BookLibraryAPI.Core.Domain.Books;
using BookLibraryAPI.Core.Domain.Common;
using BookLibraryAPI.Core.Domain.Interfaces.Ports.Email;
using BookLibraryAPI.Core.Domain.Interfaces.Repositories;
using MediatR;

namespace BookLibraryAPI.Application.Features.Books.Commands.CreateBook;

public class CreateBookCommandHandler(
    IBookRepository bookRepository,
    IEmailNotificationPort emailNotificationPort)
    : IRequestHandler<CreateBookCommand, Result<BookDto>>
{
    public async Task<Result<BookDto>> Handle(CreateBookCommand request, CancellationToken cancellationToken)
    {
        var book = Book.Create(request.Title, request.Author, request.Year);

        var createdBook = await bookRepository.AddAsync(book, cancellationToken);
    
        if (createdBook is null)
        {
            return Result<BookDto>.Failure(Error.NullValue.Name);
        }
        
        await emailNotificationPort.SendWelcomeEmailAsync(
            book.Title.Value,
            book.Author.Value, 
            cancellationToken);
       
        var dto = createdBook.ToDto();
        
        
        return Result<BookDto>.Success(dto);
    }
}