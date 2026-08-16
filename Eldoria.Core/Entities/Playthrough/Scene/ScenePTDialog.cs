namespace Eldoria.Core.Entities.Playthrough.Scene
{
    public class ScenePTDialog
    {
        public int Id { get; set; }
        public int SourceSceneDialogId { get; set; }
        public string Title { get; set; } = string.Empty;

        public int ScenePTId { get; set; }
        public ScenePT ScenePT { get; set; } = null!;

        public ICollection<ScenePTDialogPage> DialogPages { get; set; } = [];
    }
}
