using GeneticNeuralNetwork;
using UnityEngine;

[RequireComponent(typeof(MazePopulation))]
public class MazeBuilder : MonoBehaviour
{
    [SerializeField] [Range(0, 4)] private int mapNumber;
    public int MapNumber { get => mapNumber; }
    [SerializeField] private Transform mazeParent;
    [SerializeField] private GameObject[] platformPrefabs;
    private MazePopulation population;

    public void Build()
    {
        population = GetComponent<MazePopulation>();
        ClearParent();

        int mazesPerCollum = population.NumberOfAgents / MazePopulation.NumberOfCollums;
        float x, y;

        for (int mazeIndex = 0; mazeIndex < population.NumberOfAgents; mazeIndex++) {
            x = MazePopulation.DistanceBetweenPlataformsX * (mazeIndex / mazesPerCollum);
            y = (mazeIndex % mazesPerCollum) * MazePopulation.DistanceBetweenPlataformsY;
            Instantiate(platformPrefabs[mapNumber], new Vector3(x, y, 0), Quaternion.identity, mazeParent);
        }
    }

    public void ClearParent()
    {
        int n = mazeParent.childCount;
        for (int i = n - 1; i >= 0; i--)
            DestroyImmediate(mazeParent.GetChild(i).gameObject);
    }
}