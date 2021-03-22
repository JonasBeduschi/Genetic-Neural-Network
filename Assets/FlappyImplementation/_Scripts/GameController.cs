using System;
using UnityEngine;

namespace FlappyPlane
{
    public class GameController : MonoBehaviour
    {
        public static GameController Instance;
        public static float Speed { get => speed; private set => speed = value; }
        private static float speed = 1f;
        public const float MaxSpeed = 5f;
        public const int ScoreToMaxSpeed = 400;

        public static event Action OnGameStart;
        public static event Action OnGameReset;

        private void Awake()
        {
            if (Instance != null && Instance != this) {
                Destroy(this);
            }
            else {
                Instance = this;
                Score.OnScoreChange += ChangeSpeed;
            }
        }

        private void Start()
        {
            OnGameStart?.Invoke();
        }


        private static void ChangeSpeed(int score)
        {
            if (score >= ScoreToMaxSpeed)
                speed = MaxSpeed;
            else
                speed = 1f + (score / (float)ScoreToMaxSpeed) * (MaxSpeed - 1);
        }

        public static void ResetGame()
        {
            Score.ResetScore();
            OnGameReset?.Invoke();
        }

        private void OnDestroy()
        {
            Score.OnScoreChange -= ChangeSpeed;
        }
    }
}