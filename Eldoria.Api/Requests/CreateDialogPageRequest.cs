namespace Eldoria.Api.Requests
{
    public class CreateDialogPageRequest
    {
        public int OrderNum { get; set; }
        public IFormFile Photo { get; set; } = null!;                
    }
}
