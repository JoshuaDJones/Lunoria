namespace Eldoria.Api.Requests
{
    public class UpdateEquippableItemRequest : EquippableItemRequest
    {
        public IFormFile? Photo { get; set; }
    }
}
