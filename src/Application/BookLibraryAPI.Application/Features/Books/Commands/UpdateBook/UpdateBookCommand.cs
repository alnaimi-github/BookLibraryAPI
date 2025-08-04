using BookLibraryAPI.Application.Common.DTOs.Books;
using BookLibraryAPI.Core.Domain.Common;
using MediatR;

namespace BookLibraryAPI.Application.Features.Books.Commands.UpdateBook;

public sealed record UpdateBookCommand(int Id, string Title, string Author, int Year) : IRequest<Result<bool>>;

