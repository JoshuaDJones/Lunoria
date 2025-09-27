namespace Eldoria.Api.Requests
{
    public class CreateSceneRequest
    {
        public int JourneyId { get; set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public IFormFile Photo { get; set; } = default!;
        public string GridUrl { get; set; } = default!;
    }
}
