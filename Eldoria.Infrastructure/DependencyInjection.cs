using Eldoria.Core.Interfaces;
using Eldoria.Infrastructure.Db;
using Eldoria.Infrastructure.Db.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Eldoria.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        string? connectionString)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ISceneRepository, SceneRepository>();
            services.AddScoped<ISceneDialogRepository, SceneDialogRepository>();
            services.AddScoped<IJourneyRepository, JourneyRepository>();
            services.AddScoped<IJourneyCharacterRepository, JourneyCharacterRepository>();
            services.AddScoped<ICharacterSpellRepository, CharacterSpellRepository>();
            services.AddScoped<ICharacterRepository, CharacterRepository>();
            services.AddScoped<ISpellRepository, SpellRepository>();
            services.AddScoped<IJourneyCharacterSpellRepository, JourneyCharacterSpellRepository>();
            services.AddScoped<IJourneyPlaythroughRepository, JourneyPlaythroughRepository>();
            services.AddScoped<IJourneyPlaythroughCharacterRepository, JourneyPlaythroughCharacterRepository>();
            services.AddScoped<IScenePlaythroughRepository, ScenePlaythroughRepository>();
            services.AddScoped<ISceneCharacterRepository, SceneCharacterRepository>();
            services.AddScoped<ISceneChestRepository, SceneChestRepository>();
            services.AddScoped<ISceneEventRepository, SceneEventRepository>();
            services.AddScoped<ISeriesRepository, SeriesRepository>();
            services.AddScoped<ISpellTypeRepository, SpellTypeRepository>();
            services.AddScoped<IEquippableItemRepository, EquippableItemRepository>();
            services.AddScoped<IItemRepository, ItemRepository>();
            services.AddScoped<IOwnershipRepository, OwnershipRepository>();

            return services;
        }
    }
}
