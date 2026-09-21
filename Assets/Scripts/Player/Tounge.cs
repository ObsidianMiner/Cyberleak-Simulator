using UnityEngine;

public class Tounge : MonoBehaviour
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] float maxTravelTime = 2f;
    float travelTime = 0f;
    public bool hooked;
    private void Awake()
    {
        ChemeleonMovement.Instance.grappling = this;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (hooked) return;

        if (collision.transform.TryGetComponent(out Pickupable pickupable))
        {
            pickupable.Pickup();
            Destroy(gameObject);
            return;
        }

        Destroy(GetComponent<Rigidbody>());
        ChemeleonMovement.Instance.StartGrapple();
        hooked = true;
        transform.parent = collision.transform;
    }
    private void OnDestroy()
    {
        if (hooked) ChemeleonMovement.Instance.EndGrapple();
    }
    private void OnDisable()
    {
        Destroy(gameObject);
    }
    private void Update()
    {
        if (!hooked)
        {
            travelTime += Time.deltaTime;
            if (travelTime > maxTravelTime)
            {
                // TODO Play cut graple sound
                Destroy(gameObject);
            }
        }
        lineRenderer.SetPositions(new Vector3[] { transform.position, ChemeleonMovement.Instance.toungeStart.position });
    }
}
