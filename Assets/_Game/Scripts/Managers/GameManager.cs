using UnityEngine;
using RockPaperPistol.Utils;
public class GameManager : Singleton<GameManager>
{
    [SerializeField] private PistolId _heroPistol;
    public PistolId HeroPistol => _heroPistol;

    public void DefineHeroPistol(PistolId pistol)
    {
        _heroPistol = pistol;
    }
}
