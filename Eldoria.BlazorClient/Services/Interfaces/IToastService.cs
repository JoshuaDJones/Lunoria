using Eldoria.BlazorClient.Common.Enums;

namespace Eldoria.BlazorClient.Services.Interfaces
{
    public interface IToastService
    {
        void ShowInformation(string message);
        void ShowSuccess(string message);
        void ShowError(string message);

        event Action<string, ToastLevel>? OnShow;
    }
}
