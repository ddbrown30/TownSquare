using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class HelpPanel : MonoBehaviour
{
    public Button CloseButton;

    UnityEvent OnCloseListener;

    void Awake()
    {
        OnCloseListener = new UnityEvent();
    }

    public void OpenHelpPanel(UnityAction CloseEvent)
    {
        OnCloseListener.AddListener(CloseEvent);
    }

    public void CloseHelpPanel()
    {
        OnCloseListener.Invoke();
    }
}
