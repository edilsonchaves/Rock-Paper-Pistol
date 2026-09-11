using UnityEngine;

namespace RockPaperPistol.Unity
{
    [CreateAssetMenu(fileName = "BattleTiming", menuName = "Rock Paper Pistol/Tempos da Batalha")]
    public sealed class BattleTiming : ScriptableObject
    {
        [Header("Animações do turno")]
        [Min(0f)]
        [Tooltip("Depois da carta escolhida ir à mesa, antes de revelar o oponente.")]
        [SerializeField] private float selectedDelay = GameSessionDriver.DefaultSelectedDelay;

        [Min(0f)]
        [Tooltip("Depois das duas cartas reveladas, antes de checar o naipe.")]
        [SerializeField] private float revealedDelay = GameSessionDriver.DefaultRevealedDelay;

        [Min(0f)]
        [Tooltip("Depois da checagem de naipe, antes da checagem de valor.")]
        [SerializeField] private float suitCheckDelay = GameSessionDriver.DefaultSuitCheckDelay;

        [Min(0f)]
        [Tooltip("Depois da checagem de valor, antes de mostrar o resultado.")]
        [SerializeField] private float valueCheckDelay = GameSessionDriver.DefaultValueCheckDelay;

        [Min(0f)]
        [Tooltip("Resultado visível (vitória, derrota ou empate) antes de encerrar o turno.")]
        [SerializeField] private float resultDelay = GameSessionDriver.DefaultResultDelay;

        [Min(0f)]
        [Tooltip("Pausa antes do próximo turno começar.")]
        [SerializeField] private float nextTurnDelay = GameSessionDriver.DefaultNextTurnDelay;

        public float SelectedDelay => selectedDelay;
        public float RevealedDelay => revealedDelay;
        public float SuitCheckDelay => suitCheckDelay;
        public float ValueCheckDelay => valueCheckDelay;
        public float ResultDelay => resultDelay;
        public float NextTurnDelay => nextTurnDelay;
    }
}
