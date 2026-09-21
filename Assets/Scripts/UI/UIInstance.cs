using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Base class for all menus to inherit. Only one UIInstance can be active at once.
/// </summary>
public class UIInstance : MonoBehaviour
{
    [SerializeField] bool grayOutBackground;
    public bool cannotExitOutOfWithB;
    public UIInstance toBackOutTo;
    public GameObject firstSelected;
    protected bool disableOnClose = true;
    virtual protected void OnClose()
    {

    }
    virtual protected void OnOpen()
    {

    }
    public void Close()
    {
        UIUtil.currentUI = null;
        if (disableOnClose) gameObject.SetActive(false);
        if (grayOutBackground) UIUtil.Main.grayOut.SetActive(false);
        OnClose();
        if (toBackOutTo != null) toBackOutTo.Open();
    }
    public void Open()
    {
        UIUtil.currentUI = this;
        gameObject.SetActive(true);
        if (UIUtil.useNavigation && EventSystem.current != null) EventSystem.current.SetSelectedGameObject(firstSelected);
        if (grayOutBackground) UIUtil.Main.grayOut.SetActive(true);
        OnOpen();
    }
    public void Toggle()
    {
        if (gameObject.activeSelf) Close();
        else Open();
    }
}