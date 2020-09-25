using System;
using UnityEditor;
using UnityEngine;

public class Population : MonoBehaviour
{
    public bool PauseAtEndOfRound;
    public bool LoadSaveFile;
    public bool SaveOnBetterFitness;
    [SerializeField] private SaveSO saveFile;
    [SerializeField] private int inputNodes;
    [SerializeField] [Tooltip("Usually between input and output values")] private int hiddenNodes;
    [SerializeField] private int outputNodes;
    [SerializeField] [Range(0f, .02f)] private float mutationRate = .01f;
    [Tooltip("How many creatures should be created by reproduction, instead of randomly")]
    [SerializeField] [Range(.9f, 1f)] private float procriationPercentage;
    [SerializeField] private bool randomizeStartingHeights = false;
    [SerializeField] [Range(1, 6)] private int startingHeight;

    [SerializeField] private int numberOfCreatures;
    [Space(10)]
    [SerializeField] private GameObject creaturePrefab;
    [SerializeField] private GameObject platformPrefab;
    [SerializeField] private Transform creaturesParent;
    [SerializeField] [Tooltip("Build by clicking on the context menu cog")] private Transform mazeParent;
    public const float DistanceBetweenPlataforms = 4f;

    private int generation = 0;
    public static int MaximumLife = 500;
    public static int AmountDeadByAge = 0;
    private const int minDeadByAge = 3;

    private Creature[] creatures;

    #region Natural Selection
    private float[] fitness;
    private float fitnessSum;
    private static float bestFitness;
    private NeuralNetwork[] tempNeuralNets;
    #endregion

    /// <summary>Skips a frame in between destroying and rebuilding everything</summary>
    private bool skipFrame = false;

    [ContextMenu("Build")]
    public void Build()
    {
        for (int i = 0; i < numberOfCreatures; i++)
            Instantiate(platformPrefab, new Vector3(DistanceBetweenPlataforms * (i - numberOfCreatures / 2), 0, 0), Quaternion.identity, mazeParent);
    }

    private void Awake()
    {
        creatures = new Creature[numberOfCreatures];
        fitness = new float[numberOfCreatures];
    }

    private void Start()
    {
        GameObject go;
        for (int i = 0; i < creatures.Length; i++) {
            if (randomizeStartingHeights)
                startingHeight = UnityEngine.Random.Range(1, 6);
            go = Instantiate(creaturePrefab, new Vector3(DistanceBetweenPlataforms * (i - numberOfCreatures / 2), startingHeight, 0), Quaternion.identity, creaturesParent);
            go.name = "Creature " + i;
            creatures[i] = go.GetComponent<Creature>();
            if (LoadSaveFile && saveFile.Fitness > 0) {
                saveFile.Load(out float[,] WIH, out float[,] WHO);
                creatures[i].Setup(new NeuralNetwork(WIH, WHO));
            }
            else
                creatures[i].Setup(inputNodes, hiddenNodes, outputNodes);
        }
    }

    private void LateUpdate()
    {
        if (skipFrame) {
            KillCreatures();
            NewPopulation();
            Mutate();
            AmountDeadByAge = 0;
            skipFrame = false;
        }
        else {
            if (!AllDead())
                return;
            skipFrame = true;
            generation++;
            if (AmountDeadByAge > minDeadByAge)
                MaximumLife += 100;
            CalculateFitness();
            NaturalSelection();
            Debug.Log($"Gen {generation} - Avg {fitnessSum / numberOfCreatures} - Best {bestFitness} - Age {AmountDeadByAge}");
            TryToSave();
            if (PauseAtEndOfRound) EditorApplication.isPaused = true;
        }
    }

    private void TryToSave()
    {
        if (!SaveOnBetterFitness || creatures[0].Fitness < saveFile.Fitness)
            return;
        saveFile.Save(creatures[0].Fitness, creatures[0].NeuralNet.WeightInputHidden, creatures[0].NeuralNet.WeightHiddenOutput);
        Debug.Log("<color=blue>Saved with Fitness </color>" + creatures[0].Fitness);
    }

    private void CalculateFitness()
    {
        fitnessSum = 0;
        bestFitness = 0;
        Array.Sort(creatures);
        Array.Reverse(creatures);
        for (int i = 0; i < numberOfCreatures; i++) {
            fitness[i] = creatures[i].Fitness;
            fitnessSum += fitness[i];
            if (fitness[i] > bestFitness)
                bestFitness = fitness[i];
        }
    }

    private void NaturalSelection()
    {
        tempNeuralNets = new NeuralNetwork[numberOfCreatures];
        float[] targets = new float[numberOfCreatures];
        float sum = 0;
        for (int i = 0; i < numberOfCreatures; i++)
            targets[i] = UnityEngine.Random.Range(0f, fitnessSum);
        Array.Sort(targets);
        int currentCreature = 0;
        for (int j = 0; j < numberOfCreatures; j++) {
            sum += fitness[j];
            while (sum >= targets[currentCreature]) {
                tempNeuralNets[currentCreature] = creatures[j].GetBrain();
                currentCreature++;
                if (currentCreature >= numberOfCreatures)
                    return;
            }
        }
    }

    private void KillCreatures()
    {
        for (int i = 0; i < creatures.Length; i++)
            if (creatures[i] != null) Destroy(creatures[i].gameObject);
    }

    private void NewPopulation()
    {
        GameObject go;
        for (int i = 0; i < creatures.Length; i++) {
            if (randomizeStartingHeights)
                startingHeight = UnityEngine.Random.Range(1, 6);
            go = Instantiate(creaturePrefab, new Vector3(DistanceBetweenPlataforms * (i - numberOfCreatures / 2), startingHeight, 0), Quaternion.identity, creaturesParent);
            go.name = "Creature " + i;
            creatures[i] = go.GetComponent<Creature>();
            if ((float)i / creatures.Length > procriationPercentage)
                creatures[i].Setup(inputNodes, hiddenNodes, outputNodes);
            else
                creatures[i].Setup(tempNeuralNets[i]);
        }
    }

    private void Mutate()
    {
        for (int i = 5; i < numberOfCreatures; i++)
            creatures[i].Mutate(mutationRate);
    }

    private bool AllDead()
    {
        for (int i = numberOfCreatures - 1; i >= 0; i--)
            if (!creatures[i].Dead)
                return false;
        return true;
    }

    public static float ComparedToBestFitness(float value) => value / bestFitness;
}