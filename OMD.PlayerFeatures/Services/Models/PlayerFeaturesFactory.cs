﻿using Microsoft.Extensions.DependencyInjection;
using OMD.PlayersFeatures.Enumerations;
using OMD.PlayersFeatures.Models;
using OpenMod.API.Ioc;
using OpenMod.Unturned.Players;
using SDG.Unturned;
using System;

namespace OMD.PlayersFeatures.Services;

[PluginServiceImplementation(Lifetime = ServiceLifetime.Singleton)]
public sealed class PlayerFeaturesFactory : IPlayerFeaturesFactory
{
    public FeaturesIntegrationType IntegrationType { get; private set; }

    private Func<Player, PlayerFeatures> _featuresFactory = null!;

    public void SetIntegrationType(string type)
    {
        if (!Enum.TryParse(type, true, out FeaturesIntegrationType integrationType))
        {
            IntegrationType = FeaturesIntegrationType.None;

            throw new ArgumentException($"Failed to parse \"{type}\" to {nameof(FeaturesIntegrationType)}");
        }

        IntegrationType = integrationType;

        _featuresFactory = IntegrationType switch {
            FeaturesIntegrationType.RocketMod => (player) => new RocketModPlayerFeatures(player),
            FeaturesIntegrationType.OpenMod => (player) => new OpenModPlayerFeatures(player),
            _ => (player) => throw new InvalidOperationException("Features integration type is not set!")
        };
    }

    public PlayerFeatures CreateFor(UnturnedPlayer player)
    {
        if (_featuresFactory is null)
            throw new InvalidOperationException("Features integration type is not set!");

        return _featuresFactory(player.Player);
    }
}
