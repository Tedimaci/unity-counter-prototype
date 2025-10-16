using UnityEngine;
using UnityEngine.Events;

public class WallController : MonoBehaviour
{
    public UnityEvent<Collider> onTriggerEnter;

    void OnTriggerEnter(Collider col)
    {
        onTriggerEnter?.Invoke(col);
    }
}