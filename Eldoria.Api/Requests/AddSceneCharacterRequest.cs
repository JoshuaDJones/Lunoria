using Microsoft.AspNetCore.Mvc;

namespace Eldoria.Api.Requests
{
    public class AddSceneCharacterRequest
    {
        public int SceneId { get; set; }
        public int CharacterId { get; set; }        
    }
}
