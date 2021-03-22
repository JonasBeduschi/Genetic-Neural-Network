using UnityEngine;

namespace FlappyPlane
{
    public class PlayerVisual : MonoBehaviour
    {
        [SerializeField] private float maxRotation;
        [SerializeField] private float rotationSpeed;
        private Rigidbody2D rb;
        private bool gameStarted = false;

        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            GameController.OnGameStart += GameStarted;
        }

        private void Update()
        {
            if (gameStarted)
                UpdateRotation();
        }

        private void GameStarted()
        {
            gameStarted = true;
        }

        private void UpdateRotation()
        {
            transform.rotation = Quaternion.Euler(0, 0, (rb.velocity.y * rotationSpeed).Capped(-maxRotation, maxRotation));
        }
    }
}