using CoreService.Contact.DTOs;
using CoreService.Contact.Infrastructure.Repositories;
using CoreService.Contact.Services;

namespace CoreService.Contact;

public static class ContactFeatureExtensions
{
    public static IServiceCollection AddContactFeature(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<ContactUploadOptions>(configuration.GetSection(ContactUploadOptions.SectionName));
        services.AddScoped<IContactRepository, ContactRepository>();
        services.AddScoped<IContactService, ContactService>();
        return services;
    }
}
