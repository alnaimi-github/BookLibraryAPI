using BookLibraryAPI.Application.Common.DTOs.Users;
using BookLibraryAPI.Core.Domain.Common;
using MediatR;

namespace BookLibraryAPI.Application.Features.Users.Commands.Register;

public sealed record RegisterCommand(
    string Username,
    string Password,
    string Role) : IRequest<Result<AuthenticationResultDto>>;