using Eldoria.Application.Services;
using Eldoria.Core.Entities.Playthrough.Base;
using Eldoria.Core.Entities.Playthrough.Journey;
using Eldoria.Core.Entities.Playthrough.Scene;
using Eldoria.Core.Enums;
using Eldoria.Core.Interfaces;
using NSubstitute;

namespace Eldoria.Application.Tests;

public sealed class ScenePlaythroughCombatTests
{
    private static readonly CancellationToken Ct = CancellationToken.None;

    [Fact]
    public async Task MeleeAttack_DefeatsEnemy_RemovesParticipant_AndRewardsPlayer()
    {
        var repository = CreateRepository(out var transaction);
        var attacker = PlayerParticipant(
            id: 1,
            name: "Hero",
            currentHp: 5,
            currentMp: 5,
            meleeDamage: 4);
        var target = SceneParticipant(
            id: 2,
            name: "Goblin",
            ParticipantType.Enemy,
            currentHp: 5);
        var scene = Scene(attacker, target);
        repository.GetSceneForCharacterInstanceAddAsync(7, 8, 9, Ct)
            .Returns(scene);

        var service = new ScenePlaythroughService(repository);
        var result = await service.AttackAsync(
            7, 8, 9, attacker.Id, target.Id,
            SceneAttackType.Melee, 1, null, Ct);

        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal(5, result.Value.Damage);
        Assert.True(result.Value.TargetDefeated);
        Assert.Equal(0, target.ScenePlaythroughCharacter!.CurrentHp);
        Assert.True(target.ScenePlaythroughCharacter.IsDead);
        Assert.False(target.ScenePlaythroughCharacter.IsActive);
        Assert.DoesNotContain(target, scene.SceneParticipants);
        Assert.True(
            attacker.JourneyPlaythroughCharacter!.CurrentHp == 9 ^
            attacker.JourneyPlaythroughCharacter.CurrentMp == 9);
        await repository.Received(1).SaveChangesAsync(Ct);
        await transaction.Received(1).CommitAsync(Ct);
    }

    [Fact]
    public async Task SpellAttack_UsesSpellDamageAndRoll_AndDeductsMp()
    {
        var repository = CreateRepository(out _);
        var attacker = PlayerParticipant(
            id: 1,
            name: "Mage",
            currentHp: 10,
            currentMp: 8,
            meleeDamage: null);
        attacker.JourneyPlaythroughCharacter!.Spells.Add(
            new JourneyPTCharacterSpell
            {
                PlaythroughSpellId = 30,
                PlaythroughSpell = new PlaythroughSpell
                {
                    Id = 30,
                    Name = "Arc Bolt",
                    DamageEffect = 6,
                    MpCost = 3,
                    PlaythroughSpellType = new PlaythroughSpellType
                    {
                        TypeName = "Arcane"
                    }
                }
            });
        var target = SceneParticipant(
            id: 2,
            name: "Ogre",
            ParticipantType.Enemy,
            currentHp: 20);
        var scene = Scene(attacker, target);
        repository.GetSceneForCharacterInstanceAddAsync(7, 8, 9, Ct)
            .Returns(scene);

        var service = new ScenePlaythroughService(repository);
        var result = await service.AttackAsync(
            7, 8, 9, attacker.Id, target.Id,
            SceneAttackType.Spell, 4, 30, Ct);

        Assert.True(result.Success);
        Assert.NotNull(result.Value);
        Assert.Equal(10, result.Value.Damage);
        Assert.Equal(10, target.ScenePlaythroughCharacter!.CurrentHp);
        Assert.Equal(5, attacker.JourneyPlaythroughCharacter.CurrentMp);
    }

