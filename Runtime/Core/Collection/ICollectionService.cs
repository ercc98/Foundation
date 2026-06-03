using System;

namespace ErccDev.Foundation.Core.Collection
{
    /// <summary>
    /// Contract for the "compendium / album" progression: a set of unique entries the player
    /// discovers once and keeps forever (creatures, cards, locations, artwork...). Deliberately
    /// NOT named "collectible" — that word is reserved for in-game pickups (coins, gems) so the
    /// two concepts never clash inside a game project.
    /// </summary>
    public interface ICollectionService
    {
        /// <summary>Has this entry already been discovered?</summary>
        bool IsDiscovered(string entryId);

        /// <summary>Marks an entry as discovered. Returns true only on the first discovery.</summary>
        bool Discover(string entryId);

        /// <summary>How many distinct entries the player has discovered.</summary>
        int DiscoveredCount { get; }

        /// <summary>How many entries the full collection contains.</summary>
        int TotalCount { get; }

        /// <summary>0..1 completion ratio across the whole collection.</summary>
        float Completion01 { get; }

        /// <summary>Raised once, the first time a given entry is discovered.</summary>
        event Action<CollectionEntryDefinition> OnDiscovered;
    }
}
