using RockPaperPistol.Data;
using UnityEngine;

namespace RockPaperPistol.Managers
{
    public class TableManager : MonoBehaviour
    {
        private GameContent _gameContent;
        [SerializeField] private DeckDefinition _playerDeck;
        [SerializeField] private EnemyDefinition _currentEnemy;

        [SerializeField] private Sprite _enemyBody;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            _currentEnemy = _gameContent.Enemies[0];
            
        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
