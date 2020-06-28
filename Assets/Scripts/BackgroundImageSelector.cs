using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BackgroundImageSelector : MonoBehaviour
{
    public TMP_Text SelectedText;
    public Sprite[] BackgroundImages;
    public int CurrentIndex { get; private set; }

    public void NextImage()
    {
        if (CurrentIndex >= BackgroundImages.Length)
        {
            SetImageIndex(0);
        }
        else
        {
            SetImageIndex(CurrentIndex + 1);
        }
    }

    public void PreviousImage()
    {
        if (CurrentIndex == 0)
        {
            SetImageIndex(BackgroundImages.Length);
        }
        else
        {
            SetImageIndex(CurrentIndex - 1);
        }
    }

    public void SetImageIndex(int index)
    {
        CurrentIndex = index;
        if (index == 0)
        {
            TownsfolkManager.Instance.SetBackgroundImage(null);
            SelectedText.text = "None";
        }
        else
        {
            TownsfolkManager.Instance.SetBackgroundImage(BackgroundImages[index - 1]);
            SelectedText.text = index.ToString();
        }
    }
}
