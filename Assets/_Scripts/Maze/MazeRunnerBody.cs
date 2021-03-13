using UnityEngine;

namespace GeneticNeuralNetwork
{
    public class MazeRunnerBody : MonoBehaviour
    {
        [SerializeField] private MazeRunner agent;
        [SerializeField] private Component[] toBeDestroyed;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            agent.Die(transform.localPosition.x, collision.relativeVelocity.magnitude);
            Die();
        }

        public void Die()
        {
            float f = Population.CompareToBestFitness(agent.Fitness);
            spriteRenderer.color = new Color(f, f, f);
            foreach (var item in toBeDestroyed)
                Destroy(item);
        }
    }
}