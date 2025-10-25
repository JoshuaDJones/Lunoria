using Eldoria.Application.Common;
using Eldoria.Core.Entities;
using Eldoria.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Eldoria.Application.Services
{
    public class DialogPageService : IDialogPageService
    {
        private readonly IAzureStorageBlob _azureStorageBlob;
        private readonly IRepository<DialogPage> _dialogPageRepository;
        private readonly IRepository<SceneDialog> _sceneDialogRepository;

        public DialogPageService(IRepository<DialogPage> dialogRepository, IRepository<SceneDialog> sceneDialogRepository, IAzureStorageBlob azureStorageBlob)
        {   
            _dialogPageRepository = dialogRepository;
            _sceneDialogRepository = sceneDialogRepository;
            _azureStorageBlob = azureStorageBlob;
        }

        public async Task<Result> CreateDialogPageAsync(int sceneDialogId, int orderNum, IFormFile photo, CancellationToken ct)
        {
            var sceneDialog = await _sceneDialogRepository.GetByIdAsync(sceneDialogId, ct);

            if (sceneDialog is null)
                return Result.Fail(new Error("SceneDialog.NotFound", "Scene dialog does not exist."));

            var (photoUrl, fileName) = await _azureStorageBlob.UploadPhoto(photo);

            var dialogPage = new DialogPage
            {
                SceneDialogId = sceneDialogId,
                OrderNum = orderNum,
                PhotoUrl = photoUrl,
                FileName = fileName,
                CreateDate = DateTime.UtcNow,
                UpdateDate = DateTime.UtcNow,
            };

            await _dialogPageRepository.AddAsync(dialogPage, ct);
            await _dialogPageRepository.SaveChangesAsync(ct);

            return Result.Ok();
        }

        public async Task<Result> DeleteDialogPageAsync(int dialogPageId, CancellationToken ct)
        {
            var dialogPage = await _dialogPageRepository.GetByIdAsync(dialogPageId, ct);

            if (dialogPage is null)
                return Result.Fail(new Error("DialogPage.NotFound", "Dialog page does not exist."));

            _dialogPageRepository.Remove(dialogPage);
            await _dialogPageRepository.SaveChangesAsync(ct);

            return Result.Ok();
        }

        public async Task<Result> EditDialogPageAsync(int dialogPageId, int? orderNum, IFormFile? photo, CancellationToken ct)
        {
            var dialogPage = await _dialogPageRepository.GetByIdAsync(dialogPageId, ct);

            if (dialogPage is null)
                return Result.Fail(new Error("DialogPage.NotFound", "Dialog page does not exist."));

            if (orderNum is not null)
                dialogPage.OrderNum = (int)orderNum;

            if(photo is not null && dialogPage.PhotoUrl is not null)
            {
                await _azureStorageBlob.DeletePhotoFromUrl(dialogPage.PhotoUrl);
            }

            if(photo is not null)
            {
                var (photoUrl, fileName) = await _azureStorageBlob.UploadPhoto(photo);

                dialogPage.PhotoUrl = photoUrl;
                dialogPage.FileName = fileName;
            }

            await _dialogPageRepository.SaveChangesAsync(ct);
            return Result.Ok();    
        }
    }
}
