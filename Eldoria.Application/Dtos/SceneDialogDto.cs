using Eldoria.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eldoria.Application.Dtos
{
    public class SceneDialogDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public List<DialogPageDto>? DialogPages { get; set; } = [];
    }
}