    [Fact]
    public async Task MeleeAttack_AppliesDuplicateEquipmentBonusesAndReductionsOnce()
    {
        var repository = CreateRepository(out _);
        var attacker = PlayerParticipant(
            id: 1,
            name: "Knight",
            currentHp: 10,
            currentMp: 5,
            meleeDamage: 4);
        var weapon = new PlaythroughEquippableItem
        {
            Id = 50,
            Name = "Runed Sword",
            MeleeAttackDamageModifier = 3
        };
        attacker.JourneyPlaythroughCharacter!.EquippableItems.Add(
            EquippedJourneyItem(501, weapon));
        attacker.JourneyPlaythroughCharacter.EquippableItems.Add(
            EquippedJourneyItem(502, weapon));

        var target = SceneParticipant(
            id: 2,
            name: "Guardian",
            ParticipantType.Enemy,
            currentHp: 20);
        var armor = new PlaythroughEquippableItem
        {
            Id = 60,
            Name = "Iron Armor",
            MeleeDamageReduction = 2
        };
        target.ScenePlaythroughCharacter!.EquippableItems.Add(
            EquippedSceneItem(601, armor));
        target.ScenePlaythroughCharacter.EquippableItems.Add(
            EquippedSceneItem(602, armor));

        var scene = Scene(attacker, target);
        repository.GetSceneForCharacterInstanceAddAsync(7, 8, 9, Ct)
            .Returns(scene);
        var service = new ScenePlaythroughService(repository);

        var result = await service.AttackAsync(
            7, 8, 9, attacker.Id, target.Id,
            SceneAttackType.Melee, 1, null, Ct);

        Assert.True(result.Success);
        Assert.Equal(6, result.Value!.Damage);
        Assert.Equal(14, target.ScenePlaythroughCharacter.CurrentHp);
    }

    [Fact]
    public async Task AdditionalAttack_KeepsTurnUntilAllowanceIsSpent()
    {
        var repository = CreateRepository(out _);
        var attacker = PlayerParticipant(
            id: 1,
            name: "Rogue",
            currentHp: 10,
            currentMp: 5,
            meleeDamage: 2);
        var dagger = new PlaythroughEquippableItem
        {
            Id = 50,
            Name = "Dagger",
            AdditionalAttacksPerTurn = 1
        };
        attacker.JourneyPlaythroughCharacter!.EquippableItems.Add(
            EquippedJourneyItem(501, dagger));
        attacker.AttacksRemaining = 2;

        var target = SceneParticipant(
            id: 2,
            name: "Training Dummy",
            ParticipantType.Enemy,
            currentHp: 20);
        var scene = Scene(attacker, target);
        repository.GetSceneForCharacterInstanceAddAsync(7, 8, 9, Ct)
            .Returns(scene);
        var service = new ScenePlaythroughService(repository);

        var firstAttack = await service.AttackAsync(
            7, 8, 9, attacker.Id, target.Id,
            SceneAttackType.Melee, 1, null, Ct);

        Assert.True(firstAttack.Success);
        Assert.Equal(1, attacker.AttacksRemaining);
        Assert.Same(attacker, scene.CurrentParticipant);

        var secondAttack = await service.AttackAsync(
            7, 8, 9, attacker.Id, target.Id,
            SceneAttackType.Melee, 1, null, Ct);

        Assert.True(secondAttack.Success);
        Assert.Equal(0, attacker.AttacksRemaining);
        Assert.Same(target, scene.CurrentParticipant);
        Assert.Equal(1, target.AttacksRemaining);
    }

    [Fact]
    public async Task DownedPlayer_RecoversOnFifthScheduledTurn()
    {
        var repository = CreateRepository(out _);
        var player = PlayerParticipant(
            id: 1,
            name: "Hero",
            currentHp: 4,
            currentMp: 5,
            meleeDamage: 2);
        var enemy = SceneParticipant(
            id: 2,
            name: "Wraith",
            ParticipantType.Enemy,
            currentHp: 10,
            meleeDamage: 3);
        var scene = Scene(enemy, player);
        repository.GetSceneForCharacterInstanceAddAsync(7, 8, 9, Ct)
            .Returns(scene);
        var service = new ScenePlaythroughService(repository);

        var attack = await service.AttackAsync(
            7, 8, 9, enemy.Id, player.Id,
            SceneAttackType.Melee, 1, null, Ct);

        Assert.True(attack.Success);
        Assert.True(player.JourneyPlaythroughCharacter!.IsDown);
        Assert.Equal(4, player.DownedTurnsRemaining);
        Assert.Same(enemy, scene.CurrentParticipant);

        for (var turn = 0; turn < 3; turn++)
        {
            var forfeit = await service.ForfeitActionAsync(
                7, 8, 9, enemy.Id, Ct);
            Assert.True(forfeit.Success);
            Assert.True(player.JourneyPlaythroughCharacter.IsDown);
            Assert.Same(enemy, scene.CurrentParticipant);
        }

        var recoveryTurn = await service.ForfeitActionAsync(
            7, 8, 9, enemy.Id, Ct);

        Assert.True(recoveryTurn.Success);
        Assert.False(player.JourneyPlaythroughCharacter.IsDown);
        Assert.Equal(1, player.JourneyPlaythroughCharacter.CurrentHp);
        Assert.Null(player.DownedTurnsRemaining);
        Assert.Same(player, scene.CurrentParticipant);
        Assert.Contains(
            scene.Playthrough.EventLogs,
            eventLog => eventLog.Message == "Hero recovered with 1 HP");
    }

