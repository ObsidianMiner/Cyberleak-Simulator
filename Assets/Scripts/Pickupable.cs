using UnityEngine;
using UnityEngine.Events;

public class Pickupable : MonoBehaviour
{
    public UnityEvent onPickup;
    public void Pickup()
    {
        onPickup.Invoke();
        Destroy(gameObject);
    }
}
