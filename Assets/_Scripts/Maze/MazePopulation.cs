using UnityEngine;
using Random = UnityEngine.Random;

namespace GeneticNeuralNetwork
{
    public class MazePopulation : Population
    {
        [Header("Specific")]
        [SerializeField] private bool randomizeStartingHeights = false;

        private const int minStartingHeight = -1;
        private const int maxStartingHeight = 7;
        [SerializeField] [Range(minStartingHeight, maxStartingHeight)] private int startingHeight;

        public const float DistanceBetweenPlataformsX = 130f;
        public const float DistanceBetweenPlataformsY = 11f;
        public const int NumberOfCollums = 5;

        private static int agentsPerCollum;

        protected override void SetupFields()
        {
            base.SetupFields();
            MaximumLife = 600;
            AmountDeadByAge = 0;
            minDeadByAge = 2000;

            mapNumber = GetComponent<MazeBuilder>().MapNumber;

            agents = new MazeRunner[numberOfAgents];
            MazeRunner temp = originalAgent.GetComponent<MazeRunner>();

            inputNodes = 2 + temp.numberOfLasers + temp.memoriesToConsider * 2;
            for (int i = 0; i < hiddenNodes.Length; i++)
                if (hiddenNodes[i] <= 0)
                    hiddenNodes[i] = outputNodes;

            agentsPerCollum = numberOfAgents / NumberOfCollums;
        }

        protected override void InstantiateAgent(int agentIndex)
        {
            if (randomizeStartingHeights)
                startingHeight = Random.Range(minStartingHeight, maxStartingHeight + 1);

            GameObject go = Instantiate(originalAgent, PositionFromIndex(agentIndex, startingHeight), Quaternion.identity, agentParent);

            go.name = "Agent " + agentIndex;
            go.SetActive(true);
            agents[agentIndex] = go.GetComponent<MazeRunner>();
        }

        private static Vector3 PositionFromIndex(int agentIndex, float startingHeight)
        {
            float x = DistanceBetweenPlataformsX * (agentIndex / agentsPerCollum);
            float y = (agentIndex % agentsPerCollum) * DistanceBetweenPlataformsY + startingHeight;
            return new Vector3(x, y, 0);
        }
    }
}