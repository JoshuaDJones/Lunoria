using Eldoria.Application.Services;
using Eldoria.Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Eldoria.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(
        this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICharacterService, CharacterService>();
            services.AddScoped<ICharacterSpellService, CharacterSpellService>();
            services.AddScoped<ISpellService, SpellService>();
            services.AddScoped<IItemService, ItemService>();
            services.AddScoped<IJourneyService, JourneyService>();
            services.AddScoped<ISceneService, SceneService>();
            services.AddScoped<ISceneCharacterService, SceneCharacterService>();
            services.AddScoped<ISceneCharacterItemService, SceneCharacterItemService>();
            services.AddScoped<IJourneyCharacterService, JourneyCharacterService>();
            services.AddScoped<IJourneyCharacterItemService, JourneyCharacterItemService>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddScoped<IAzureStorageBlob, AzureStorageBlob>();

            return services;
        }
    }
}
