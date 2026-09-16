using UnityEngine;
using RockPaperPistol.Utils;
using RockPaperPistol.Data;
public class GameManager : Singleton<GameManager>
{
    [SerializeField] private CardPistolDefinition _heroPistol;
    public CardPistolDefinition HeroPistol => _heroPistol;

    [SerializeField] private int _currentLevel;

    public int CurrentLevel => _currentLevel;

    public void SetLevel(int level)
    {
        _currentLevel = level;
    }

    public void DefineHeroPistol(CardPistolDefinition pistol)
    {
        _heroPistol = pistol;
    }
}
