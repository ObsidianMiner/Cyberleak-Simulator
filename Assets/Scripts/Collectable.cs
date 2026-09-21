using System.Collections;
using TMPro;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] CanvasGroup group;
    [SerializeField] bool[] collectablesCollected;
    public void CollectableGot(int index)
    {
        collectablesCollected[index] = true;
        text.text = $"Brain rot collected {collectablesCollected.CountTrue()}/{collectablesCollected.Length}";
        JSAM.AudioManager.PlaySound(SoundsSounds.sus);
        StartCoroutine("Fade");
    }
    IEnumerator Fade()
    {
        float time = 1f;
        group.alpha = 1f;
        yield return null;
        while (time >= 0f)
        {
            time -= Time.deltaTime * 0.3f;
            group.alpha = time;
            yield return null;
        }
        group.alpha = 0f;
    }
}
