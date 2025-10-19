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
            services.AddScoped<IJourneyRepository, JourneyRepository>();
            services.AddScoped<IJourneyCharacterRepository, JourneyCharacterRepository>();
            services.AddScoped<ICharacterSpellRepository, CharacterSpellRepository>();
            services.AddScoped<ICharacterRepository, CharacterRepository>();
            services.AddScoped<ISpellRepository, SpellRepository>();

            return services;
        }
    }
}