    private static IPlaythroughRepository CreateRepository(
        out IPlaythroughTransaction transaction)
    {
        var repository = Substitute.For<IPlaythroughRepository>();
        transaction = Substitute.For<IPlaythroughTransaction>();
        repository.BeginSceneStartTransactionAsync(Ct).Returns(transaction);
        repository.SaveChangesAsync(Ct).Returns(1);
        return repository;
    }

    private static ScenePT Scene(
        ScenePTParticipant current,
        params ScenePTParticipant[] others)
    {
        ScenePTParticipant[] participants = [current, .. others];
        var scene = new ScenePT
        {
            Id = 9,
            PlaythroughId = 8,
            Status = ScenePlaythroughStatus.InProgress,
            RoundNumber = 1,
            CurrentParticipantId = current.Id,
            CurrentParticipant = current,
            Playthrough = new Playthrough { Id = 8 }
        };

        foreach (var participant in participants)
        {
            participant.ScenePlaythrough = scene;
            participant.ScenePlaythroughId = scene.Id;
            scene.SceneParticipants.Add(participant);
        }

        return scene;
    }

    private static ScenePTParticipant PlayerParticipant(
        int id,
        string name,
        int currentHp,
        int currentMp,
        int? meleeDamage)
    {
        var character = new JourneyPTCharacter
        {
            Id = id + 100,
            CurrentHp = currentHp,
            MaxHp = 20,
            CurrentMp = currentMp,
            MaxMp = 20,
            MeleeAttackDamage = meleeDamage,
            PlaythroughCharacter = new PlaythroughCharacter
            {
                Id = id + 200,
                Name = name
            }
        };

        return new ScenePTParticipant
        {
            Id = id,
            IsActive = true,
            ParticipantType = ParticipantType.Player,
            SortOrderWithinType = id,
            JourneyPlaythroughCharacterId = character.Id,
            JourneyPlaythroughCharacter = character
        };
    }

    private static ScenePTParticipant SceneParticipant(
        int id,
        string name,
        ParticipantType type,
        int currentHp,
        int? meleeDamage = null)
    {
        var character = new ScenePTCharacter
        {
            Id = id + 100,
            CurrentHp = currentHp,
            MaxHp = currentHp,
            CurrentMp = 0,
            MaxMp = 0,
            MeleeAttackDamage = meleeDamage,
            IsActive = true,
            PlaythroughCharacter = new PlaythroughCharacter
            {
                Id = id + 200,
                Name = name,
                CharacterType = type == ParticipantType.NPC
                    ? CharacterType.NPC
                    : CharacterType.Enemy
            }
        };

        return new ScenePTParticipant
        {
            Id = id,
            IsActive = true,
            ParticipantType = type,
            SortOrderWithinType = id,
            ScenePlaythroughCharacterId = character.Id,
            ScenePlaythroughCharacter = character
        };
    }

    private static JourneyPTCharacterEquippableItem EquippedJourneyItem(
        int linkId,
        PlaythroughEquippableItem item)
    {
        return new JourneyPTCharacterEquippableItem
        {
            Id = linkId,
            IsEquipped = true,
            PlaythroughEquippableItemId = item.Id,
            PlaythroughEquippableItem = item
        };
    }

    private static ScenePTCharacterEquippableItem EquippedSceneItem(
        int linkId,
        PlaythroughEquippableItem item)
    {
        return new ScenePTCharacterEquippableItem
        {
            Id = linkId,
            IsEquipped = true,
            PlaythroughEquippableItemId = item.Id,
            PlaythroughEquippableItem = item
        };
    }
}
