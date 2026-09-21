using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Reflection;
using TMPro;

public class SyncOnStart : MonoBehaviour
{
    [SerializeField] string propertyName;
    [SerializeField] MonoBehaviour script;
    public enum SetType { Slider, Dropbox, Toggle}
    [SerializeField] SetType setType;
    // Start is called before the first frame update
    void Start()
    {
        switch (setType)
        {
            case SetType.Slider:
                GetComponent<Slider>().SetValueWithoutNotify((float)script.GetType().GetProperty(propertyName).GetValue(script, null));
                break;
            case SetType.Dropbox:
                GetComponent<TMP_Dropdown>().SetValueWithoutNotify((int)script.GetType().GetProperty(propertyName).GetValue(script, null));
                break;
            case SetType.Toggle:
                GetComponent<Toggle>().SetIsOnWithoutNotify((bool)script.GetType().GetProperty(propertyName).GetValue(script, null));
                break;
            default:
                break;
        }
    }
}
