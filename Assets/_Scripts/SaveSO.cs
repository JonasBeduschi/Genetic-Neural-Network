using UnityEngine;

[CreateAssetMenu(fileName = "New Save File", menuName = "New SaveSO")]
public class SaveSO : ScriptableObject
{
    [SerializeField] public float Fitness { get; private set; }
    [SerializeField] private float[] WeightInputHidden;
    [SerializeField] private float[] WeightHiddenOutput;

    [SerializeField] private int sizeWIH;
    [SerializeField] private int sizeWHO;
    public void Save(float fitness, float[,] originalWIH, float[,] originalWHO)
    {
        Fitness = fitness;
        sizeWIH = originalWIH.GetLength(0);
        WeightInputHidden = new float[sizeWIH * originalWIH.GetLength(1)];
        for (int i = 0; i < sizeWIH; i++)
            for (int j = 0; j < originalWIH.GetLength(1); j++)
                WeightInputHidden[i * originalWIH.GetLength(1) + j] = originalWIH[i, j];

        sizeWHO = originalWHO.GetLength(0);
        WeightHiddenOutput = new float[sizeWHO * originalWHO.GetLength(1)];
        for (int i = 0; i < sizeWHO; i++)
            for (int j = 0; j < originalWHO.GetLength(1); j++)
                WeightHiddenOutput[i * originalWHO.GetLength(1) + j] = originalWHO[i, j];
    }

    public void Load(out float[,] originalWIH, out float[,] originalWHO)
    {
        originalWIH = new float[sizeWIH, WeightInputHidden.Length / sizeWIH];
        for (int i = 0; i < sizeWIH; i++)
            for (int j = 0; j < originalWIH.GetLength(1); j++)
                originalWIH[i, j] = WeightInputHidden[i * originalWIH.GetLength(1) + j];

        originalWHO = new float[sizeWHO, WeightHiddenOutput.Length / sizeWHO];
        for (int i = 0; i < sizeWHO; i++)
            for (int j = 0; j < originalWHO.GetLength(1); j++)
                originalWHO[i, j] = WeightHiddenOutput[i * originalWHO.GetLength(1) + j];
    }
}