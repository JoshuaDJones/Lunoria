using Eldoria.Application.Common;
using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;

namespace Eldoria.Application.Services
{
    public class DialogPageSectionService : IDialogPageSectionService
    {
        private readonly IRepository<DialogPage> _dialogPageRepository;
        private readonly IRepository<DialogPageSection> _dialogPageSectionRepository;
        private readonly IRepository<Character> _characterRepository;

        public DialogPageSectionService(IRepository<DialogPage> dialogPageRepository, IRepository<DialogPageSection> dialogPageSectionRepository, ICharacterRepository characterRepository)
        {
            _dialogPageRepository = dialogPageRepository;
            _dialogPageSectionRepository = dialogPageSectionRepository;
            _characterRepository = characterRepository;
        }

        public async Task<Result> CreateDialogPageSectionAsync(int dialogPageId, int? characterId, int orderNum, string readingText, bool isNarrator, CancellationToken ct)
        {
            var dialogPage = await _dialogPageRepository.GetByIdAsync(dialogPageId, ct);

            if (dialogPage is null)
                return Result.Fail(new Error("DialogPage.NotFound", "Dialog page does not exist."));

            var character = characterId is not null ? await _characterRepository.GetByIdAsync((int)characterId, ct) : null;

            if (character is null && isNarrator is false)
                return Result.Fail(new Error("Character.NotFound", "Character does not exist."));

            var dialogPageSection = new DialogPageSection
            {
                DialogPageId = dialogPageId,
                CharacterId = characterId,
                OrderNum = orderNum,
                ReadingText = readingText,
                IsNarrator = isNarrator,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
            };

            await _dialogPageSectionRepository.AddAsync(dialogPageSection, ct);
            await _dialogPageSectionRepository.SaveChangesAsync(ct);

            return Result.Ok();
        }

        public async Task<Result> DeleteDialogPageSectionAsync(int dialogPageSectionId, CancellationToken ct)
        {
            var dialogPageSection = await _dialogPageSectionRepository.GetByIdAsync(dialogPageSectionId, ct);

            if (dialogPageSection is null)
                return Result.Fail(new Error("DialogPageSection.NotFound", "Dialog page section not found."));

            _dialogPageSectionRepository.Remove(dialogPageSection);
            await _dialogPageSectionRepository.SaveChangesAsync(ct);

            return Result.Ok();
        }

        public async Task<Result> EditDialogPageSectionAsync(int dialogPageSectionId, int? characterId, int? orderNum, string? readingText, bool? isNarrator, CancellationToken ct)
        {
            var dialogPageSection = await _dialogPageSectionRepository.GetByIdAsync(dialogPageSectionId, ct);

            if (dialogPageSection is null)
                return Result.Fail(new Error("DialogPageSection.NotFound", "Dialog page section does not exist."));

            dialogPageSection.CharacterId = characterId;

            if (orderNum is not null)
                dialogPageSection.OrderNum = (int)orderNum;

            if (readingText is not null)    
                dialogPageSection.ReadingText = readingText;

            if (isNarrator is not null)
                dialogPageSection.IsNarrator = (bool)isNarrator;

            await _dialogPageSectionRepository.SaveChangesAsync(ct);
            return Result.Ok();
        }
    }
}
