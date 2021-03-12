using UnityEngine;
using GeneticNeuralNetwork;

[CreateAssetMenu(fileName = "New Save File", menuName = "New Save")]
public class SaveSO : ScriptableObject
{
    // Exists just to be visible in the Inspector
    [SerializeField] private float fitness;
    public float Fitness { get => fitness; private set => fitness = value; }
    [SerializeField] private float[] arrayOfWeights;
    [SerializeField] private int[] rows;
    [SerializeField] private int[] collums;

    /// <summary>Saves a NN weights matrix</summary>
    /// <param name="fitness">The fitness of the NN creature, useful for saving when a better one is found</param>
    /// <param name="originalWeights">The NN weights to be saved</param>
    public void Save(float fitness, Matrix[] originalWeights)
    {
        Fitness = fitness;
        rows = new int[originalWeights.Length];
        collums = new int[originalWeights.Length];

        for (int i = 0; i < originalWeights.Length; i++) {
            rows[i] = originalWeights[i].Rows;
            collums[i] = originalWeights[i].Collums;
        }

        int totalSize = 0;
        for (int i = 0; i < originalWeights.Length; i++) {
            totalSize += rows[i] * collums[i];
        }
        arrayOfWeights = new float[totalSize];

        int count = 0;
        for (int i = 0; i < originalWeights.Length; i++) {
            for (int row = 0; row < rows[i]; row++) {
                for (int collum = 0; collum < collums[i]; collum++) {
                    arrayOfWeights[count] = originalWeights[i][row, collum];
                    count++;
                }
            }
        }
    }

    /// <summary>Loads the NN weights from the save file</summary>
    /// <param name="weights">Where to load the weights on</param>
    public void Load(out Matrix[] weights)
    {
        weights = new Matrix[rows.Length];

        int count = 0;
        for (int i = 0; i < weights.Length; i++) {
            weights[i] = new Matrix(rows[i], collums[i]);
            for (int row = 0; row < rows[i]; row++) {
                for (int collum = 0; collum < collums[i]; collum++) {
                    weights[i][row, collum] = arrayOfWeights[count];
                    count++;
                }
            }
        }
    }

    [ContextMenu("Reset Item")]
    public void Reset()
    {
        Fitness = 0;
        arrayOfWeights = System.Array.Empty<float>();
        rows = System.Array.Empty<int>();
        collums = System.Array.Empty<int>();
    }
}