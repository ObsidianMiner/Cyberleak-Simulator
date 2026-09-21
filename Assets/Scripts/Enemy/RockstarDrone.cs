using UnityEngine;

public class RockstarDrone : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] GameObject missle;
    public void CheckPlayer()
    {
        bool safe = ColorControl.Instance.IsSafe();
        // Play sounds
        if (!safe)
        {
            if (missle.TryGetComponent(out Rigidbody rb)) rb.isKinematic = false;
            missle.transform.parent = null;
            if (missle.TryGetComponent(out Missle explosion)) explosion.enabled = true;
        }
        else
        {
            ColorControl.Instance.PlaySafeAnimation();
        }
    }
    public void StartStrafe()
    {
        gameObject.SetActive(true);
        animator.Play("DroneSwoop", 0, 0f);
    }
    public void FinishStrafe()
    {
        gameObject.SetActive(false);
    }
}
