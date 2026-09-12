using RockPaperPistol.Core;
using UnityEngine;

namespace RockPaperPistol.Unity
{
    [CreateAssetMenu(fileName = "GameSounds", menuName = "Rock Paper Pistol/Sons do Jogo")]
    public sealed class GameSounds : ScriptableObject
    {
        public const string DefaultAssetPath = "Assets/_Game/Content/GameSounds.asset";

        [Header("Volumes")]
        [Range(0f, 1f)]
        [Tooltip("Volume da música em loop (menu e batalha).")]
        [SerializeField] private float musicVolume = 0.28f;

        [Range(0f, 1f)]
        [Tooltip("Volume dos efeitos da gameplay.")]
        [SerializeField] private float sfxVolume = 0.85f;

        [Header("Biblioteca — arquivos em Assets/_Game/Sounds")]
        [Tooltip("phantasticbeats-carnival-109979. Tema do parque.")]
        [SerializeField] private AudioClip carnival;

        [Tooltip("freesound_community-shuffleandcardflip1-40942. Embaralha e vira carta.")]
        [SerializeField] private AudioClip cardFlip;

        [Tooltip("freesound_community-baralho-101467. Baralho / compra.")]
        [SerializeField] private AudioClip deckCards;

        [Tooltip("oxidvideos-placing-poker-chips-522515. Fichas na mesa.")]
        [SerializeField] private AudioClip chips;

        [Tooltip("universfield-gunshot-352466. Tiro. Reservado para a carta pistola.")]
        [SerializeField] private AudioClip gunshot;

        [Tooltip("dragon-studio-heavy-boulder-thud-515257. Pedra. Reservado para o naipe Pedra.")]
        [SerializeField] private AudioClip rockThud;

        [Tooltip("freesound_community-papel-empujado-100951. Papel. Reservado para o naipe Papel.")]
        [SerializeField] private AudioClip paperSlide;

        [Tooltip("freesound_community-scissors-69248. Tesoura. Reservado para o naipe Tesoura.")]
        [SerializeField] private AudioClip scissors;

        [Tooltip("spinopel-cut-using-a-scissor-429792. Corte de tesoura. Alternativa de naipe.")]
        [SerializeField] private AudioClip scissorsCut;

        [Tooltip("freesound_community-goodresult-82807. Resultado positivo.")]
        [SerializeField] private AudioClip goodResult;

        [Tooltip("primalhousemusic-production-elements-impactor-e-188986. Impacto / derrota.")]
        [SerializeField] private AudioClip impact;

        [Tooltip("tithuh-level-up-0-523643. Level up.")]
        [SerializeField] private AudioClip levelUp;

        [Tooltip("u_c0n7gy2b9p-level-up-232906. Level up curto.")]
        [SerializeField] private AudioClip levelUpAlt;

        [Tooltip("mrstokes302-you-win-sfx-mrstokes302-442128. Vitória da run.")]
        [SerializeField] private AudioClip youWin;

        [Tooltip("driken5482-applause-cheer-236786. Aplausos.")]
        [SerializeField] private AudioClip applause;

        [Tooltip("universfield-yeah-boy-114748. Yeah boy.")]
        [SerializeField] private AudioClip yeahBoy;

        [Tooltip("freesound_community-taratata-6264. Fanfarra.")]
        [SerializeField] private AudioClip fanfare;

        [Header("Onde toca — arraste um clip da biblioteca")]
        [Tooltip("Toca em loop no menu e na batalha.")]
        [SerializeField] private AudioClip musicLoop;

        [Tooltip("GameplayEvent.TurnStart — início do turno / mão nova.")]
        [SerializeField] private AudioClip onTurnStart;

        [Tooltip("GameplayEvent.CardSelected — jogador escolhe a carta.")]
        [SerializeField] private AudioClip onCardSelected;

        [Tooltip("GameplayEvent.CardsRevealed — as duas cartas entram na mesa.")]
        [SerializeField] private AudioClip onCardsRevealed;

        [Tooltip("GameplayEvent.SuitChecked — comparação de naipe. Vazio de propósito até ligar Pedra/Papel/Tesoura.")]
        [SerializeField] private AudioClip onSuitChecked;

        [Tooltip("GameplayEvent.ValueChecked — comparação de número.")]
        [SerializeField] private AudioClip onValueChecked;

        [Tooltip("GameplayEvent.PlayerWin — vitória do round.")]
        [SerializeField] private AudioClip onPlayerWin;

        [Tooltip("GameplayEvent.PlayerLose — derrota do round.")]
        [SerializeField] private AudioClip onPlayerLose;

        [Tooltip("GameplayEvent.Draw — empate do round.")]
        [SerializeField] private AudioClip onDraw;

        [Tooltip("GameplayEvent.CardDiscarded — carta vai ao descarte.")]
        [SerializeField] private AudioClip onCardDiscarded;

        [Tooltip("GameplayEvent.GameWin — venceu a run.")]
        [SerializeField] private AudioClip onGameWin;

        [Tooltip("GameplayEvent.GameLose — perdeu a run.")]
        [SerializeField] private AudioClip onGameLose;

        public float MusicVolume => musicVolume;
        public float SfxVolume => sfxVolume;
        public AudioClip Music => musicLoop != null ? musicLoop : carnival;
        public AudioClip Carnival => carnival;
        public AudioClip CardFlip => cardFlip;
        public AudioClip DeckCards => deckCards;
        public AudioClip Chips => chips;
        public AudioClip Gunshot => gunshot;
        public AudioClip Rock => rockThud;
        public AudioClip Paper => paperSlide;
        public AudioClip Scissors => scissors != null ? scissors : scissorsCut;
        public AudioClip ScissorsCut => scissorsCut;
        public AudioClip GoodResult => goodResult;
        public AudioClip Impact => impact;
        public AudioClip LevelUp => levelUp;
        public AudioClip LevelUpAlt => levelUpAlt;
        public AudioClip YouWin => youWin;
        public AudioClip Applause => applause;
        public AudioClip YeahBoy => yeahBoy;
        public AudioClip Fanfare => fanfare;

        public AudioClip ClipFor(GameplayEvent gameplayEvent)
        {
            switch (gameplayEvent)
            {
                case GameplayEvent.TurnStart:
                    return onTurnStart;
                case GameplayEvent.CardSelected:
                    return onCardSelected;
                case GameplayEvent.CardsRevealed:
                    return onCardsRevealed;
                case GameplayEvent.SuitChecked:
                    return onSuitChecked;
                case GameplayEvent.ValueChecked:
                    return onValueChecked;
                case GameplayEvent.PlayerWin:
                    return onPlayerWin;
                case GameplayEvent.PlayerLose:
                    return onPlayerLose;
                case GameplayEvent.Draw:
                    return onDraw;
                case GameplayEvent.CardDiscarded:
                    return onCardDiscarded;
                case GameplayEvent.GameWin:
                    return onGameWin;
                case GameplayEvent.GameLose:
                    return onGameLose;
                default:
                    return null;
            }
        }
    }
}
