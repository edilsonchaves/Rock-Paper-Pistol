using UnityEngine;
using RockPaperPistol.Utils;
using RockPaperPistol.Data;
public class GameManager : Singleton<GameManager>
{
    [SerializeField] private CardPistolDefinition _heroPistol;
    public CardPistolDefinition HeroPistol => _heroPistol;

    public void DefineHeroPistol(CardPistolDefinition pistol)
    {
        _heroPistol = pistol;
    }
}
