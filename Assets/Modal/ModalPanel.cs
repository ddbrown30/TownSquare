using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using TMPro;

public class ModalPanel : MonoBehaviour
{
    public TMP_Text Question;  //The Modal Window Question (or statement)
    public Button Button1;   //The first button
    public Button Button2;   //The second button
    public Button Button3;   //The third button

    public GameObject ModalPanelObject;       //Reference to the Panel Object
    private static ModalPanel MainModalPanel; //Reference to the Modal Panel, to make sure it's been included

    public static ModalPanel Instance()
    {
        if (!MainModalPanel)
        {
            MainModalPanel = FindObjectOfType(typeof(ModalPanel)) as ModalPanel;
            if (!MainModalPanel)
            {
                Debug.LogError("There needs to be one active ModalPanel script on a GameObject in your scene.");
            }
        }
        return MainModalPanel;
    }

    public void MessageBox(string Question, UnityAction YesEvent, UnityAction NoEvent, UnityAction CancelEvent, UnityAction OkEvent, string MessageType)
    {
        ModalPanelObject.SetActive(true);  //Activate the Panel; its default is "off" in the Inspector
        if (MessageType == "YesNoCancel")  //If the user has asked for the Message Box type "YesNoCancel"
        {
            //Button1 is on the far left; Button2 is in the center and Button3 is on the right.  Each can be activated and labeled individually.
            Button1.onClick.RemoveAllListeners(); if (YesEvent != null) Button1.onClick.AddListener(YesEvent); Button1.onClick.AddListener(ClosePanel); Button1.GetComponentInChildren<TMP_Text>().text = "Yes";
            Button2.onClick.RemoveAllListeners(); if (NoEvent != null) Button2.onClick.AddListener(NoEvent); Button2.onClick.AddListener(ClosePanel); Button2.GetComponentInChildren<TMP_Text>().text = "No";
            Button3.onClick.RemoveAllListeners(); if (CancelEvent != null) Button3.onClick.AddListener(CancelEvent); Button3.onClick.AddListener(ClosePanel); Button3.GetComponentInChildren<TMP_Text>().text = "Cancel";
            Button1.gameObject.SetActive(true); //We always turn on ONLY the buttons we need, and leave the rest off.
            Button2.gameObject.SetActive(true);
            Button3.gameObject.SetActive(true);
        }
        if (MessageType == "YesNo")        //If the user has asked for the Message Box type "YesNo"
        {
            Button1.onClick.RemoveAllListeners();
            Button2.onClick.RemoveAllListeners(); if (YesEvent != null) Button2.onClick.AddListener(YesEvent); Button2.onClick.AddListener(ClosePanel); Button2.GetComponentInChildren<TMP_Text>().text = "Yes";
            Button3.onClick.RemoveAllListeners(); if (NoEvent != null) Button3.onClick.AddListener(NoEvent); Button3.onClick.AddListener(ClosePanel); Button3.GetComponentInChildren<TMP_Text>().text = "No";
            Button1.gameObject.SetActive(false);
            Button2.gameObject.SetActive(true);
            Button3.gameObject.SetActive(true);
        }
        if (MessageType == "Ok")           //If the user has asked for the Message Box type "Ok"
        {
            Button1.onClick.RemoveAllListeners();
            Button2.onClick.RemoveAllListeners(); if (OkEvent != null) Button2.onClick.AddListener(OkEvent); Button2.onClick.AddListener(ClosePanel); Button2.GetComponentInChildren<TMP_Text>().text = "Ok";
            Button3.onClick.RemoveAllListeners();
            Button1.gameObject.SetActive(false);
            Button2.gameObject.SetActive(true);
            Button3.gameObject.SetActive(false);
        }
        this.Question.text = Question;     //Fill in the Question/statement part of the Message Box
    }

    void ClosePanel()
    {
        ModalPanelObject.SetActive(false); //Close the Modal Dialog
    }
}
