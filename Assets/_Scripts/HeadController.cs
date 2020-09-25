using UnityEngine;

public class HeadController : MonoBehaviour
{
    [SerializeField] private Creature creature;
    [SerializeField] private GameObject turnOn;
    [SerializeField] private GameObject turnOff;
    [SerializeField] private Material deadMaterial;
    [SerializeField] private MeshRenderer deadRenderer;
    private bool dead = false;

    private void FixedUpdate()
    {
        if (dead)
            return;
        if (transform.position.y < -2f)
            Die(400);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.tag.Equals("Floor"))
            Die(collision.relativeVelocity.sqrMagnitude);
    }

    private void Die(float impact)
    {
        creature.Die(transform.position.z, impact);
        turnOn.SetActive(true);
        float f = Population.ComparedToBestFitness(creature.Fitness);
        Color color = new Color(f, f, f);
        deadRenderer.material = new Material(deadMaterial);
        deadRenderer.material.color = color;
        Destroy(turnOff);
        dead = true;
    }
}