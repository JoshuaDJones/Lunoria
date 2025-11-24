using Eldoria.BlazorClient.Common.Enums;

namespace Eldoria.BlazorClient.Services.Interfaces
{
    public interface IToastService
    {
        void showInformation(string message);
        void showSuccess(string message);
        void showError(string message);

        event Action<string, ToastLevel>? OnShow;
    }
}
