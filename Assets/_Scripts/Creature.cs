using System;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Creature : MonoBehaviour, IComparable
{
    public NeuralNetwork NeuralNet { get; private set; }
    private int lifeCounter = 0;

    public float Fitness = 0;
    public float[] inputs;
    private float[] movement;
    private List<Vector3> previousPositions = new List<Vector3>();

    public Transform bodyTransform;
    public int numberOfLasers;
    public int memoryLength;
    public int memoriesToConsider;
    float posY, posZ;
    private static int totalNumberOfMemories = -1;
    public Transform[] lasers;
    public LayerMask layer;
    private Rigidbody headRB;

    private const float strengthAdjustment = .3f;
    private Ray ray;
    private RaycastHit hit;

    public void Setup(int inputNodes, int hiddenNodes, int outputNodes)
    {
        headRB = bodyTransform.GetComponent<Rigidbody>();
        if (totalNumberOfMemories == -1) {

            totalNumberOfMemories = 3;
        }
        NeuralNet = new NeuralNetwork(inputNodes, hiddenNodes, outputNodes);
        inputs = new float[inputNodes];
        for (int i = 0; i < memoryLength; i++)
            previousPositions.Add(bodyTransform.position);
        GetInputs();
    }

    public void Setup(NeuralNetwork newNeuralNet)
    {
        headRB = bodyTransform.GetComponent<Rigidbody>();
        NeuralNet = new NeuralNetwork(newNeuralNet);
        inputs = new float[NeuralNet.GetInputLength()];
        for (int i = 0; i < memoryLength; i++)
            previousPositions.Add(bodyTransform.position);
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
            Die(bodyTransform.position.z, 0);
        }
    }

    private float[] GetInputs()
    {
        posY = bodyTransform.position.y;
        posZ = bodyTransform.position.z;

        // Get delta position (instant velocity)
        inputs[0] = posY - previousPositions[memoryLength - 1].y;
        inputs[1] = posZ - previousPositions[memoryLength - 1].z;

        //Save this position
        previousPositions.Add(bodyTransform.position);
        while (previousPositions.Count > memoryLength)
            previousPositions.RemoveAt(0);

        // Get some relative history
        for (int i = 0; i < memoriesToConsider; i++) {
            inputs[2 + i * 2] = posY - previousPositions[memoryLength / memoriesToConsider * i].y;
            inputs[3 + i * 2] = posZ - previousPositions[memoryLength / memoriesToConsider * i].z;
        }

        // Get lasers
        for (int i = 0; i < lasers.Length; i++)
            inputs[2 + memoriesToConsider * 2 + i] = GetDistanceFromEye(lasers[i]);

        return inputs;
    }

    private float GetDistanceFromEye(Transform laser)
    {
        ray = new Ray(laser.position, laser.forward);
        if (Physics.Raycast(ray, out hit, float.MaxValue, layer))
            return hit.distance - .5f;
        return float.MaxValue;
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

    public void Mutate(float mutationRate, float mutationAmount) => NeuralNet.Mutate(mutationRate, mutationAmount);

    public void Die(float headPositionZ, float impact)
    {
        if (Dead)
            return;
        Dead = true;
        if (impact == 0)
            impact = -50;
        Destroy(bodyTransform.GetComponent<Rigidbody>());
        Destroy(bodyTransform.GetComponent<BodyController>());
        if (headPositionZ <= 0) {
            Fitness = 0;
            return;
        }
        headPositionZ += 1;
        Fitness = (headPositionZ.Power(2) / ((lifeCounter / 2 + 100) * (impact + 100))) * 100;
    }

    public NeuralNetwork GetBrain() => NeuralNet.Clone();

    public int CompareTo(object obj) => Fitness.CompareTo(((Creature)obj).Fitness);

    public bool Dead { get; private set; } = false;

    private void OnDrawGizmosSelected()
    {
        if (Dead)
            return;
        Gizmos.color = Color.cyan;
        if (previousPositions.Count > 0) {
            for (int i = 0; i < memoriesToConsider; i++) {
                Gizmos.DrawWireSphere(
                new Vector3(0,
                previousPositions[memoryLength / memoriesToConsider * i].y,
                previousPositions[memoryLength / memoriesToConsider * i].z),
                .5f);
            }
        }
        Gizmos.color = Color.yellow;
        for (int i = 0; i < lasers.Length; i++)
            Gizmos.DrawLine(lasers[i].position, GetHitPoint(lasers[i]));
    }
}