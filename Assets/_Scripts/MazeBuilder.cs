using GeneticNeuralNetwork;
using UnityEngine;

[RequireComponent(typeof(Population))]
public class MazeBuilder : MonoBehaviour
{
    [SerializeField] [Range(0, 4)] private int mapNumber;
    public int MapNumber { get => mapNumber; }
    [SerializeField] private Transform mazeParent;
    [SerializeField] private GameObject[] platformPrefabs;
    private Population population;

    public void Build()
    {
        population = GetComponent<Population>();
        int n = mazeParent.childCount;
        for (int i = n - 1; i >= 0; i--)
            DestroyImmediate(mazeParent.GetChild(i).gameObject);

        int mazesPerCollum = population.NumberOfAgents / Population.NumberOfCollums;
        float x, y;

        for (int mazeIndex = 0; mazeIndex < population.NumberOfAgents; mazeIndex++) {
            x = Population.DistanceBetweenPlataformsX * (mazeIndex / mazesPerCollum);
            y = (mazeIndex % mazesPerCollum) * Population.DistanceBetweenPlataformsY;
            Instantiate(platformPrefabs[mapNumber], new Vector3(x, y, 0), Quaternion.identity, mazeParent);
        }
    }
}