using Eldoria.Core.Entities;
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
        Task<List<SceneDialog>> GetSceneDialogs(int sceneId, CancellationToken ct);
    }
}
