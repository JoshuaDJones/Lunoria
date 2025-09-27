namespace Eldoria.Api.Requests
{
    public class CreateJourneyRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public IFormFile Photo { get; set; } = null!;
    }
}
