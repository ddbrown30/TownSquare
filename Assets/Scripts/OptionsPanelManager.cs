using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionsPanelManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    bool IsOpen;
    bool IsMouseOver;

    public GameObject PanelParent;

    public Slider RoleScaleSlider;
    public Slider RadiusSlider;
    public Slider VoteSpeedSlider;
    public BackgroundImageSelector BackgroundSelector;

    float DefaultRoleScale = 0.5f;
    float DefaultRadiusScale = 0.6f;
    float DefaultVoteSpeed = 0.6f;
    int DefaultBackgroundIndex = 0;

    void Awake()
    {
        
        RoleScaleSlider.value = PlayerPrefs.GetFloat("RoleScale", DefaultRoleScale);
        RadiusSlider.value = PlayerPrefs.GetFloat("Radius", DefaultRadiusScale);
        VoteSpeedSlider.value = PlayerPrefs.GetFloat("VoteSpeed", DefaultVoteSpeed);
        BackgroundSelector.SetImageIndex(PlayerPrefs.GetInt("BackgroundIndex", DefaultBackgroundIndex));
    }

    void ResetToDefaults()
    {
        RoleScaleSlider.value = DefaultRoleScale;
        RadiusSlider.value = DefaultRadiusScale;
        VoteSpeedSlider.value = DefaultVoteSpeed;
        BackgroundSelector.SetImageIndex(DefaultBackgroundIndex);

        PlayerPrefs.SetFloat("RoleScale", RoleScaleSlider.value);
        PlayerPrefs.SetFloat("Radius", RadiusSlider.value);
        PlayerPrefs.SetFloat("VoteSpeed", VoteSpeedSlider.value);
        PlayerPrefs.SetInt("BackgroundIndex", BackgroundSelector.CurrentIndex);
    }

    public void OpenOptionsPanel()
    {
        IsOpen = true;
        PanelParent.SetActive(true);
    }

    void CloseOptionsPanel()
    {
        IsOpen = false;
        PanelParent.SetActive(false);

        //Save our current values when we close the menu
        PlayerPrefs.SetFloat("RoleScale", RoleScaleSlider.value);
        PlayerPrefs.SetFloat("Radius", RadiusSlider.value);
        PlayerPrefs.SetFloat("VoteSpeed", VoteSpeedSlider.value);
        PlayerPrefs.SetInt("BackgroundIndex", BackgroundSelector.CurrentIndex);
    }

    public void OnClickResetOptions()
    {
        ModalManager.Instance().MessageBox("Reset all options to their default values?", ResetToDefaults, null, null, null, "YesNo");
    }

    void Update()
    {
        if(IsOpen && IsMouseOver == false)
        {
            if (Input.GetMouseButtonDown(0))
                CloseOptionsPanel();
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        IsMouseOver = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        IsMouseOver = false;
    }
}
