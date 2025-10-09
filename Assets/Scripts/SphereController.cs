using UnityEngine;

public class SphereController : MonoBehaviour
{
    private Rigidbody[] rigidBodies;
    [SerializeField] private GameObject sphere;
    public int spheresToSpawn = 10;
    [SerializeField] private float actualSphereRadius;
    public int spawnAreaSize = 5;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (sphere == null)
            Debug.LogError("Gameobject must be assigned to sphere!");

        GetActualSpereRadius();
        GenerateSpheres();
        rigidBodies = GetComponentsInChildren<Rigidbody>();
    }

    public void DropBalls()
    {
        foreach(Rigidbody rb in rigidBodies)
        {
            rb.useGravity = true;
        }
    }

    private void GenerateSpheres()
    {
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
                        Instantiate(sphere, new Vector3(
                            x * rasterSize + actualSphereRadius + n1 - spawnAreaSize,
                            transform.position.y + y * rasterSize + actualSphereRadius + n2,
                            z * rasterSize + actualSphereRadius + n3 - spawnAreaSize
                            ), transform.rotation, transform);
                    }
                }
            }
        }
    }

    private void GetActualSpereRadius()
    {
        actualSphereRadius = sphere.GetComponent<SphereCollider>().radius 
            * sphere.transform.lossyScale.x;
    }
}
