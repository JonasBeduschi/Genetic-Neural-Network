using GeneticNeuralNetwork;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[SelectionBase]
public class MazeRunner : Agent
{
    private int lifeCounter = 0;

    private List<Vector2> previousPositions = new List<Vector2>();

    public Transform bodyTransform;
    public int numberOfLasers;
    public int memoryLength;
    public int memoriesToConsider;
    private float posY, posX;
    private static int totalNumberOfMemories = -1;
    public Transform[] lasers;
    public LayerMask mazeLayer;
    private Rigidbody2D headRB;

    private const float strengthAdjustment = 100f;
    private const float historyCap = 10f;
    private const float velocityCap = .1f;
    public const float EyeDistanceCap = 20f;
    private const float creatureSize = 1;
    private RaycastHit2D[] hits = new RaycastHit2D[1];
    private ContactFilter2D filter;

    protected override void SetupAgent()
    {
        if (totalNumberOfMemories == -1)
            totalNumberOfMemories = 3;
        filter = new ContactFilter2D
        {
            layerMask = mazeLayer,
            useLayerMask = true,
            maxDepth = transform.position.z + 1,
            minDepth = transform.position.z - 1,
            useDepth = true
        };
        headRB = bodyTransform.GetComponent<Rigidbody2D>();
        for (int i = 0; i < memoryLength; i++)
            previousPositions.Add(bodyTransform.position);
        GetInputs();
    }

    private void FixedUpdate()
    {
        if (Dead)
            return;
        output = NeuralNet.Query(GetInputs());
        Move();
        lifeCounter += 1;
        if (lifeCounter >= Population.MaximumLife) {
            Population.AmountDeadByAge++;
            Die(bodyTransform.localPosition.x, 0);
        }
    }

    protected override float[] GetInputs()
    {
        posY = bodyTransform.position.y;
        posX = bodyTransform.position.x;

        // Bias
        GetInput()[0] = 1;

        // Get position delta (instant velocity)
        GetInput()[1] = (posX - previousPositions[memoryLength - 1].x) / velocityCap;
        GetInput()[2] = (posY - previousPositions[memoryLength - 1].y) / velocityCap;

        // Save this position
        previousPositions.Add(bodyTransform.position);
        while (previousPositions.Count > memoryLength)
            previousPositions.RemoveAt(0);

        // Get some relative history
        for (int i = 0; i < memoriesToConsider; i++) {
            GetInput()[3 + i * 2] = (posX - previousPositions[memoryLength / memoriesToConsider * i].x) / historyCap;
            GetInput()[4 + i * 2] = (posY - previousPositions[memoryLength / memoriesToConsider * i].y) / historyCap;
        }

        // Get lasers
        for (int i = 0; i < lasers.Length; i++)
            GetInput()[3 + memoriesToConsider * 2 + i] = GetDistanceFromEye(lasers[i]);

        return GetInput();
    }

    private float GetDistanceFromEye(Transform laser)
    {
        if (Physics2D.Raycast(laser.position, laser.right, filter, hits, EyeDistanceCap) > 0)
            return hits[0].distance - creatureSize / 2;
        return EyeDistanceCap;
    }

    private Vector3 GetHitPoint(Transform laser)
    {
        if (!Application.isPlaying)
            filter = new ContactFilter2D
            {
                layerMask = mazeLayer,
                useLayerMask = true,
                maxDepth = transform.position.z + 1,
                minDepth = transform.position.z - 1,
                useDepth = true
            };
        if (Physics2D.Raycast(laser.position, laser.right, filter, hits, EyeDistanceCap) > 0)
            return hits[0].point;
        return laser.position;
    }

    private void Move()
    {
        headRB.AddForce(new Vector2(GetOutput()[0], GetOutput()[1]) * strengthAdjustment, ForceMode2D.Force);
    }

    public void Die(float bodyPositionX, float impact)
    {
        if (Dead)
            return;
        Dead = true;
        bodyTransform.GetComponent<MazeRunnerBody>().Die();
        SetFitness(bodyPositionX, impact);
    }

    private void SetFitness(float bodyPositionX, float impact)
    {
        if (bodyPositionX <= 0) {
            Fitness = 0;
            return;
        }
        impact += 1;
        bodyPositionX += 1;
        Fitness = Mathf.Pow(bodyPositionX, 2) / impact / Mathf.Sqrt(lifeCounter);
    }

    private void OnDrawGizmosSelected()
    {
        if (!Selection.activeGameObject.Equals(gameObject))
            return;

        // Memories
        Gizmos.color = Color.cyan;
        if (previousPositions.Count > 0) {
            for (int i = 0; i < memoriesToConsider; i++) {
                Gizmos.DrawWireSphere(
                new Vector3(previousPositions[memoryLength / memoriesToConsider * i].x,
                previousPositions[memoryLength / memoriesToConsider * i].y,
                0),
                .5f);
            }
        }

        // Lasers
        Gizmos.color = Color.yellow;
        for (int i = 0; i < lasers.Length; i++) {
            if (lasers[i] != null) {
                Gizmos.DrawLine(lasers[i].position, GetHitPoint(lasers[i]));
            }
        }
        if (!Application.isPlaying)
            return;

        // Force
        Gizmos.color = Color.magenta;
        Vector3 direction = new Vector3(GetOutput()[0], GetOutput()[1], 0).normalized * .5f;
        Gizmos.DrawLine(bodyTransform.position, bodyTransform.position + direction + direction * 2f);
    }
}