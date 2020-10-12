using System;
using UnityEditor;
using UnityEngine;

public class Population : MonoBehaviour
{
    private enum PauseCondition { None, EveryGeneration, OnDeathByAge, OnBestFitness }
    [SerializeField] private PauseCondition pauseCondition = PauseCondition.None;
    [Range(-1, 4)] public int LoadSaveFile;
    public bool SaveOnBetterFitness;
    [SerializeField] private int inputNodes;
    [SerializeField] [Tooltip("Usually between input and output values")] private int hiddenNodes;
    [SerializeField] private int outputNodes;
    [SerializeField] [Range(0, 100)] private int creatureMutationRate = 5;
    [SerializeField] [Range(0f, .02f)] private float connectionMutationRate = .01f;
    [SerializeField] [Range(0f, .5f)] private float mutationAmount = .2f;
    [Tooltip("How many creatures should be created by reproduction, instead of randomly")]
    [SerializeField] [Range(.9f, 1f)] private float procriationPercentage;
    [SerializeField] private bool randomizeStartingHeights = false;
    [SerializeField] [Range(1, 6)] private int startingHeight;

    [SerializeField] private int numberOfCreatures;
    [Space(10)]
    [SerializeField] private GameObject creatureOriginal;
    [SerializeField] private Transform creaturesParent;
    [SerializeField] [Tooltip("Build by clicking on the context menu cog")] private Transform mazeParent;
    public const float DistanceBetweenPlataforms = 4f;

    private int generation = 0;
    public static int MaximumLife = 600;
    public static int AmountDeadByAge = 0;
    private const int minDeadByAge = 2000;

    private Creature[] creatures;

    #region Natural Selection
    private float[] fitness;
    private float fitnessSum;
    private static float bestFitness;
    private NeuralNetwork[] tempNeuralNets;
    private float oldAverage = 0;
    #endregion

    [SerializeField] [Range(0, 4)] private int mapNumber;
    [SerializeField] private SaveSO[] saveFiles;
    [SerializeField] private GameObject[] platformPrefabs;
    /// <summary>Skips a frame in between destroying and rebuilding everything</summary>
    private bool skipFrame = false;

    [ContextMenu("Build")]
    public void Build()
    {
        int n = mazeParent.childCount;
        for (int i = n - 1; i >= 0; i--)
            DestroyImmediate(mazeParent.GetChild(i).gameObject);

        for (int i = 0; i < numberOfCreatures; i++)
            Instantiate(platformPrefabs[mapNumber], new Vector3(DistanceBetweenPlataforms * (i - numberOfCreatures / 2), 0, 0), Quaternion.identity, mazeParent);
    }

    private void Awake()
    {
        creatures = new Creature[numberOfCreatures];
        fitness = new float[numberOfCreatures];
        creatureOriginal.SetActive(false);
        Creature c = creatureOriginal.GetComponent<Creature>();
        inputNodes = 2 + c.numberOfLasers + c.memoriesToConsider * 2;
        hiddenNodes = inputNodes;
    }

    private void Start()
    {
        GameObject go;
        for (int i = 0; i < creatures.Length; i++) {
            if (randomizeStartingHeights)
                startingHeight = UnityEngine.Random.Range(1, 6);
            go = Instantiate(creatureOriginal, new Vector3(DistanceBetweenPlataforms * (i - numberOfCreatures / 2), startingHeight, 0), Quaternion.identity, creaturesParent);
            go.name = "Creature " + i;
            go.SetActive(true);
            creatures[i] = go.GetComponent<Creature>();
            if (LoadSaveFile >= 0 && saveFiles[mapNumber].Fitness > 0) {
                saveFiles[mapNumber].Load(out float[,] WIH, out float[,] WHO);
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
            if (oldAverage > fitnessSum / numberOfCreatures)
                Debug.Log($"Gen {generation} - Avg <color=red>{fitnessSum / numberOfCreatures}</color> - Best {bestFitness} - Age {AmountDeadByAge}");
            else
                Debug.Log($"Gen {generation} - Avg <color=green>{fitnessSum / numberOfCreatures}</color> - Best {bestFitness} - Age {AmountDeadByAge}");
            oldAverage = fitnessSum / numberOfCreatures;
            bool saved = TryToSave();
            switch (pauseCondition) {
                case PauseCondition.EveryGeneration:
                    EditorApplication.isPaused = true;
                    break;
                case PauseCondition.OnDeathByAge:
                    if (AmountDeadByAge > 0)
                        EditorApplication.isPaused = true;
                    break;
                case PauseCondition.OnBestFitness:
                    if (saved)
                        EditorApplication.isPaused = true;
                    break;
                default:
                    break;
            }
        }
    }

    private bool TryToSave()
    {
        if (!SaveOnBetterFitness || creatures[0].Fitness * .999f < saveFiles[mapNumber].Fitness)
            return false;
        saveFiles[mapNumber].Save(creatures[0].Fitness, creatures[0].NeuralNet.WeightInputHidden, creatures[0].NeuralNet.WeightHiddenOutput);
        Debug.Log("<color=blue>Saved with Fitness </color>" + creatures[0].Fitness);
        return true;
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
            go = Instantiate(creatureOriginal, new Vector3(DistanceBetweenPlataforms * (i - numberOfCreatures / 2), startingHeight, 0), Quaternion.identity, creaturesParent);
            go.name = "Creature " + i;
            go.SetActive(true);
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
            if (creatureMutationRate > UnityEngine.Random.Range(0, 100))
                creatures[i].Mutate(connectionMutationRate, mutationAmount);
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