using UnityEngine;

namespace FlappyPlane
{
    public class FakePlayer : MonoBehaviour
    {
        [SerializeField] private int pointsPerSpike = 2;
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Hole"))
                Score.AddPoints(pointsPerSpike);
        }
    }
}