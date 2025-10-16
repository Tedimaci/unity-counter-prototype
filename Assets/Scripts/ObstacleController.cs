using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.Hierarchy;
using UnityEngine;

public class ObstacleController : MonoBehaviour
{
    public GameObject[] obstacleList;
    public Color[] colors;
    public int initialHeight;
    public float actualHeight;
    public int actualFloorNumber = 0;
    //private
    private Dictionary<int, int> obstacleDictionary = new Dictionary<int, int>();
    private Dictionary<int, int> obstacleAngleDictionary = new Dictionary<int, int>();
    private int obstacleListMax =0;

    public static ObstacleController SharedInstance;

    void Awake()
    {
        SharedInstance = this;
    }

    private void Start()
    {
        obstacleListMax = 0;
        for (int a = 0; a < obstacleList.Length; a++)
        {
            for (int b = 0;
                b < obstacleList[a].GetComponent<ObstacleParameters>().obstacleAngleVariant.Length; 
                b++)
            {
                obstacleDictionary.Add(obstacleListMax, a);
                obstacleAngleDictionary.Add(obstacleListMax,
                    obstacleList[a].GetComponent<ObstacleParameters>().obstacleAngleVariant[b]);
                obstacleListMax++;
            }
        }
    }

    public void AddObstacle()
    {
        int index = Random.Range(0, obstacleListMax);
        actualHeight = initialHeight + actualFloorNumber *
                obstacleList[obstacleDictionary[index]].GetComponent<ObstacleParameters>().obstacleHeight;
        Instantiate(obstacleList[obstacleDictionary[index]],
            new Vector3(0, actualHeight, 0),
            Quaternion.Euler(0, obstacleAngleDictionary[index], 0),
            transform);
        actualFloorNumber++;
    }

    public void RemoveObstacle()
    {
        float top = 0;
        GameObject gameObject = null;
        foreach (var obstacle in GetComponentsInChildren<Transform>())
        {
            if (obstacle.position.y > top)
            {
                top = obstacle.position.y;
                gameObject = obstacle.gameObject;
            }
        }
        Destroy(gameObject);
    }

    public void RemoveAllObstacle()
    {
        foreach (var obstacle in GetComponentsInChildren<Transform>())
        {
            if (!obstacle.CompareTag("GameController"))
            {
                Destroy(obstacle.gameObject);
            }
        }
    }
}
