using BookLibraryAPI.Application.Common.Behaviors;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace BookLibraryAPI.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
         services.AddMediatR(config =>
               {
                   config.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
                   
                   config.AddOpenBehavior(typeof(RequestLoggingPipelineBehavior<,>));
                   config.AddOpenBehavior(typeof(ValidationPipelineBehavior<,>));
                   config.AddOpenBehavior(typeof(ExceptionHandlingPipelineBehavior<,>));
               });


        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

        return services;
    }
}