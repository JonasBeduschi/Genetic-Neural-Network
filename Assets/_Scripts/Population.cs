using System;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GeneticNeuralNetwork
{
    public class Population : MonoBehaviour
    {
        private enum PauseCondition { None, EveryGeneration, OnDeathByAge, OnBestFitness }

        [Header("Simulation")]
        [SerializeField] private PauseCondition pauseCondition = PauseCondition.None;

        [Range(-1, 4)] public int LoadSaveFile;
        public bool SaveOnBetterFitness;
        [SerializeField] private bool autoselectFirstAgent;
        [SerializeField] private int numberOfAgents;
        public int NumberOfAgents { get => numberOfAgents; }

        [Header("Neural Network")]
        [SerializeField] private int inputNodes;

        [SerializeField] [Tooltip("Usually between input and output values")] private int[] hiddenNodes;
        [SerializeField] private int outputNodes;

        [Header("Natural Selection")]
        [SerializeField] [Range(0f, .2f)] private float agentMutationRate = .05f;

        [SerializeField] [Range(0f, .02f)] private float neuronMutationRate = .01f;
        [SerializeField] [Range(0f, .5f)] private float mutationAmount = .2f;

        [Tooltip("How many creatures should be created by reproduction, instead of randomly")]
        [SerializeField] [Range(.9f, 1f)] private float procriationPercentage;

        [SerializeField] private bool randomizeStartingHeights = false;
        private const int minStartingHeight = -1;
        private const int maxStartingHeight = 7;
        [SerializeField] [Range(minStartingHeight, maxStartingHeight)] private int startingHeight;

        [Header("References")]
        [SerializeField] private GameObject originalAgent;

        [SerializeField] private Transform agentParent;

        public const float DistanceBetweenPlataformsX = 130f;
        public const float DistanceBetweenPlataformsY = 11f;
        public const int NumberOfCollums = 5;

        private int generation = 0;
        public static int MaximumLife = 600;
        public static int AmountDeadByAge = 0;
        private const int minDeadByAge = 2000;

        private MazeRunner[] creatures;

        #region Natural Selection

        private float[] fitness;
        private float fitnessSum;
        private static float bestFitness;
        private NeuralNetwork[] nextPopulation;
        private float oldAverage = 0;

        #endregion Natural Selection

        [Space(5)]
        [SerializeField] private SaveSO[] saveFiles;

        private int mapNumber;

        /// <summary>Skips a frame in between destroying and rebuilding everything</summary>
        private bool skipFrame = false;

        private void Awake()
        {
            mapNumber = GetComponent<MazeBuilder>().MapNumber;
            creatures = new MazeRunner[numberOfAgents];
            fitness = new float[numberOfAgents];
            originalAgent.SetActive(false);
            MazeRunner c = originalAgent.GetComponent<MazeRunner>();
            inputNodes = 2 + c.numberOfLasers + c.memoriesToConsider * 2;
            for (int i = 0; i < hiddenNodes.Length; i++)
                if (hiddenNodes[i] <= 0)
                    hiddenNodes[i] = outputNodes;
        }

        private void Start()
        {
            for (int i = 0; i < creatures.Length; i++) {
                InstantiateAgent(i);
                if (LoadSaveFile >= 0 && saveFiles[mapNumber].Fitness > 0) {
                    saveFiles[mapNumber].Load(out Matrix[] weights);
                    creatures[i].SetupNN(new NeuralNetwork(weights));
                }
                else
                    SetupNewAgent(creatures[i]);
            }
        }

        private void FixedUpdate()
        {
            if (skipFrame)
                SetupNewGeneration();
            else if (AllDead()) {
                skipFrame = true;
                generation++;
                if (AmountDeadByAge > minDeadByAge)
                    MaximumLife += 100;
                CalculateFitness();
                NaturalSelection();
                LogGenerationFitness();
                oldAverage = fitnessSum / numberOfAgents;
                TryToPause(TryToSave());
            }
        }

        private void LogGenerationFitness()
        {
            if (oldAverage > fitnessSum / numberOfAgents)
                Debug.Log($"{Time.time} - Gen {generation} - Avg <color=red>{fitnessSum / numberOfAgents}</color> - Best {bestFitness} - Age {AmountDeadByAge}");
            else
                Debug.Log($"{Time.time} - Gen {generation} - Avg <color=green>{fitnessSum / numberOfAgents}</color> - Best {bestFitness} - Age {AmountDeadByAge}");
        }

        private void SetupNewGeneration()
        {
            KillCreatures();
            NewPopulation();
            Mutate();
            AmountDeadByAge = 0;
            skipFrame = false;
        }

        private bool TryToSave()
        {
            if (!SaveOnBetterFitness)
                return false;
            if (creatures[0].Fitness <= saveFiles[mapNumber].Fitness)
                return false;
            saveFiles[mapNumber].Save(creatures[0].Fitness, creatures[0].NeuralNet.Weights);
            Debug.Log("<color=blue>Saved with Fitness </color>" + creatures[0].Fitness);
            return true;
        }

        private void TryToPause(bool agentWasSaved)
        {
            switch (pauseCondition) {
                case PauseCondition.EveryGeneration:
                    EditorApplication.isPaused = true;
                    break;

                case PauseCondition.OnDeathByAge:
                    if (AmountDeadByAge > 0)
                        EditorApplication.isPaused = true;
                    break;

                case PauseCondition.OnBestFitness:
                    if (agentWasSaved)
                        EditorApplication.isPaused = true;
                    break;

                default:
                    break;
            }
        }

        private void CalculateFitness()
        {
            fitnessSum = 0;
            bestFitness = 0;
            Array.Sort(creatures);
            Array.Reverse(creatures);
            for (int i = 0; i < numberOfAgents; i++) {
                fitness[i] = creatures[i].Fitness;
                fitnessSum += fitness[i];
                if (fitness[i] > bestFitness)
                    bestFitness = fitness[i];
            }
        }

        private void NaturalSelection()
        {
            NeuralNetwork[] temp = new NeuralNetwork[numberOfAgents];
            float[] targetFitness = new float[numberOfAgents];
            float sum = 0;
            for (int i = 0; i < numberOfAgents; i++)
                targetFitness[i] = Random.Range(0f, fitnessSum);
            Array.Sort(targetFitness);
            temp[0] = creatures[0].GetBrain();
            int currentAgent = 1;
            for (int j = 0; j < numberOfAgents; j++) {
                sum += fitness[j];
                while (sum >= targetFitness[currentAgent]) {
                    temp[currentAgent] = creatures[j].GetBrain();
                    currentAgent++;
                    if (currentAgent >= numberOfAgents) {
                        Breeding(temp);
                        return;
                    }
                }
            }
            Breeding(temp);
        }

        private void Breeding(NeuralNetwork[] temp)
        {
            int length = temp.Length;
            nextPopulation = new NeuralNetwork[length];
            // Always skip the first (best) one
            nextPopulation[0] = new NeuralNetwork(temp[0]);
            for (int i = 1; i < length; i++)
                nextPopulation[i] = new NeuralNetwork(temp[Random.Range(0, length)], temp[Random.Range(0, length)]);
        }

        private void KillCreatures()
        {
            for (int i = 0; i < creatures.Length; i++)
                if (creatures[i] != null) Destroy(creatures[i].gameObject);
        }

        private void NewPopulation()
        {
            for (int i = 0; i < creatures.Length; i++) {
                InstantiateAgent(i);
                if ((float)i / creatures.Length < procriationPercentage)
                    creatures[i].SetupNN(nextPopulation[i]);
                else
                    SetupNewAgent(creatures[i]);
            }
        }

        private void SetupNewAgent(MazeRunner agent)
        {
            agent.SetupNN(inputNodes, hiddenNodes, outputNodes);
        }

        private void InstantiateAgent(int agentIndex)
        {
            if (randomizeStartingHeights)
                startingHeight = Random.Range(minStartingHeight, maxStartingHeight + 1);

            int agentsPerCollum = numberOfAgents / NumberOfCollums;

            float x = DistanceBetweenPlataformsX * (agentIndex / agentsPerCollum);
            float y = (agentIndex % agentsPerCollum) * DistanceBetweenPlataformsY + startingHeight;

            GameObject go = Instantiate(originalAgent, new Vector3(x, y, 0), Quaternion.identity, agentParent);

            go.name = "Agent " + agentIndex;
            go.SetActive(true);
            creatures[agentIndex] = go.GetComponent<MazeRunner>();

#if UNITY_EDITOR
            if (agentIndex != 0)
                return;
            if (Selection.activeObject == null || Selection.activeObject.name == "Agent 0")
                Selection.activeGameObject = go;
#endif
        }

        private void Mutate()
        {
            for (int i = 5; i < numberOfAgents; i++)
                if (agentMutationRate > Random.Range(0f, 1f))
                    creatures[i].Mutate(neuronMutationRate, mutationAmount);
        }

        private bool AllDead()
        {
            for (int i = 0; i < numberOfAgents; i++)
                if (!creatures[i].Dead)
                    return false;
            return true;
        }

        public static float CompareToBestFitness(float value) => value / bestFitness;
    }
}