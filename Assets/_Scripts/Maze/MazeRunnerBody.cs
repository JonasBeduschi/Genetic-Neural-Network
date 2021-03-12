using UnityEngine;

namespace GeneticNeuralNetwork
{
    public class MazeRunnerBody : MonoBehaviour
    {
        [SerializeField] private MazeRunner agent;
        [SerializeField] private GameObject turnOff;
        [SerializeField] private SpriteRenderer spriteRenderer;

        private void OnCollisionEnter2D(Collision2D collision)
        {
            Die(collision.relativeVelocity.magnitude);
        }

        private void Die(float impact)
        {
            agent.Die(transform.localPosition.x, impact);
            float f = MazePopulation.CompareToBestFitness(agent.Fitness);
            spriteRenderer.color = new Color(f, f, f);
            Destroy(turnOff);
        }
    }
}