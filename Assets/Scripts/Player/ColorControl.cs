using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ColorControl : MonoBehaviour
{
    public static ColorControl Instance;
    public Color color;
    [SerializeField] RenderTexture floorTexture;
    [SerializeField] ChemeleonMovement movement;
    [SerializeField] GameObject disguisedTextObject;
    [SerializeField] Animator disguisedAnimator;
    Texture2D tempFloorTex;
    [Header("Input")]
    [SerializeField] InputActionReference rAction;
    [SerializeField] InputActionReference gAction;
    [SerializeField] InputActionReference bAction;
    [SerializeField] Material chameleonMat;
    [SerializeField] Image currentColorImage;
    [SerializeField] Image currentGroundColorImage;
    [SerializeField] GameObject groundColorBG;
    [Header("Stats")]
    [SerializeField] float sameColorThreshold = 0.3f;
    public float changePerPress = 0.05f;
    public float decaySpeed;
    Color currentColor;
    [Header("Sliders")]
    public ColorSlider[] colorSliders;

    private void Awake()
    {
        Instance = this;
        tempFloorTex = new Texture2D(floorTexture.width, floorTexture.height, TextureFormat.ARGB32, false);
        InvokeRepeating(nameof(UpdateGroundColorDisplay), 0f, 0.1f);
    }
    private void OnEnable()
    {
        rAction.action.performed += RPerformed;
        gAction.action.performed += GPerformed;
        bAction.action.performed += BPerformed;
        rAction.action.canceled += RCanceled;
        gAction.action.canceled += GCanceled;
        bAction.action.canceled += BCanceled;
    }

    private void RCanceled(InputAction.CallbackContext obj) => colorSliders[0].OnReleased();
    private void GCanceled(InputAction.CallbackContext obj) => colorSliders[1].OnReleased();
    private void BCanceled(InputAction.CallbackContext obj) => colorSliders[2].OnReleased();

    private void OnDisable()
    {
        rAction.action.performed -= RPerformed;
        gAction.action.performed -= GPerformed;
        bAction.action.performed -= BPerformed;
        rAction.action.canceled -= RCanceled;
        gAction.action.canceled -= GCanceled;
        bAction.action.canceled -= BCanceled;
    }

    private void RPerformed(InputAction.CallbackContext obj) => colorSliders[0].OnPressed();
    private void GPerformed(InputAction.CallbackContext obj) => colorSliders[1].OnPressed();
    private void BPerformed(InputAction.CallbackContext obj) => colorSliders[2].OnPressed();

    private void Update()
    {
        for (int i = 0; i < colorSliders.Length; i++)
        {
            colorSliders[i].UpdateColor();
        }
        currentColor = new Color(colorSliders[0].value, colorSliders[1].value, colorSliders[2].value);
        // The 1.25 is to account for the texture having an average luminocity of 0.7
        chameleonMat.SetColor("_BaseColor", currentColor * 1.25f);
        currentColorImage.color = currentColor;
    }
    [ContextMenu("Check Safe")]
    public void LogSafe() => Debug.Log($"Safe {IsSafe()}");
    public bool IsSafe()
    {
        if (movement.airborne) return false;

        // This pulls the data from the GPU to the CPU. It may cause a lag spike because of that. In future versions I should consider making it async.

        OKLab chameleonColor = OKLab.FromRGB(currentColor);
        OKLab okLabFloorColor = GetFloorColor();

        if (OKLab.Distance(chameleonColor, okLabFloorColor) > sameColorThreshold)
        {
            Debug.Log($"Color: {okLabFloorColor} was to different from: {currentColor}");
            return false;
        }

        return true;
    }
    OKLab GetFloorColor()
    {
        RenderTexture.active = floorTexture;
        tempFloorTex.ReadPixels(new Rect(0, 0, floorTexture.width, floorTexture.height), 0, 0);
        RenderTexture.active = null;

        Color[] floorColors = new Color[] {
            tempFloorTex.GetPixel(3, 3),
            tempFloorTex.GetPixel(floorTexture.width - 4, floorTexture.height - 4),
            tempFloorTex.GetPixel(3, floorTexture.height - 4),
            tempFloorTex.GetPixel(floorTexture.width - 4, 3)
        };

        OKLab[] floorColorsOKLab = new OKLab[4];
        for (int i = 0; i < floorColors.Length; i++)
        {
            floorColorsOKLab[i] = OKLab.FromLinearRGB(floorColors[i].r, floorColors[i].g, floorColors[i].b);
        }

        return OKLab.Average(floorColorsOKLab);
    }
    void UpdateGroundColorDisplay()
    {
        if (movement.airborne) groundColorBG.SetActive(false);
        else
        {
            groundColorBG.SetActive(true);
            currentGroundColorImage.color = GetFloorColor().ToUIRGBColor();
        }
    }
    public void PlaySafeAnimation()
    {
        disguisedTextObject.SetActive(true);
        disguisedAnimator.Play("Safe", 0, 0f);
    }
}
