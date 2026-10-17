using Eldoria.Core.Entities;
using Eldoria.Core.Entities.Playthrough.Scene;
using Eldoria.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eldoria.Core.Interfaces
{
    public interface ISceneDialogRepository : IRepository<SceneDialog>
    {
        Task<SceneDialog?> GetForUserAsync(int userId, int sceneDialogId, CancellationToken ct);
        Task<List<SceneDialog>> GetSceneDialogs(int userId, int sceneId, CancellationToken ct);
    }
}
