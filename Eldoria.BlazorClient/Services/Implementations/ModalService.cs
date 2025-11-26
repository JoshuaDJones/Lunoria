using Eldoria.BlazorClient.Services.Interfaces;
using Microsoft.AspNetCore.Components;

namespace Eldoria.BlazorClient.Services.Implementations
{
    public class ModalService : IModalService
    {
        public event Action? OnChanged;
        private readonly List<RenderFragment> _stack = [];

        public IReadOnlyList<RenderFragment> Stack => _stack;

        public void Push(RenderFragment fragment)
        {
            _stack.Add(fragment);
            OnChanged?.Invoke();
        }

        public void Pop()
        {
            if (_stack.Count == 0)
                return;

            OnChanged?.Invoke();
            _stack.RemoveAt(_stack.Count - 1);
        }

        public void Clear()
        {
            _stack.Clear();
            OnChanged?.Invoke();
        }
    }
}
