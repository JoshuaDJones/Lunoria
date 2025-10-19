namespace Eldoria.Application.Dtos
{
    public class SceneDashboardDto
    {
        public SceneDto Scene { get; set; } = null!;
        public List<JourneyCharacterDto> Players { get; set; } = [];
    }
}
