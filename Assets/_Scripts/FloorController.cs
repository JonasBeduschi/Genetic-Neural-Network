using UnityEngine;

public class FloorController : MonoBehaviour
{
    [SerializeField] GameObject[] obstacles;
    private void Awake()
    {
        foreach (GameObject obstacle in obstacles)
            if (Random.Range(0, 100) < 50)
                obstacle.SetActive(false);
        Destroy(this);
    }
}