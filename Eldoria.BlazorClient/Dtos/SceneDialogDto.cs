namespace Eldoria.BlazorClient.Dtos
{
    public class SceneDialogDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public List<DialogPageDto>? DialogPages { get; set; } = [];
    }
}
