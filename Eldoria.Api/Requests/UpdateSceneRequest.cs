namespace Eldoria.Api.Requests
{
    public class UpdateSceneRequest
    {
        public int JourneyId { get; set; }
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public IFormFile? Photo { get; set; }
        public string GridUrl { get; set; } = default!;
    }
}
