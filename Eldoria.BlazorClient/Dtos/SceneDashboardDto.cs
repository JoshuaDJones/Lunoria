namespace Eldoria.BlazorClient.Dtos
{
    public class SceneDashboardDto
    {
        public SceneDto Scene { get; set; } = null!;
        public List<JourneyCharacterDto> Players { get; set; } = [];
    }
}
