using Eldoria.BlazorClient.Common.Enums;
using Eldoria.BlazorClient.Services.Interfaces;

namespace Eldoria.BlazorClient.Services
{
    public class ToastService : IToastService
    {
        public event Action<string, ToastLevel>? OnShow;

        public void ShowError(string message)
        {
            OnShow?.Invoke(message, ToastLevel.Error);
        }

        public void ShowSuccess(string message)
        {
            OnShow?.Invoke(message, ToastLevel.Success);
        }

        public void ShowInformation(string message)
        {
            OnShow?.Invoke(message, ToastLevel.Informational);
        }
    }
}
