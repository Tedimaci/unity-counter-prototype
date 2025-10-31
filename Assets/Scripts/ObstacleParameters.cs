using UnityEngine;

// Script used to add reusable parameters to each individual obastacle Asset
public class ObstacleParameters : MonoBehaviour
{
    public int obstacleHeight;
    public int[] obstacleAngleVariant;

    private void Start()
    {
        // Check if obstacleHeight is set in the Inspector
        if (obstacleHeight <= 0)
            Debug.LogError("Obstacle height must be set to a positive number!");
        // Check if obstacleAngleVariant are assigned in the Inspector
        if (obstacleAngleVariant == null)
            Debug.LogError("At least one Angle must be assigned to obstacleAngleVariant!");
    }
}