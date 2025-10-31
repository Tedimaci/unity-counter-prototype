using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

// Script to control the generation of the tower of obstacles
public class ObstacleController : MonoBehaviour
{
    [Header("Tower generation setup")]
    [SerializeField] private int initialHeight;
    [SerializeField] private GameObject[] obstacleList;
    [SerializeField] private Material[] materials;

    public float actualHeight { get; private set; } = 0;
    public int actualFloorNumber { get; private set; } = 0;
    private int[] obstacles;
    private int[] obstacleAngles;
    public static ObstacleController SharedInstance;

    void Awake()
    {
        SharedInstance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        // Check if obstacles are assigned in the Inspector
        if (obstacleList == null)
            Debug.LogError("At least one Gameobject must be assigned to obstacleList!");
        // Check if materials are assigned in the Inspector
        if (materials == null)
            Debug.LogError("At least one Gameobject must be assigned to materials!");
        // Claculate the length and initialize the possible obstacle and angle variations
        int obstaclesLength = 0;
        for (int a = 0; a < obstacleList.Length; a++)
        {
            obstaclesLength += obstacleList[a].GetComponent<ObstacleParameters>().obstacleAngleVariant.Length;
        }
        obstacles = new int[obstaclesLength];
        obstacleAngles = new int[obstaclesLength];
        // Fill up the initialized obstacle arrays
        int i = 0;
        for (int a = 0; a < obstacleList.Length; a++)
        {
            for (int b = 0; b < obstacleList[a].GetComponent<ObstacleParameters>().obstacleAngleVariant.Length; b++)
            {
                obstacles[i] = a;
                obstacleAngles[i] = obstacleList[a].GetComponent<ObstacleParameters>().obstacleAngleVariant[b];
                i++;
            }
        }
    }

    // Function to add a floor to the tower
    public void AddObstacle()
    {
        // Generate a random index
        int index = Random.Range(0, obstacles.Length);
        // Calculate the height of the tower
        actualHeight = initialHeight + actualFloorNumber *
                obstacleList[obstacles[index]].GetComponent<ObstacleParameters>().obstacleHeight;
        // Instantiate the chosen obsatcle with its rotation
        GameObject newObstacle = Instantiate(obstacleList[obstacles[index]],
            new Vector3(0, actualHeight, 0),
            Quaternion.Euler(0, obstacleAngles[index], 0),
            transform);
        actualFloorNumber++;
        // Change the material of the obstacle
        int obstacleMaterialIndex = Random.Range(0, materials.Length);
        foreach (var obstacleRenderer in newObstacle.GetComponentsInChildren<MeshRenderer>())
        {
            obstacleRenderer.material = materials[obstacleMaterialIndex];
        }
    }

    // Function to remove all obstacles from the tower
    public void RemoveAllObstacle()
    {
        // Destroy ebery object that is a child component of the obstacle controller
        foreach (var obstacle in GetComponentsInChildren<Transform>())
        {
            if (!obstacle.CompareTag("GameController"))
            {
                Destroy(obstacle.gameObject);
            }
        }
        // Reset the public variables
        actualFloorNumber = 0;
        actualHeight = 0;
    }
}
