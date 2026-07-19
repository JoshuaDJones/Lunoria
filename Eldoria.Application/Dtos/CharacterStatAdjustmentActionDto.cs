using Eldoria.Core.Enums;

namespace Eldoria.Application.Dtos
{
    public class CharacterStatAdjustmentActionDto
    {
        public int Id { get; set; }
        public CharacterStatType CharacterStatType { get; set; }
        public AdjustmentOperation AdjustmentOperation { get; set; }
        public int Value { get; set; }
        public int? CharacterId { get; set; }
        public CharacterDto? Character { get; set; }
    }
}
