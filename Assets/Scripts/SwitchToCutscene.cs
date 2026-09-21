using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchToCutscene : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 6)
        {
            SceneManager.LoadScene(1);
        }
    }
}
