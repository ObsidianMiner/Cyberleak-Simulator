using UnityEngine;

public class Spinner : MonoBehaviour
{
    public Vector3 spinSpeed;

    void Update()
    {
        transform.Rotate(spinSpeed * Time.deltaTime);
    }
}
