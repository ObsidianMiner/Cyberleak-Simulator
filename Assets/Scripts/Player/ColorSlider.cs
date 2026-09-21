using UnityEngine;
using UnityEngine.UI;

public class ColorSlider : MonoBehaviour
{
    public Slider slider;
    public Color color;
    public Image fill;
    public float value;
    public float smoothDampVel;
    public float animationSpeed = 2f;
    [SerializeField] Image buttonImage;
    [SerializeField] Sprite pressedSprite;
    [SerializeField] Sprite unpressedSprite;

    public void UpdateColor()
    {
        fill.color = color;
        value -= Time.deltaTime * ColorControl.Instance.decaySpeed;
        value = Mathf.Clamp01(value);
        slider.value = Mathf.SmoothDamp(slider.value, value, ref smoothDampVel, Time.deltaTime * animationSpeed);
    }
    public void OnPressed()
    {
        value += ColorControl.Instance.changePerPress;
        buttonImage.sprite = pressedSprite;
    }
    public void OnReleased() => buttonImage.sprite = unpressedSprite;
}
