using BookLibraryAPI.Application.Common.DTOs.Books;
using BookLibraryAPI.Core.Domain.Common;
using MediatR;

namespace BookLibraryAPI.Application.Features.Books.Queries.GetAllBooks;

public sealed record GetAllBooksQuery : IRequest<IEnumerable<BookDto>>, IRequest<Result<IEnumerable<BookDto>>>;