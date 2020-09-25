using UnityEngine;

public class NeuralNetwork
{
    public float[,] WeightInputHidden;
    public float[,] WeightHiddenOutput;

    private float[,] hiddenInputs;
    private float[,] hiddenOutputs;
    private float[,] finalInputs;
    private float[,] finalOutputs;

    public NeuralNetwork(int inputNodes, int hiddenNodes, int outputNodes)
    {
        WeightInputHidden = Extensions.RandomMatrix(hiddenNodes, inputNodes);
        WeightHiddenOutput = Extensions.RandomMatrix(outputNodes, hiddenNodes);
    }

    public NeuralNetwork(float[,] WeightInputHidden, float[,] WeightHiddenOutput)
    {
        this.WeightInputHidden = WeightInputHidden.Copy();
        this.WeightHiddenOutput = WeightHiddenOutput.Copy();
    }

    public NeuralNetwork(NeuralNetwork original)
    {
        WeightInputHidden = original.WeightInputHidden.Copy();
        WeightHiddenOutput = original.WeightHiddenOutput.Copy();
    }

    public float[] Query(float[] inputs)
    {
        hiddenInputs = WeightInputHidden.DotMultiply(inputs.ToMatrix());
        hiddenOutputs = hiddenInputs.Activate();
        finalInputs = WeightHiddenOutput.DotMultiply(hiddenOutputs);
        finalOutputs = finalInputs.Activate();
        return finalOutputs.ToArray();
    }

    public NeuralNetwork Clone() => new NeuralNetwork(this);

    public void Mutate(float mutationRate)
    {
        for (int i = 0; i < WeightInputHidden.GetLength(0); i++) {
            for (int j = 0; j < WeightInputHidden.GetLength(1); j++) {
                if (Random.Range(0f, 1f) < mutationRate)
                    WeightInputHidden[i, j] = Random.Range(-.5f, .5f);
            }
        }
        for (int i = 0; i < WeightHiddenOutput.GetLength(0); i++) {
            for (int j = 0; j < WeightHiddenOutput.GetLength(1); j++) {
                if (Random.Range(0f, 1f) < mutationRate)
                    WeightHiddenOutput[i, j] = Random.Range(-.5f, .5f);
            }
        }
    }

    public int GetInputLength() => WeightInputHidden.GetLength(1);
}