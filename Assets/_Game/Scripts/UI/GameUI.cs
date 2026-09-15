using UnityEngine;
using TMPro;
using RockPaperPistol.Events;
using System.Runtime.Serialization;
using System.Collections.Generic;
using UnityEngine.UI;

namespace RockPaperPistol.UI
{
    public class GameUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _roundText;
        [SerializeField] private List<Image> _roundElement;
        [SerializeField] private Sprite _drawSprite;
        void OnEnable()
        {
            GameEvents.UI.onRoundUpdate += UpdateRoundText;
            GameEvents.UI.onWinnerUpdate += UpdateImageRoundWinner;

        }

        void OnDisable()
        {
            GameEvents.UI.onRoundUpdate -= UpdateRoundText;
        }

        private void UpdateRoundText(int currentRound, int totalRound)
        {
            _roundText.text = currentRound + " / " + totalRound;
        }

        private void UpdateImageRoundWinner(int currentRound, Sprite avatarWinner = null)
        {
            _roundElement[currentRound - 1].sprite = avatarWinner == null ? _drawSprite : avatarWinner;
            _roundElement[currentRound - 1].enabled = true;
        }
    }
}