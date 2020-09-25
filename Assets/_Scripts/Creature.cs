using System;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Creature : MonoBehaviour, IComparable
{
    public NeuralNetwork NeuralNet { get; private set; }
    private int lifeCounter = 0;

    public float Fitness = 0;
    private float[] inputs;
    private float[] movement;
    private List<Vector3> previousPositions = new List<Vector3>();
    private const int maxMemory = 100;

    [SerializeField] private Transform headTranform;
    [SerializeField] private Transform[] lasers;
    [SerializeField] private LayerMask layer;
    [SerializeField] private Rigidbody headRB;

    private const float strengthAdjustment = .01f;
    private Ray ray;
    private RaycastHit hit;

    public void Setup(int inputNodes, int hiddenNodes, int outputNodes)
    {
        NeuralNet = new NeuralNetwork(inputNodes, hiddenNodes, outputNodes);
        inputs = new float[inputNodes];
        for (int i = 0; i < maxMemory; i++)
            previousPositions.Add(headTranform.position);
        GetInputs();
    }

    public void Setup(NeuralNetwork newNeuralNet)
    {
        NeuralNet = new NeuralNetwork(newNeuralNet);
        inputs = new float[NeuralNet.GetInputLength()];
        for (int i = 0; i < maxMemory; i++)
            previousPositions.Add(headTranform.position);
        GetInputs();
    }

    private void FixedUpdate()
    {
        if (Dead)
            return;
        movement = NeuralNet.Query(GetInputs());
        Move();
        lifeCounter += 1;
        if (lifeCounter >= Population.MaximumLife) {
            Population.AmountDeadByAge++;
            Die(headTranform.position.z, 1);
        }
    }

    private float[] GetInputs()
    {
        // Get delta position (instant velocity)
        inputs[0] = headTranform.position.y - previousPositions[0].y;
        inputs[1] = headTranform.position.z - previousPositions[0].z;

        //Save this position
        previousPositions.Add(headTranform.position);
        while (previousPositions.Count > maxMemory)
            previousPositions.RemoveAt(0);

        // Get some relative history
        inputs[2] = headTranform.position.y - previousPositions[maxMemory / 2 - 1].y;
        inputs[3] = headTranform.position.z - previousPositions[maxMemory / 2 - 1].z;
        inputs[4] = headTranform.position.y - previousPositions[0].y;
        inputs[5] = headTranform.position.z - previousPositions[0].z;

        // Get lasers
        for (int i = 0; i < lasers.Length; i++)
            inputs[6 + i] = GetDistanceFromEye(lasers[i]);

        return inputs;
    }

    private float GetDistanceFromEye(Transform laser)
    {
        ray = new Ray(laser.position, laser.forward);
        if (Physics.Raycast(ray, out hit, float.MaxValue, layer))
            return hit.distance;
        return -1;
    }

    private Vector3 GetHitPoint(Transform laser)
    {
        ray = new Ray(laser.position, laser.forward);
        if (Physics.Raycast(ray, out hit, float.MaxValue, layer))
            return hit.point;
        return Vector3.zero;
    }

    private void Move()
    {
        headRB.AddForce(new Vector3(0, movement[0], movement[1]) * strengthAdjustment, ForceMode.Impulse);
    }

    public void Mutate(float mutationRate) => NeuralNet.Mutate(mutationRate);

    public void Die(float headPositionZ, float impact)
    {
        if (Dead)
            return;
        Dead = true;
        Destroy(headTranform.GetComponent<Rigidbody>());
        if (headPositionZ <= 0) {
            Fitness = 0;
            return;
        }
        headPositionZ += 1;
        Fitness = headPositionZ.Power(2) / ((lifeCounter + 100) * (impact + 100));
    }

    public NeuralNetwork GetBrain() => NeuralNet.Clone();

    public int CompareTo(object obj) => Fitness.CompareTo(((Creature)obj).Fitness);

    public bool Dead { get; private set; } = false;

    private void OnDrawGizmosSelected()
    {
        if (Dead)
            return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(
            new Vector3(0, previousPositions[maxMemory / 2 - 1].y, previousPositions[maxMemory / 2 - 1].z), .5f);
        Gizmos.DrawWireSphere(
            new Vector3(0, previousPositions[0].y, previousPositions[0].z), .5f);
        Gizmos.color = Color.yellow;
        for (int i = 0; i < lasers.Length; i++)
            Gizmos.DrawLine(lasers[i].position, GetHitPoint(lasers[i]));
    }
}