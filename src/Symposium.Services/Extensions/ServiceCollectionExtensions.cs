using Microsoft.Extensions.DependencyInjection;

namespace Symposium.Services.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection serviceCollection)
    {
        return serviceCollection.AddScoped<SubmissionService>().AddScoped<SubmissionNotificationService>();
    }
}
