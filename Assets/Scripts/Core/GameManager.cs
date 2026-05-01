using System.Collections.Generic;
using Scripts.Enums;
using Scripts.Grid;
using Scripts.Misc;
using UnityEngine;

namespace Scripts.Core
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; }

        public Queue<Character> charactersQueue = new();

        private bool isGameWon;
        public bool IsGameWon => isGameWon;

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void GameOver(bool value)
        {
            if (value)
            {
                isGameWon = true;
                UIManager.Instance.ShowGameEndStatus(value);
                SoundManager.Instance.PlayGameLoseSound();
            }
            else
            {
                isGameWon = false;
                UIManager.Instance.ShowGameEndStatus(value);
                SoundManager.Instance.PlayGameWinSound();
            }
        }
    }
}
