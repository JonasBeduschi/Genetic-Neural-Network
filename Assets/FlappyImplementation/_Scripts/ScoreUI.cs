using UnityEngine;
using UnityEngine.UI;

namespace FlappyPlane
{
    public class ScoreUI : MonoBehaviour
    {
        private Text scoreText;

        private void Awake()
        {
            scoreText = GetComponent<Text>();
            scoreText.enabled = false;
            GameController.OnGameStart += Show;
            Score.OnScoreChange += UpdateScore;
        }

        private void Show()
        {
            scoreText.enabled = true;
        }

        public void UpdateScore(int value)
        {
            scoreText.text = $"Score: {value}";
        }

        private void OnDestroy()
        {
            GameController.OnGameStart -= Show;
            Score.OnScoreChange -= UpdateScore;
        }
    }
}