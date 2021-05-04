using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace InteractiveMiniGame.RussianEggs
{
    public class RussianEggsMainGame : MonoBehaviour
    {
        [Header("Game options")]

        [Tooltip("Initial wolf (player) lives")]
        [Range(1, 3)]
        [SerializeField] public int initialWolfLives = 3;

        [Tooltip("Initial frame interval for eggs update")]
        [Range(1, 100)]
        [SerializeField] public int initialEggsUpdateInterval = 50;

        [Tooltip("Minimum frame interval for eggs update")]
        [Range(1, 100)]
        [SerializeField] public int minEggsUpdateInterval = 10;

        [Tooltip("Initial spawn chance of new egg every eggs update")]
        [Range(0, 100)]
        [SerializeField] public float initialEggSpawnChances = 5.0f;

        [Tooltip("Maximum spawn chance of new egg every eggs update")]
        [Range(0, 100)]
        [SerializeField] public float maxEggSpawnChances = 75.0f;

        [Tooltip("Initial position of wolf (player) character")]
        [SerializeField] public InGamePosition initialWolfPosition;

        // Rendering can pe separated if needed
        [Header("Rendering info")]
        [Tooltip("Rendering informations for wolf character")]
        [SerializeField] public WolfRendering wolfRendering;

        [Tooltip("Rendering informations for sliding eggs")]
        [SerializeField] public EggsRendering eggsRendering;

        [Tooltip("Rendering informations for wolf (player) lives")]
        [SerializeField] public LivesRendering livesRendering;

        [Tooltip("Rendering informations for player score")]
        [SerializeField] public ScoreRendering scoreRendering;

        [Tooltip("Rendering informations for hare")]
        [SerializeField] public HareRendering hareRendering;

        // Setter can be private if needed
        [HideInInspector] public bool gameStarted;

        [HideInInspector] public bool gameOver;

        [HideInInspector] public int score;

        [HideInInspector] public int lives;

        [HideInInspector] public InGamePosition wolfPosition;

        [HideInInspector] public Dictionary<InGamePosition, bool[]> eggsStates;

        [HideInInspector] public int currentEggsUpdateInterval;

        [HideInInspector] public float currentEggSpawnChance;

        // Can be done via iteration if need
        private bool CanAddNewEgg => eggsStates.All(eggsState => !eggsState.Value.FirstOrDefault());

        // Can be done via iteration if need
        private bool AnyEggs => eggsStates.Any(eggsState => eggsState.Value.Any(state => state));

        private int _nextEggsUpdate;

        public void StartGame()
        {
            _nextEggsUpdate = 0;

            eggsStates = new Dictionary<InGamePosition, bool[]>
            {
                { InGamePosition.UpRight, new bool[eggsRendering.upRightEggSlide.eggsRenderers.Length] },
                { InGamePosition.DownRight, new bool[eggsRendering.downRightEggSlide.eggsRenderers.Length] },
                { InGamePosition.UpLeft, new bool[eggsRendering.upLeftEggSlide.eggsRenderers.Length] },
                { InGamePosition.DownLeft, new bool[eggsRendering.downLeftEggSlide.eggsRenderers.Length] },
            };

            wolfPosition = initialWolfPosition;

            currentEggsUpdateInterval = initialEggsUpdateInterval;
            currentEggSpawnChance = initialEggSpawnChances;

            score = 0;

            lives = initialWolfLives;

            gameOver = false;
            gameStarted = true;
        }

        public void GameOver()
        {
            gameOver = true;
        }

        public void ExitGame()
        {
            gameStarted = false;

            wolfRendering.rendrer.sprite = null;

            foreach (var eggsRendering in eggsRendering.upRightEggSlide.eggsRenderers)
                eggsRendering.enabled = false;

            foreach (var eggsRendering in eggsRendering.downRightEggSlide.eggsRenderers)
                eggsRendering.enabled = false;

            foreach (var eggsRendering in eggsRendering.upLeftEggSlide.eggsRenderers)
                eggsRendering.enabled = false;

            foreach (var eggsRendering in eggsRendering.downLeftEggSlide.eggsRenderers)
                eggsRendering.enabled = false;

            livesRendering.rendrer.sprite = null;
            scoreRendering.renderer.text = "";
            hareRendering.renderer.sprite = null;
        }

        void FixedUpdate()
        {
            if (!gameStarted || gameOver)
                return;

            if (lives <= 0)
            {
                GameOver();
                return;
            }

            if (_nextEggsUpdate > 0)
            {
                _nextEggsUpdate--;
                return;
            }
            _nextEggsUpdate = currentEggsUpdateInterval;

            foreach (var eggsState in eggsStates)
            {
                UpdateEggsState(eggsState.Value, eggsState.Key);
            }

            if (!AnyEggs)
                AddRandomEgg();

            if (CanAddNewEgg)
            {
                var random = UnityEngine.Random.Range(0.0f, 100.0f);

                if (random <= currentEggSpawnChance)
                    AddRandomEgg();
            }
        }

        void UpdateEggsState(bool[] eggsState, InGamePosition slidePosition)
        {
            var eggsLastIndex = (eggsState.Length - 1);

            if (eggsState[eggsLastIndex])
                EggFall(slidePosition);

            for (int i = eggsLastIndex; i >= 0; i--)
            {
                if (i == 0)
                    eggsState[i] = false;
                else
                    eggsState[i] = eggsState[i - 1];
            }
        }

        void EggFall(InGamePosition slidePosition)
        {
            if (slidePosition == wolfPosition)
            {
                score += 1;

                currentEggsUpdateInterval = Mathf.Max(initialEggsUpdateInterval - score / 10, minEggsUpdateInterval);
                currentEggSpawnChance = Mathf.Min(initialEggSpawnChances * (score + 10) / 10, maxEggSpawnChances);
            }
            else
            {
                lives--;
            }
        }

        void AddRandomEgg()
        {
            var randomPosition = (InGamePosition) UnityEngine.Random.Range(0, 3);

            eggsStates[randomPosition][0] = true;
        }

        #region Rendering

        void Update()
        {
            if (!gameStarted)
                return;

            UpdateWolfRenderer();
            UpdateEggsRenderers();
            UpdateLivesRenderer();
            UpdateScoreRenderer();
            UpdateHareRenderer();
        }

        void UpdateWolfRenderer()
        {
            switch(wolfPosition)
            {
                case InGamePosition.UpRight:
                    wolfRendering.rendrer.sprite = wolfRendering.upRightSprite;
                    return;
                case InGamePosition.DownRight:
                    wolfRendering.rendrer.sprite = wolfRendering.downRightSprite;
                    return;
                case InGamePosition.UpLeft:
                    wolfRendering.rendrer.sprite = wolfRendering.upLeftSprite;
                    return;
                case InGamePosition.DownLeft:
                    wolfRendering.rendrer.sprite = wolfRendering.downLeftSprite;
                    return;
            }
        }

        void UpdateEggsRenderers()
        {
            UpdateEggsRenderer(eggsRendering.upRightEggSlide.eggsRenderers, eggsStates[InGamePosition.UpRight]);
            UpdateEggsRenderer(eggsRendering.downRightEggSlide.eggsRenderers, eggsStates[InGamePosition.DownRight]);
            UpdateEggsRenderer(eggsRendering.upLeftEggSlide.eggsRenderers, eggsStates[InGamePosition.UpLeft]);
            UpdateEggsRenderer(eggsRendering.downLeftEggSlide.eggsRenderers, eggsStates[InGamePosition.DownLeft]);
        }

        void UpdateEggsRenderer(SpriteRenderer[] eggsRenderer, bool[] eggsState)
        {
            if (eggsRenderer.Length != eggsState.Length)
            {
                Debug.LogError("Size of eggs renderer array is not eqaul to size of eggs state array!");
                return;
            }

            for (int i = 0; i < eggsState.Length; i++)
            {
                eggsRenderer[i].enabled = eggsState[i];
            }
        }

        void UpdateLivesRenderer()
        {
            switch(lives)
            {
                case 2:
                    livesRendering.rendrer.sprite = livesRendering.live2;
                    return;
                case 1:
                    livesRendering.rendrer.sprite = livesRendering.live1;
                    return;
                case 0:
                    livesRendering.rendrer.sprite = livesRendering.live0;
                    return;
                default:
                    livesRendering.rendrer.sprite = livesRendering.live3;
                    return;
            }
        }

        void UpdateScoreRenderer()
        {
            scoreRendering.renderer.text = score.ToString();
        }

        void UpdateHareRenderer()
        {
            if (score != 0 && score % 6 == 0)
                hareRendering.renderer.sprite = hareRendering.hare;
            else
                hareRendering.renderer.sprite = null;
        }

        #endregion Rendering
    }

    [Serializable]
    public enum InGamePosition
    {
        UpRight,
        DownRight,
        UpLeft,
        DownLeft
    }

    [Serializable]
    public struct WolfRendering
    {
        [SerializeField] public SpriteRenderer rendrer;
        [Header("StateSprites")]
        [SerializeField] public Sprite upRightSprite;
        [SerializeField] public Sprite downRightSprite;
        [SerializeField] public Sprite upLeftSprite;
        [SerializeField] public Sprite downLeftSprite;
    }

    [Serializable]
    public struct EggsRendering
    {
        [Header("EggSliders")]
        [SerializeField] public EggsSlide upRightEggSlide;
        [SerializeField] public EggsSlide downRightEggSlide;
        [SerializeField] public EggsSlide upLeftEggSlide;
        [SerializeField] public EggsSlide downLeftEggSlide;
    }

    [Serializable]
    public struct LivesRendering
    {
        [SerializeField] public SpriteRenderer rendrer;
        [Header("StateSprites")]
        [SerializeField] public Sprite live3;
        [SerializeField] public Sprite live2;
        [SerializeField] public Sprite live1;
        [SerializeField] public Sprite live0;
    }

    [Serializable]
    public struct ScoreRendering
    {
        [SerializeField] public TextMeshPro renderer;
    }

    [Serializable]
    public struct HareRendering
    {
        [SerializeField] public SpriteRenderer renderer;
        [SerializeField] public Sprite hare;
    }
}