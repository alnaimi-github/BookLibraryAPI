using BookLibraryAPI.Application.Common.DTOs.Books;
using BookLibraryAPI.Core.Domain.Common;
using MediatR;

namespace BookLibraryAPI.Application.Features.Books.Queries.GetBookById;

public sealed record GetBookByIdQuery(int Id) : IRequest<Result<BookDto>>;

