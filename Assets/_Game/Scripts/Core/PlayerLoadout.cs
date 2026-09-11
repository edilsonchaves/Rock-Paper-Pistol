using System;

namespace RockPaperPistol.Core
{
    public sealed class PlayerLoadout
    {
        public PlayerLoadout(PistolId pistol)
        {
            if (pistol == PistolId.None)
            {
                throw new ArgumentOutOfRangeException(nameof(pistol), pistol, "A loadout precisa de uma pistola.");
            }

            Pistol = Card.CreatePistol(pistol);
        }

        public Card Pistol { get; }

        public static PlayerLoadout Default { get; } = new PlayerLoadout(PistolId.Pistoleiro);
    }
}
