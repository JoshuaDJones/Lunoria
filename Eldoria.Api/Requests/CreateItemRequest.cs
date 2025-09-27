namespace Eldoria.Api.Requests
{
    public class CreateItemRequest
    {
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public IFormFile Photo { get; set; } = default!;
        public int HpEffect { get; set; }
        public int MpEffect { get; set; }
    }
}
