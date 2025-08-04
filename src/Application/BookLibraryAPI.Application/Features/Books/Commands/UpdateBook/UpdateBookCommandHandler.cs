using BookLibraryAPI.Core.Domain.Common;
using BookLibraryAPI.Core.Domain.Interfaces.Repositories;
using BookLibraryAPI.Core.Domain.ValueObjects;
using MediatR;

namespace BookLibraryAPI.Application.Features.Books.Commands.UpdateBook;

public class UpdateBookCommandHandler(IBookRepository bookRepository) : IRequestHandler<UpdateBookCommand, Result<bool>>
{
    public async Task<Result<bool>> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        var book = await bookRepository.GetByIdAsync(request.Id, cancellationToken);
        if (book is null)
        {
            return Result<bool>.Failure($"Book with ID {request.Id} not found.");
        }

        book.Update(request.Title, request.Author, request.Year);
        await bookRepository.UpdateAsync(book, cancellationToken);
        return Result<bool>.Success(true);
    }
}

