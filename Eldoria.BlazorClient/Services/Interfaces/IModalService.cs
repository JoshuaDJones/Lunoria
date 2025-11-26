using Microsoft.AspNetCore.Components;

namespace Eldoria.BlazorClient.Services.Interfaces
{
    public interface IModalService
    {
        event Action? OnChanged;
        IReadOnlyList<RenderFragment> Stack { get; }
        void Push(RenderFragment fragment);
        void Pop();
        void Clear();
    }
}
