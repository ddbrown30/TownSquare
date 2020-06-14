using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TownsfolkScript : MonoBehaviour {

    public TMP_InputField NameInput;

    void OnGUI()
    {
        if (NameInput.isFocused && NameInput.text != "" && (Input.GetKey(KeyCode.Return) || Input.GetKey(KeyCode.KeypadEnter)))
        {
            AddTownsfolk();
            NameInput.Select();
            NameInput.ActivateInputField();
        }
    }

    public void AddTownsfolk()
    {
        TownsfolkManager.Instance.AddTownsfolk(NameInput.text, false);
        NameInput.text = "";
    }

    public void AddTraveler()
    {
        TownsfolkManager.Instance.AddTownsfolk(NameInput.text, true);
        NameInput.text = "";
    }
}
