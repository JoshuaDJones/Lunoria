using Eldoria.BlazorClient.Common.Enums;
using Eldoria.BlazorClient.Services.Interfaces;

namespace Eldoria.BlazorClient.Services
{
    public class ToastService : IToastService
    {
        public event Action<string, ToastLevel>? OnShow;

        public void showError(string message)
        {
            OnShow?.Invoke(message, ToastLevel.Error);
        }

        public void showSuccess(string message)
        {
            OnShow?.Invoke(message, ToastLevel.Success);
        }

        public void showInformation(string message)
        {
            OnShow?.Invoke(message, ToastLevel.Informational);
        }
    }
}
