using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Missle : MonoBehaviour
{
    [SerializeField] GameObject particle;
    private void OnCollisionEnter(Collision collision)
    {
        if (!enabled) return;
        JSAM.AudioManager.PlaySound(SoundsSounds.Explosion);
        StartCoroutine(nameof(KillPlayer));
    }
    IEnumerator KillPlayer()
    {
        if (TryGetComponent(out MeshRenderer renderer)) renderer.enabled = false;
        particle.SetActive(true);
        yield return new WaitForSeconds(0.4f);
        SceneManager.LoadScene(0);
    }
}
