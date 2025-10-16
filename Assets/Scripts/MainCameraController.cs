using UnityEngine;
using UnityEngine.InputSystem;

public class MainCameraController : MonoBehaviour
{
    public SphereController spheres;
    
    public float turnSensitivity = 1;
    public float elevationSensitivity = 0.1f;
    public float maxElevation = 20.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnLook(InputValue value)
    {
        var v = value.Get<Vector2>();
        transform.Rotate(Vector3.up, v.x * turnSensitivity);
        float y = transform.position.y;
        y += elevationSensitivity * v.y;
        if (y < 0)
        {
            y = 0;
        }
        if (y >= maxElevation)
        {
            y = maxElevation;
        }
        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
    /*
    public void OnAttack()
    {
        spheres.DropBalls();
    }*/
}
