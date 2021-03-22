using GeneticNeuralNetwork;
using UnityEngine;

namespace FlappyPlane
{
    public class PlanePopulation : Population
    {
        private readonly Vector3 ZERO = Vector3.zero;

        protected override void InstantiateAgent(int agentIndex)
        {
            GameObject go = Instantiate(originalAgent, ZERO, Quaternion.identity, agentParent);

            go.name = "Agent " + agentIndex;
            go.SetActive(true);
            agents[agentIndex] = go.GetComponent<FlappyAgent>();
        }

        protected override void SetupFields()
        {
            base.SetupFields();
            agents = new FlappyAgent[numberOfAgents];
            FlappyAgent temp = originalAgent.GetComponent<FlappyAgent>();

            inputNodes = 3 + temp.numberOfLasers;
            for (int i = 0; i < hiddenNodes.Length; i++)
                if (hiddenNodes[i] <= 0)
                    hiddenNodes[i] = outputNodes;
        }

        protected override void ResetScene()
        {
            GameController.ResetGame();
        }
    }
}