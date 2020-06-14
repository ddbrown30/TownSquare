using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TownsfolkShuffle : MonoBehaviour
{
    public Toggle ConfirmToggle;
    public void RandomizeTownsfolk()
    {
        if (ConfirmToggle.isOn)
        {
            ConfirmToggle.isOn = false;
            TownsfolkManager.Instance.RandomizeTownsfolk();
        }
    }
}
