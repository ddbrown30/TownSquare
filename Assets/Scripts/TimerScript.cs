using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerScript : MonoBehaviour {

    bool IsStarted;
    float StartDuration = 180;
    float TimeRemaining;

    public TMP_Text TimeText;

    public GameObject[] TimerButtons;

    public Image StartStopImageComponent;
    public Sprite StartSprite;
    public Sprite StopSprite;

    void Start()
    {
        TimeRemaining = StartDuration;
    }

    void Update ()
    {
        if (IsStarted)
        {
            if(TimeRemaining > 0)
            {
                TimeRemaining -= Time.deltaTime;
            }

            if (TimeRemaining < 0)
            {
                TimeText.color = Color.red;
            }
        }

        int secondsRemaining = Mathf.CeilToInt(TimeRemaining);
        float minutes = secondsRemaining / 60;
        float seconds = secondsRemaining % 60;

        TimeText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }


    public void ToggleTimer()
    {
        if (IsStarted)
        {
            StopTimer();
        }
        else
        {
            StartTimer();
        }
    }

    void StartTimer()
    {
        IsStarted = true;

        StartStopImageComponent.sprite = StopSprite;
        foreach (var button in TimerButtons)
        {
            button.SetActive(false);
        }
    }

    void StopTimer()
    {
        IsStarted = false;

        TimeText.color = Color.white;

        StartStopImageComponent.sprite = StartSprite;
        foreach (var button in TimerButtons)
        {
            button.SetActive(true);
        }

        if (TimeRemaining <= 0)
        {
            TimeRemaining = StartDuration;
        }
    }

    public void ResetTimer()
    {
        TimeRemaining = StartDuration;
        StopTimer();
    }

    public void MinUp()
    {
        TimeRemaining += 60;
        StartDuration = TimeRemaining;
    }

    public void MinDown()
    {
        if (TimeRemaining < 60)
            return;

        TimeRemaining -= 60;
        StartDuration = TimeRemaining;
    }

    public void SecUp()
    {
        int secondsRemaining = (int)TimeRemaining;
        float seconds = secondsRemaining % 60;

        if(seconds == 59)
        {
            TimeRemaining -= 59;
        }
        else
        {
            TimeRemaining += 1;
        }
        
        StartDuration = TimeRemaining;
    }

    public void SecDown()
    {
        int secondsRemaining = (int)TimeRemaining;
        float seconds = secondsRemaining % 60;
        if(seconds == 0)
        {
            TimeRemaining += 59;
        }
        else
        {
            TimeRemaining -= 1;
        }

        StartDuration = TimeRemaining;
    }
}
