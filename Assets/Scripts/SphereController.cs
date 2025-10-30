using System.Collections.Generic;
using UnityEngine;

// Script to pool Sphere Objects, control their position and serve them to other Scripts
public class SphereController : MonoBehaviour
{
    [Header("Sphere generation setup")]
    [SerializeField] private GameObject sphere;
    [SerializeField] private int spawnAreaSize = 5;
    [SerializeField] private int amountToPool = 50;
    [SerializeField] private int radiusMultiplier = 3;

    private List<GameObject> pooledObjects;
    public static SphereController SharedInstance;

    void Awake()
    {
        SharedInstance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Check if sphere is assigned in the Inspector
        if (sphere == null)
            Debug.LogError("Gameobject must be assigned to sphere!");
        // Loop through list of pooled objects, Instantiating, deactivating them and adding them to the list 
        pooledObjects = new List<GameObject>();
        for (int i = 0; i < amountToPool; i++)
        {
            GameObject obj = Instantiate(sphere);
            obj.SetActive(false);
            pooledObjects.Add(obj);
            obj.transform.SetParent(this.transform); // set as children of Spawn Manager
        }
    }

    // Function to enable gravity on active Spheres and start game
    public void DropBalls()
    {
        foreach(GameObject go in GetActiveObjectsInPool())
        {
            go.GetComponent<Rigidbody>().useGravity = true;
        }
    }

    // Function to move Spheres to position at the top of the tower.
    // The positions are spread in a grid and only one sphere can spawn in one grid slot
    public void GenerateSpheres(int spheresToSpawn, float elevation)
    {
        // Get the Sphere radius
        float actualSphereRadius = sphere.GetComponent<SphereCollider>().radius * sphere.transform.lossyScale.x;
        // The spawn grid is divided to grid slots each slot has a rastersize
        float rasterSize = (radiusMultiplier + 2) * actualSphereRadius;
        // Based on a restersize divide the spawn area to grid slots
        // The base grid is square
        int numberOfRows = Mathf.FloorToInt(spawnAreaSize * 2 / rasterSize);
        // The grid is three dimensional
        int numberOfFloors = Mathf.CeilToInt((float)spheresToSpawn / (numberOfRows * numberOfRows));
        // Try to redistribute the sphere positions throughout the available slots in the grid
        // Create an array to store every possible position in the grid
        int[] gridPositions = new int[numberOfFloors * numberOfRows * numberOfRows];
        // Fill the array based on the amount spheres that needs to be generated
        // 1 equals sphere; 0 equals no sphere
        int gridIndex;
        for (gridIndex = 0; gridIndex < gridPositions.Length; gridIndex++)
        {
            if (gridIndex < spheresToSpawn)
                gridPositions[gridIndex] = 1;
            else
                gridPositions[gridIndex] = 0;
        }
        // Try to redistribute the sphere slots in the grid evenly
        int randomRedistributedGridIndex;
        // If their are more spheres than empty spaces: redistribute the empty spaces
        if (spheresToSpawn > (gridPositions.Length - spheresToSpawn))
        {
            for (gridIndex = spheresToSpawn; gridIndex < gridPositions.Length; gridIndex++)
            {
                // Get new index but only switch positions if it has not been already switched
                randomRedistributedGridIndex = Mathf.FloorToInt(Random.Range(0f, spheresToSpawn));
                if (gridPositions[randomRedistributedGridIndex] == 1)
                {
                    gridPositions[gridIndex] = gridPositions[randomRedistributedGridIndex];
                    gridPositions[randomRedistributedGridIndex] = 0;
                }
            }
        }
        // If their are more empty spaces than spheres: redistribute the sphere positions
        else
        {
            for (gridIndex = 0; gridIndex < spheresToSpawn; gridIndex++)
            {
                // Get new index but only switch positions if it has not been already switched
                randomRedistributedGridIndex = Mathf.FloorToInt(Random.Range((float)spheresToSpawn, gridPositions.Length));
                if (gridPositions[randomRedistributedGridIndex] == 0)
                {
                    gridPositions[gridIndex] = gridPositions[randomRedistributedGridIndex];
                    gridPositions[randomRedistributedGridIndex] = 1;
                }
            }
        }
        // Generate random position inside the grid slot for each sphere in the grid
        for (int x = 0; x < numberOfRows; x++)
        {
            for (int z = 0; z < numberOfRows; z++)
            {
                for (int y = 0; y < numberOfFloors; y++)
                {
                    if (gridPositions[x + y * numberOfFloors + z * numberOfRows] == 1)
                    {
                        // Randomize sphere placement inside the grid slot
                        float n1 = Random.Range(0f, radiusMultiplier * actualSphereRadius);
                        float n2 = Random.Range(0f, radiusMultiplier * actualSphereRadius);
                        float n3 = Random.Range(0f, radiusMultiplier * actualSphereRadius);
                        //Get and set sphere asset properties and postions, and activate them
                        GameObject currentSphere = GetPooledObject();
                        currentSphere.GetComponent<Rigidbody>().useGravity = false;
                        currentSphere.GetComponent<Rigidbody>().linearVelocity = new Vector3(0, 0, 0);
                        currentSphere.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
                        currentSphere.transform.position = new Vector3(
                            x * rasterSize + actualSphereRadius + n1 - spawnAreaSize,
                            elevation + transform.position.y + y * rasterSize + actualSphereRadius + n2 + 5.0f,
                            z * rasterSize + actualSphereRadius + n3 - spawnAreaSize
                            );
                        currentSphere.SetActive( true );
                    }
                }
            }
        }
    }

    // Function to get pooled object
    private GameObject GetPooledObject()
    {
        // For as many objects as are in the pooledObjects list
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            // if the pooled objects is NOT active, return that object 
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        // otherwise, return null   
        return null;
    }

    // Function to return a list of active pooled objects
    private List<GameObject> GetActiveObjectsInPool()
    {
        List <GameObject> pooledActiveObjects = new List <GameObject>();
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (pooledObjects[i].activeInHierarchy)
            {
                pooledActiveObjects.Add(pooledObjects[i]);
            }
        }  
        return pooledActiveObjects;
    }

    // Function to disable all pooled objects
    public void ResetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (pooledObjects[i].activeInHierarchy)
            {
                pooledObjects[i].SetActive(false);
            }
        }
    }
}
