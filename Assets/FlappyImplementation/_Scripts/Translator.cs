using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FlappyPlane
{
    [SelectionBase]
    public class Translator : MonoBehaviour
    {
        private bool move;
        private Transform[] transforms;

        public event Action<GameObject> OnSpriteResetPosition;

        [Header("Random Height")]
        [SerializeField] private float maxHeight;

        [SerializeField] private int minScore;
        [SerializeField] private int maxScore;

        [Space(5)]
        [Header("Translation")]
        [SerializeField] private float speed = -1;

        [SerializeField] [Tooltip("Minimum position X before the sprite is reset")] private float minPosition = -6;
        [SerializeField] private float resetTranslation = 16;

        Vector3[] originalPos;

        private void Awake()
        {
            transforms = new Transform[transform.childCount];
            for (int i = 0; i < transforms.Length; i++)
                transforms[i] = transform.GetChild(i);

            if (transforms.Length == 0) {
                Debug.LogWarning($"No child found under {name}");
                Destroy(this);
                return;
            }

            originalPos = new Vector3[transforms.Length];
            for (int i = 0; i < transforms.Length; i++)
                originalPos[i] = transforms[i].position;

            GameController.OnGameStart += StartGame;
            GameController.OnGameReset += ResetObject;
        }

        private void Update()
        {
            if (move)
                UpdateSprite();
        }

        private void UpdateSprite()
        {
            for (int i = transforms.Length - 1; i >= 0; i--) {
                if (transforms[i].position.x < minPosition)
                    ResetSprite(i);
                else
                    transforms[i].Translate(speed * Time.deltaTime * GameController.Speed, 0, 0, Space.World);
            }
        }

        private void ResetSprite(int index)
        {
            if (maxHeight != 0 && Score.CurrentScore > minScore) {
                float newHeightRange = maxHeight * (Score.CurrentScore / (float)maxScore).Capped();
                transforms[index].Translate(
                    speed * Time.deltaTime * GameController.Speed + resetTranslation,
                    -transforms[index].position.y + Random.Range(-newHeightRange, newHeightRange),
                    0, Space.World);
            }
            else {
                transforms[index].Translate(speed * Time.deltaTime * GameController.Speed + resetTranslation, 0, 0, Space.World);
            }
            OnSpriteResetPosition?.Invoke(transforms[index].gameObject);
        }

        public void StartGame() => move = true;

        public void Pause() => move = false;

        public void Continue() => move = true;

        private void OnDestroy()
        {
            GameController.OnGameStart -= StartGame;
        }

        private void ResetObject()
        {
            for (int i = 0; i < transforms.Length; i++)
                transforms[i].position = originalPos[i];
        }

#if UNITY_EDITOR

        private void Reset()
        {
            maxHeight = 1.5f;
            maxScore = 40;
            speed = -1;
            minPosition = -6;
            resetTranslation = 16;
        }

#endif
    }
}