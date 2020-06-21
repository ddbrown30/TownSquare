using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TownsfolkToken : MonoBehaviour, IDragHandler, IDropHandler
{
    bool IsAlive = true;
    public Sprite AliveSprite;
    public Sprite DeadSprite;
    public Sprite TravelerAliveSprite;

    public Image GhostVote;

    public InputField NameText;

    Image ImageComponent;

    RectTransform RectTrans;

    Vector2 TargetPos = new Vector2();
    public void SetTargetPos(Vector2 targetPos) { TargetPos = targetPos; }

    public bool IsTraveler;

    void Awake()
    {
        RectTrans = gameObject.GetComponent<RectTransform>();
    }

    void Start()
    {
        ImageComponent = GetComponentInChildren<Image>();
        ImageComponent.sprite = IsTraveler ? TravelerAliveSprite : AliveSprite;
        GhostVote.enabled = false;
    }

    void Update()
    {
        Vector2 pos2d =  RectTrans.anchoredPosition;
        if (pos2d != TargetPos)
        {
            const float moveRate = 8f;
            pos2d = Vector2.Lerp(pos2d, TargetPos, moveRate * Time.deltaTime);

            if ((pos2d - TargetPos).sqrMagnitude < 1f)
            {
                pos2d = TargetPos;
            }

            RectTrans.anchoredPosition = pos2d;
        }
    }

    public void OnClickToken()
    {
        if(TownsfolkManager.Instance.IsVoteActive)
        {
            TownsfolkManager.Instance.VoteManager.SelectNominee(this);
        }
        else
        {
            ToggleAlive();
        }
    }

    void ToggleAlive()
    {
        if (IsAlive)
        {
            IsAlive = false;
            ImageComponent.sprite = DeadSprite;
            GhostVote.enabled = true;
        }
        else if (GhostVote.enabled)
        {
            GhostVote.enabled = false;
        }
        else
        {
            IsAlive = true;
            ImageComponent.sprite = IsTraveler ? TravelerAliveSprite : AliveSprite;
            GhostVote.enabled = false;
        }
    }

    public void RemoveTownsfolk()
    {
        TownsfolkManager.Instance.RemoveTownsfolk(this);
    }

    public void SetPlayerName(string name)
    {
        NameText.text = name;
    }

    public void SetIsTraveler(bool isTraveler)
    {
        IsTraveler = isTraveler;
        if(IsAlive && ImageComponent)
        {
            ImageComponent.sprite = IsTraveler ? TravelerAliveSprite : AliveSprite;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.parent.gameObject.GetComponent<RectTransform>(), Input.mousePosition, null, out localPoint);
        int index = TownsfolkManager.Instance.GetIndexFromPosition(localPoint);
        TownsfolkManager.Instance.MoveTownsfolkToIndex(index, this);

        RectTrans.anchoredPosition = localPoint;
        TargetPos = localPoint;
    }

    public void OnDrop(PointerEventData eventData)
    {
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(transform.parent.parent.gameObject.GetComponent<RectTransform>(), Input.mousePosition, null, out localPoint);
        int index = TownsfolkManager.Instance.GetIndexFromPosition(localPoint);
        TownsfolkManager.Instance.UpdateTown();
    }
}
