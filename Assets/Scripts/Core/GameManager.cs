using System;
using UnityEngine;

namespace StarScrape.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public enum GameState { MainMenu, Playing, GameOver }
        public GameState CurrentState { get; private set; }

        public event Action<GameState> OnGameStateChanged;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            ChangeState(GameState.Playing); // Default to playing for prototype
        }

        public void ChangeState(GameState newState)
        {
            CurrentState = newState;
            OnGameStateChanged?.Invoke(newState);

            if (newState == GameState.GameOver)
            {
                HandleGameOver();
            }
        }

        private void HandleGameOver()
        {
            // Logic for triggering the prestige system and restarting the run
            Debug.Log("Game Over! Processing prestige...");
            // TODO: Convert remaining special resources, show UI, then restart
        }
    }
}
