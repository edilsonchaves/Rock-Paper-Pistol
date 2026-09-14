using UnityEngine;
using RockPaperPistol.Data;
using System.Collections.Generic;
namespace RockPaperPistol.Managers
{ 
    public class EnemyControl : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer _avatarBody;
        [SerializeField] private string _avatarName;
        [SerializeField] private List<CardDefinition> _enemyDeck;

        public void SetupEnemy(EnemyDefinition data)
        {
            _avatarBody.color = data.SpriteColor;
            _avatarName = data.DisplayName;
            foreach(var card in data.Sequence)
            {
                _enemyDeck.Add(card);
            }
        }
    }
}