using System.Collections.Generic;
using UnityEngine;

public class SphereController : MonoBehaviour
{
    [SerializeField] private GameObject sphere;
    [SerializeField] private int spawnAreaSize = 5;
    [SerializeField] private int amountToPool;

    public static SphereController SharedInstance;
    public List<GameObject> pooledObjects;

    void Awake()
    {
        SharedInstance = this;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (sphere == null)
            Debug.LogError("Gameobject must be assigned to sphere!");
        // Loop through list of pooled objects,deactivating them and adding them to the list 
        pooledObjects = new List<GameObject>();
        for (int i = 0; i < amountToPool; i++)
        {
            GameObject obj = (GameObject)Instantiate(sphere);
            obj.SetActive(false);
            pooledObjects.Add(obj);
            obj.transform.SetParent(this.transform); // set as children of Spawn Manager
        }
    }

    public void DropBalls()
    {
        foreach(GameObject go in GetActiveObjectsInPool())
        {
            go.GetComponent<Rigidbody>().useGravity = true;
        }
    }

    public void GenerateSpheres(int spheresToSpawn, float elevation)
    {
        GameObject currentSphere = null;
        float actualSphereRadius = sphere.GetComponent<SphereCollider>().radius
            * sphere.transform.lossyScale.x;
        int radiusMultiplier = 3;
        float rasterSize = (radiusMultiplier + 2) * actualSphereRadius;
        int numberOfRows = 
            Mathf.FloorToInt(spawnAreaSize * 2 / rasterSize);
        int numberOfFloors =
            Mathf.CeilToInt((float)spheresToSpawn / (numberOfRows * numberOfRows));
        int[] a = new int[numberOfFloors*numberOfRows*numberOfRows];
        for (int b = 0; b < a.Length; b++)
        {
            if (b < spheresToSpawn)
                a[b] = 1;
            else
                a[b] = 0;
        }
        if (spheresToSpawn > (a.Length - spheresToSpawn))
        {
            for (int c = spheresToSpawn; c < a.Length; c++)
            {
                int d = Mathf.FloorToInt(Random.Range(0f, spheresToSpawn));
                a[c] = a[d];
                a[d] = 0;
            }
        }
        else
        {
            for (int c = 0; c < spheresToSpawn; c++)
            {
                int d = Mathf.FloorToInt(Random.Range((float)spheresToSpawn, a.Length));
                a[c] = a[d];
                a[d] = 1;
            }
        }
        for (int x = 0; x < numberOfRows; x++)
        {
            for (int z = 0; z < numberOfRows; z++)
            {
                for (int y = 0; y < numberOfFloors; y++)
                {
                    if (a[x + y * numberOfFloors + z * numberOfRows] == 1)
                    {
                        float n1 = Random.Range(0f, radiusMultiplier * actualSphereRadius);
                        float n2 = Random.Range(0f, radiusMultiplier * actualSphereRadius);
                        float n3 = Random.Range(0f, radiusMultiplier * actualSphereRadius);
                        currentSphere = GetPooledObject();
                        currentSphere.transform.position = new Vector3(
                            x * rasterSize + actualSphereRadius + n1 - spawnAreaSize,
                            elevation + transform.position.y + y * rasterSize + actualSphereRadius + n2 + 5.0f,
                            z * rasterSize + actualSphereRadius + n3 - spawnAreaSize
                            );
                        currentSphere.GetComponent<Rigidbody>().useGravity = false;
                        currentSphere.GetComponent<Rigidbody>().linearVelocity = new Vector3(0, 0, 0);
                        currentSphere.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, 0, 0);
                        currentSphere.SetActive( true );
                    }
                }
            }
        }
    }

    public GameObject GetPooledObject()
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

    public List<GameObject> GetActiveObjectsInPool()
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
