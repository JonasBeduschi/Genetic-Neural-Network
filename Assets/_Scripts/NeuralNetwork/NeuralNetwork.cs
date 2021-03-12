using UnityEngine;

namespace GeneticNeuralNetwork
{
    public class NeuralNetwork
    {
        public Matrix[] Weights;
        private Matrix temp;

        public NeuralNetwork(int inputNodes, int[] hiddenNodes, int outputNodes)
        {
            int[] nodes = new int[2 + hiddenNodes.Length];
            nodes[0] = inputNodes;
            for (int i = 0; i < hiddenNodes.Length; i++)
                nodes[i + 1] = hiddenNodes[i];
            nodes[nodes.Length - 1] = outputNodes;

            Weights = new Matrix[nodes.Length - 1];

            // Adding bias
            for (int i = 0; i < nodes.Length; i++)
                nodes[i]++;

            for (int i = 0; i < nodes.Length - 1; i++)
                Weights[i] = Matrix.RandomMatrix(nodes[i + 1], nodes[i]);
        }

        public NeuralNetwork(Matrix[] weights)
        {
            Weights = new Matrix[weights.Length];
            for (int i = 0; i < weights.Length; i++)
                Weights[i] = new Matrix(weights[i]);
        }

        public NeuralNetwork(NeuralNetwork original)
        {
            Weights = new Matrix[original.Weights.Length];
            for (int i = 0; i < original.Weights.Length; i++)
                Weights[i] = new Matrix(original.Weights[i]);
        }

        public NeuralNetwork(NeuralNetwork parentA, NeuralNetwork parentB)
        {
            Weights = new Matrix[parentA.Weights.Length];
            for (int i = 0; i < parentA.Weights.Length; i++)
                Weights[i] = new Matrix(ChildMatrixFromParents(parentA.Weights[i], parentB.Weights[i]));
        }

        private Matrix ChildMatrixFromParents(Matrix parentA, Matrix parentB)
        {
            float[,] array = new float[parentA.Rows, parentA.Collums];
            for (int row = 0; row < parentA.Rows; row++) {
                for (int collum = 0; collum < parentA.Collums; collum++) {
                    array[row, collum] = Extensions.Average(parentA.array[row, collum], parentB.array[row, collum]);
                }
            }
            return new Matrix(array);
        }

        public float[] Query(float[] inputs)
        {
            temp = new Matrix(inputs);
            for (int i = 0; i < Weights.Length; i++) {
                temp = Weights[i] * temp;
                temp.Activate();
            }
            return temp.ToArray();
        }

        /// <summary>Randomly mutates the weights of a NN based on certain rates.</summary>
        /// <param name="mutationRate">How likely each weight is to mutate.</param>
        /// <param name="mutationAmount">How much the weight should be changed by.</param>
        public void Mutate(float mutationRate, float mutationAmount)
        {
            mutationRate.Cap();
            mutationAmount.Cap();
            if (mutationRate == 0 || mutationAmount == 0)
                return;

            for (int k = 0; k < Weights.Length; k++) {
                for (int i = 0; i < Weights[k].Rows; i++) {
                    for (int j = 0; j < Weights[k].Collums; j++) {
                        if (Random.Range(0f, 1f) < mutationRate)
                            Weights[k][i, j] = MutateWeight(Weights[k][i, j], mutationAmount);
                    }
                }
            }
        }

        private static float MutateWeight(float number, float mutationAmount)
        {
            int sign = Random.Range(0, 2) * 2 - 1;
            number *= mutationAmount * sign;
            return number;
        }

        public int GetInputLength() => Weights[0].Collums;
    }
}