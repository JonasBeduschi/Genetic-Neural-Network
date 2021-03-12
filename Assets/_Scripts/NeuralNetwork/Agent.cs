using System;
using System.Collections;
using UnityEngine;

namespace GeneticNeuralNetwork
{
    public abstract class Agent : MonoBehaviour, IComparable
    {
        public NeuralNetwork NeuralNet { get; protected set; }

        protected float[] input;
        public float[] GetInput() => input;

        protected float[] output;
        public float[] GetOutput() => output;

        public float Fitness = 0;

        public void SetupNN(int inputNodes, int[] hiddenNodes, int outputNodes)
        {

            NeuralNet = new NeuralNetwork(inputNodes, hiddenNodes, outputNodes);
            // Add one for bias
            input = new float[inputNodes + 1];
            SetupAgent();
        }

        public void SetupNN(NeuralNetwork newNeuralNet)
        {
            NeuralNet = new NeuralNetwork(newNeuralNet);
            input = new float[NeuralNet.GetInputLength()];
            SetupAgent();
        }

        protected abstract void SetupAgent();

        public void Mutate(float mutationRate, float mutationAmount) => NeuralNet.Mutate(mutationRate, mutationAmount);

        protected abstract float[] GetInputs();

        public NeuralNetwork GetBrain() => new NeuralNetwork(NeuralNet);

        public bool Dead { get; protected set; } = false;

        public int CompareTo(object obj) => Fitness.CompareTo(((Agent)obj).Fitness);
    }
}