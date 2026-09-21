using UnityEngine;

public class Glass : MonoBehaviour
{
    [SerializeField] bool cutsceneOnly;
    [SerializeField] GameObject destroyParticle;

    private void OnCollisionEnter(Collision collision)
    {
        if (cutsceneOnly) return;
        if (collision.relativeVelocity.magnitude > 3f)
        {
            Break();
        }
    }
    public void Break()
    {
        GameObject particle = Instantiate(destroyParticle, transform.position, Quaternion.identity);
        JSAM.AudioManager.PlaySound(SoundsSounds.GlassShatter, particle.transform);
        Destroy(gameObject);
    }
}
