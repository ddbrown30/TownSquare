using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VoteManager : MonoBehaviour
{
    public GameObject HourHand;
    public GameObject MinuteHand;

    public Slider SpeedSlider;

    public Image StartStopImageComponent;
    public Sprite StartSprite;
    public Sprite StopSprite;

    bool VoteStarted;
    float NomineeAngle;
    float FirstVoteAngle;
    float CurrentAngle;
    float TotalAngleToMove;

    public void Update()
    {
        if(VoteStarted)
        {
            CurrentAngle += SpeedSlider.value * Time.deltaTime;
            if (CurrentAngle >= TotalAngleToMove)
            {
                ToggleVote();
                CurrentAngle = 0;
                Vector2 dir = RotateVector(Vector2.up, -NomineeAngle);
                MinuteHand.transform.up = dir;
            }
            else
            {
                Vector2 dir = RotateVector(Vector2.up, -(FirstVoteAngle + CurrentAngle));
                MinuteHand.transform.up = dir;
            }

        }
    }

    public void SelectNominee(TownsfolkToken nominee)
    {
        if (VoteStarted)
            return;

        CurrentAngle = 0;
        
        int nomIndex = TownsfolkManager.Instance.GetTownsfolkIndex(nominee);
        NomineeAngle = TownsfolkManager.Instance.GetAngleOfIndex(nomIndex);
        FirstVoteAngle = TownsfolkManager.Instance.GetAngleOfIndex(nomIndex + 1);

        Vector2 nomDir = RotateVector(Vector2.up, -NomineeAngle);
        HourHand.transform.up = nomDir;

        Vector2 firstVoteDir = RotateVector(Vector2.up, -FirstVoteAngle);
        MinuteHand.transform.up = firstVoteDir;

        TotalAngleToMove = Mathf.DeltaAngle(FirstVoteAngle * Mathf.Rad2Deg, NomineeAngle * Mathf.Rad2Deg) * Mathf.Deg2Rad;
        if (TotalAngleToMove < 0)
            TotalAngleToMove += Mathf.PI * 2;
    }

    public Vector2 RotateVector(Vector2 v, float delta)
    {
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }

    public void ToggleVote()
    {
        VoteStarted = !VoteStarted;

        if(VoteStarted)
        {
            StartStopImageComponent.sprite = StopSprite;
        }
        else
        {
            StartStopImageComponent.sprite = StartSprite;
        }
    }

    public void Reset()
    {
        HourHand.transform.up = Vector2.up;
        MinuteHand.transform.up = Vector2.up;
        NomineeAngle = 0;
        FirstVoteAngle = 0;
        CurrentAngle = 0;
        TotalAngleToMove = 0;

        VoteStarted = false;
        StartStopImageComponent.sprite = StartSprite;
    }
}
