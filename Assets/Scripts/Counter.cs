using UnityEngine;
using UnityEngine.Events;

// Script used to expose Collider TriggerEnter event for other scripts to use
public class Counter : MonoBehaviour
{
    public UnityEvent<Collider> onTriggerEnter;

    void OnTriggerEnter(Collider col)
    {
        onTriggerEnter?.Invoke(col);
    }
}