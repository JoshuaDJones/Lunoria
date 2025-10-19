using Eldoria.Application.Common;
using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;

namespace Eldoria.Application.Services
{
    public class SceneCharacterService : ISceneCharacterService
    {
        private readonly IRepository<SceneCharacter> _sceneCharacterRepository;
        private readonly IRepository<Scene> _sceneRepository;
        private readonly ICharacterRepository _characterRepository;

        public SceneCharacterService(IRepository<SceneCharacter> sceneCharacterRepository, IRepository<Scene> sceneRepository, ICharacterRepository characterRepository)
        {
            _sceneCharacterRepository = sceneCharacterRepository;
            _sceneRepository = sceneRepository;
            _characterRepository = characterRepository;
        }

        public async Task<Result> AddSceneCharacterAsync(int sceneId, int characterId, CancellationToken ct)
        {
            var scene = await _sceneRepository.GetByIdAsync(sceneId, ct);

            if (scene is null)
                return Result.Fail(new Error("Scene.NotFound", "Scene does not exist"));

            var character = await _characterRepository.GetByIdAsync(characterId, ct);

            if (character is null)
                return Result.Fail(new Error("Character.NotFound", "Character does not exist"));

            var sceneCharacter = new SceneCharacter
            {
                CurrentHp = character.MaxHp,
                CurrentMp = character.MaxMp,
                IsDown = false,
                IsAlternateForm = false,
                SceneId = sceneId,
                CharacterId = characterId,
            };

            await _sceneCharacterRepository.AddAsync(sceneCharacter, ct);
            await _sceneCharacterRepository.SaveChangesAsync(ct);

            return Result.Ok();
        }

        public async Task<Result> AdjustCharacterHpMpAsync(int sceneCharacterId, int newHp, int newMp, CancellationToken ct)
        {
            var sceneCharacter = await _sceneCharacterRepository.GetByIdAsync(sceneCharacterId, ct);

            if (sceneCharacter is null)
                return Result.Fail(new Error("SceneCharacter.NotFound", "Scene Character does not exist."));

            sceneCharacter.CurrentHp = newHp;
            sceneCharacter.CurrentMp = newMp;

            await _sceneCharacterRepository.SaveChangesAsync(ct);

            return Result.Ok();
        }

        public async Task<Result> DeleteSceneCharacterAsync(int sceneCharacterId, CancellationToken ct)
        {
            var sceneCharacter = await _sceneCharacterRepository.GetByIdAsync(sceneCharacterId, ct);

            if (sceneCharacter is null)
                return Result.Fail(new Error("SceneCharacter.NotFound", "Scene Character does not exist."));

            _sceneCharacterRepository.Remove(sceneCharacter);
            await _sceneCharacterRepository.SaveChangesAsync(ct);

            return Result.Ok(); 
        }
    }
}