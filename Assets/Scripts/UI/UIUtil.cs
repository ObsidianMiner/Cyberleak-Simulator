using UnityEngine;
using UnityEngine.InputSystem;

public class UIUtil : MonoBehaviour
{
    public static UIUtil Main;
    static UIInstance _currentUI;
    public UIInstance debugMenu;
    public UIInstance pauseMenu;
    public bool hideHUD;
    public static bool useNavigation;
    [Header("Input")]
    [SerializeField] InputActionReference pauseAction;
    [SerializeField] InputActionReference debugAction;

    public static UIInstance currentUI
    {
        get
        {
            return _currentUI;
        }
        set
        {
            _currentUI = value;
            Main.HUD.SetActive(value == null && !Main.hideHUD);
        }
    }
    public static bool inMenu
    {
        get
        {
            return _currentUI != null;
        }
    }
    public GameObject HUD;
    public GameObject grayOut;
    public bool forceUnlockCursor;
    [SerializeField] RectTransform scaleableMenu;
    public float menuScale
    {
        set
        {
            scaleableMenu.localScale = new Vector3(value, value, value);
        }
    }

    public void OnDestroy()
    {
        if (_currentUI != null) _currentUI.Toggle();
    }
    private void OnValidate()
    {
        Main = this;
    }
    private void Awake()
    {
        Main = this;
    }
    private void OnEnable()
    {
        pauseAction.action.Enable();
        debugAction.action.Enable();
    }
    void Update()
    {
        bool canOpenMenus = true;

        if (pauseAction.action.WasPressedThisFrame() && canOpenMenus)
        {
            if (currentUI == null) pauseMenu.Toggle();
            else currentUI.Close();
        }

        if (debugAction.action.WasPressedThisFrame()) debugMenu.Toggle();


        bool unlockCursor = currentUI != null || forceUnlockCursor;
        Cursor.lockState = unlockCursor ? CursorLockMode.None : CursorLockMode.Locked;
    }
    public void ToggleHUD()
    {
        HUD.SetActive(!HUD.activeSelf);
    }
}
