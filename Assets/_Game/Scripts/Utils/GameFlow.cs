using RockPaperPistol.Core;
using UnityEngine;

namespace RockPaperPistol.Utils
{
    public sealed class GameFlow : MonoBehaviour
    {
        public const string MenuScene = "MainMenu";
        public const string WeaponScene = "WeaponScene";
        public const string BattleScene = "SampleScene";

        public static GameFlow Current { get; private set; }

        public PistolId SelectedPistol { get; private set; } = PistolId.Pistoleiro;

        public PlayerLoadout Loadout => new PlayerLoadout(SelectedPistol);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Ensure()
        {
            if (Current != null)
            {
                return;
            }

            GameObject root = new GameObject("GameFlow");
            DontDestroyOnLoad(root);
            Current = root.AddComponent<GameFlow>();
        }

        public void SelectPistol(PistolsEnum pistol)
        {
            SelectedPistol = ToPistolId(pistol);
        }

        public void SelectPistol(PistolId pistol)
        {
            if (pistol == PistolId.None)
            {
                pistol = PistolId.Pistoleiro;
            }

            SelectedPistol = pistol;
        }

        public static bool IsBattleScene(string sceneName)
        {
            return sceneName == BattleScene;
        }

        public static void GoToMenu()
        {
            Utils.LoadScene(MenuScene);
        }

        public static void GoToWeaponSelect()
        {
            Utils.LoadScene(WeaponScene);
        }

        public static void GoToBattle()
        {
            Utils.LoadScene(BattleScene);
        }

        public static PistolId ToPistolId(PistolsEnum pistol)
        {
            switch (pistol)
            {
                case PistolsEnum.MummyPistol:
                    return PistolId.Mumia;
                case PistolsEnum.PiratePistol:
                    return PistolId.Pirata;
                case PistolsEnum.StonePistol:
                    return PistolId.Estatua;
                default:
                    return PistolId.Pistoleiro;
            }
        }
    }
}
