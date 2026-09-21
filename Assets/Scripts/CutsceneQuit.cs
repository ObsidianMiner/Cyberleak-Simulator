using UnityEngine;

public class CutsceneQuit : MonoBehaviour
{
    private void Start()
    {
        Application.Quit();
        Debug.Log($"Cutscene Quit");
    }
}
