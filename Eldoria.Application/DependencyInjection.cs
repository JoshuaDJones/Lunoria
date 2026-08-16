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
            services.AddScoped<ISpellTypeService, SpellTypeService>();
            services.AddScoped<IEquippableItemService, EquippableItemService>();
            services.AddScoped<IConsumableItemService, ConsumableItemService>();
            services.AddScoped<IJourneyService, JourneyService>();
            services.AddScoped<IPlaythroughService, PlaythroughService>();
            services.AddScoped<ISceneService, SceneService>();
            services.AddScoped<ISceneEventService, SceneEventService>();
            services.AddScoped<ISceneGridService, SceneGridService>();
            services.AddScoped<ISceneChestService, SceneChestService>();
            services.AddScoped<ISceneCharacterService, SceneCharacterService>();
            services.AddScoped<ISceneDialogService, SceneDialogService>();
            services.AddScoped<IDialogPageService, DialogPageService>();
            services.AddScoped<IDialogPageSectionService, DialogPageSectionService>();
            services.AddScoped<IJourneyCharacterService, JourneyCharacterService>();
            services.AddScoped<IJourneyCharacterSpellService, JourneyCharacterSpellService>();
            services.AddScoped<IImagesService, ImagesService>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddScoped<IAzureStorageBlob, AzureStorageBlob>();
            services.AddScoped<ISeriesService, SeriesService>();
            services.AddScoped<IJourneyIntroPageService, JourneyIntroPageService>();

            return services;
        }
    }
}
