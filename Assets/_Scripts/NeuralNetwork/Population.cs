using System;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GeneticNeuralNetwork
{
    public abstract class Population : MonoBehaviour
    {
        protected enum PauseCondition { None, EveryGeneration, OnDeathByAge, OnBestFitness }

        [Header("Simulation")]
        [SerializeField] protected PauseCondition pauseCondition = PauseCondition.None;

        [Range(-1, 4)] public int LoadSaveFile;
        /// <summary>
        /// 
        /// </summary>
        public bool SaveOnBetterFitness;
        [SerializeField] protected bool autoselectFirstAgent;
        [SerializeField] protected int numberOfAgents;
        public int NumberOfAgents { get => numberOfAgents; }

        [Header("Neural Network")]
        [SerializeField] protected int inputNodes;

        [SerializeField] protected int[] hiddenNodes;
        [SerializeField] protected int outputNodes;

        [Header("Natural Selection")]
        [SerializeField] [Range(0f, .2f)] protected float agentMutationRate = .05f;

        [SerializeField] [Range(0f, .02f)] protected float neuronMutationRate = .01f;
        [SerializeField] [Range(0f, .5f)] protected float mutationAmount = .2f;

        /// <summary>How many creatures should be created by reproduction, instead of randomly</summary>
        [SerializeField] [Range(.9f, 1f)] protected float procriationPercentage;

        [Header("References")]
        [SerializeField] protected GameObject originalAgent;

        [SerializeField] protected Transform agentParent;

        [Space(5)]
        [SerializeField] protected SaveSO[] saveFiles;

        protected int mapNumber;

        protected int generation = 0;
        public static int MaximumLife = 600;
        public static int AmountDeadByAge = 0;
        protected static int minDeadByAge = 2000;

        protected static Agent[] agents;

        #region Natural Selection

        protected float[] fitness;
        protected float fitnessSum;
        protected static float bestFitness;
        protected NeuralNetwork[] nextPopulation;
        protected float oldAverage = 0;

        #endregion Natural Selection

        /// <summary>Skips a frame in between destroying and rebuilding everything</summary>
        protected bool skipFrame = false;

        private void Awake()
        {
            SetupFields();
        }

        protected virtual void SetupFields()
        {
            fitness = new float[numberOfAgents];
            originalAgent.SetActive(false);
        }

        private void Start()
        {
            for (int i = 0; i < agents.Length; i++) {
                InstantiateAgent(i);
                if (LoadSaveFile >= 0 && saveFiles[mapNumber].Fitness > 0) {
                    saveFiles[mapNumber].Load(out Matrix[] weights);
                    agents[i].SetupNN(new NeuralNetwork(weights));
                }
                else
                    SetupNewAgent(agents[i]);
            }
            if (autoselectFirstAgent)
                SelectFirstAgent(agents[0].gameObject);
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

        private void SetupNewGeneration()
        {
            KillCreatures();
            NewPopulation();
            if (autoselectFirstAgent)
                SelectFirstAgent(agents[0].gameObject);
            Mutate();
            AmountDeadByAge = 0;
            skipFrame = false;
        }

        protected virtual void LogGenerationFitness()
        {
            if (oldAverage > fitnessSum / numberOfAgents)
                Debug.Log($"{Time.time} - Gen {generation} - Avg <color=red>{fitnessSum / numberOfAgents}</color> - Best {bestFitness}");
            else
                Debug.Log($"{Time.time} - Gen {generation} - Avg <color=green>{fitnessSum / numberOfAgents}</color> - Best {bestFitness}");
        }

        protected bool TryToSave()
        {
            if (!SaveOnBetterFitness)
                return false;
            if (agents[0].Fitness <= saveFiles[mapNumber].Fitness)
                return false;
            saveFiles[mapNumber].Save(agents[0].Fitness, agents[0].NeuralNet.Weights);
            Debug.Log("<color=blue>Saved with Fitness </color>" + agents[0].Fitness);
            return true;
        }

        protected void TryToPause(bool agentWasSaved)
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

        protected void CalculateFitness()
        {
            fitnessSum = 0;
            bestFitness = 0;
            Array.Sort(agents);
            Array.Reverse(agents);
            for (int i = 0; i < numberOfAgents; i++) {
                fitness[i] = agents[i].Fitness;
                fitnessSum += fitness[i];
                if (fitness[i] > bestFitness)
                    bestFitness = fitness[i];
            }
        }

        protected void NaturalSelection()
        {
            NeuralNetwork[] temp = new NeuralNetwork[numberOfAgents];
            float[] targetFitness = new float[numberOfAgents];
            float sum = 0;
            for (int i = 0; i < numberOfAgents; i++)
                targetFitness[i] = Random.Range(0f, fitnessSum);
            Array.Sort(targetFitness);
            temp[0] = agents[0].GetBrain();
            int currentAgent = 1;
            for (int j = 0; j < numberOfAgents; j++) {
                sum += fitness[j];
                while (sum >= targetFitness[currentAgent]) {
                    temp[currentAgent] = agents[j].GetBrain();
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

        protected static void KillCreatures()
        {
            for (int i = 0; i < agents.Length; i++)
                if (agents[i] != null) Destroy(agents[i].gameObject);
        }

        protected void NewPopulation()
        {
            for (int i = 0; i < agents.Length; i++) {
                InstantiateAgent(i);
                if ((float)i / agents.Length < procriationPercentage)
                    agents[i].SetupNN(nextPopulation[i]);
                else
                    SetupNewAgent(agents[i]);
            }
        }

        protected void SetupNewAgent(Agent agent)
        {
            agent.SetupNN(inputNodes, hiddenNodes, outputNodes);
        }

        protected abstract void InstantiateAgent(int agentIndex);

        private static void SelectFirstAgent(GameObject go)
        {
#if UNITY_EDITOR
            if (Selection.activeObject == null || string.Equals(Selection.activeObject.name, "Agent 0"))
                Selection.activeGameObject = go;
#endif
        }

        protected void Mutate()
        {
            for (int i = 5; i < numberOfAgents; i++)
                if (agentMutationRate > Random.Range(0f, 1f))
                    agents[i].Mutate(neuronMutationRate, mutationAmount);
        }

        protected bool AllDead()
        {
            for (int i = 0; i < numberOfAgents; i++)
                if (!agents[i].Dead)
                    return false;
            return true;
        }

        public static float CompareToBestFitness(float value) => value / bestFitness;
    }
}