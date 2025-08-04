using BookLibraryAPI.Application.Common.DTOs.Users;
using BookLibraryAPI.Core.Domain.Common;
using MediatR;

namespace BookLibraryAPI.Application.Features.Users.Commands.Login;

public sealed record  LoginCommand(string Username, string Password) : IRequest<Result<AuthenticationResultDto>>;