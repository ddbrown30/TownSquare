using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TownsfolkToken : MonoBehaviour, IBeginDragHandler, IDragHandler, IDropHandler, IPointerClickHandler
{
    bool IsAlive = true;
    public Sprite AliveSprite;
    public Sprite DeadSprite;
    public Sprite TravelerAliveSprite;

    public Image GhostVote;

    public TMP_InputField NameText;

    public Image TokenImage;

    RectTransform RectTrans;

    Vector2 TargetPos = new Vector2();
    public void SetTargetPos(Vector2 targetPos) { TargetPos = targetPos; }

    public bool IsTraveler;

    public ContextMenu ContextMenuPrefab;
    ContextMenu TokenContextMenu;

    void Awake()
    {
        RectTrans = gameObject.GetComponent<RectTransform>();
    }

    void Start()
    {
        TokenImage.alphaHitTestMinimumThreshold = 0.5f;

        TokenImage.sprite = IsTraveler ? TravelerAliveSprite : AliveSprite;
        GhostVote.enabled = false;
    }

    void Update()
    {
        Vector2 pos2d =  RectTrans.localPosition;
        if (pos2d != TargetPos)
        {
            const float moveRate = 8f;
            pos2d = Vector2.Lerp(pos2d, TargetPos, moveRate * Time.deltaTime);

            if ((pos2d - TargetPos).sqrMagnitude < 1f)
            {
                pos2d = TargetPos;
            }

            RectTrans.localPosition = pos2d;
        }
    }

    void ToggleAlive()
    {
        if (IsAlive)
        {
            IsAlive = false;
            TokenImage.sprite = DeadSprite;
            GhostVote.enabled = true;
        }
        else if (GhostVote.enabled)
        {
            GhostVote.enabled = false;
        }
        else
        {
            IsAlive = true;
            TokenImage.sprite = IsTraveler ? TravelerAliveSprite : AliveSprite;
            GhostVote.enabled = false;
        }
    }

    public void RemoveTownsfolk()
    {
        TownsfolkManager.Instance.RemoveTownsfolk(this);
    }

    void OnClickRemoveTownsfolk()
    {
        ModalManager.Instance().MessageBox("Remove this player from the town?", RemoveTownsfolk, null, null, null, "YesNo");
    }

    void NominatePlayer()
    {
        TownsfolkManager.Instance.StartNomination(this);
    }

    public void SetPlayerName(string name)
    {
        NameText.text = name;
    }

    public void SetIsTraveler(bool isTraveler)
    {
        IsTraveler = isTraveler;
        if(IsAlive && TokenImage)
        {
            TokenImage.sprite = IsTraveler ? TravelerAliveSprite : AliveSprite;
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        eventData.eligibleForClick = false; //Disable clicks so that we still receive OnDrop
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

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            ToggleAlive();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            TokenContextMenu = Instantiate<GameObject>(ContextMenuPrefab.gameObject, GetComponentInParent<Canvas>().transform).GetComponent<ContextMenu>();
            TokenContextMenu.transform.localScale = Vector3.one;
            TokenContextMenu.transform.localPosition = Vector3.zero;

            TokenContextMenu.AddMenuItem("Nominate player", NominatePlayer);
            TokenContextMenu.AddMenuItem("Remove player", OnClickRemoveTownsfolk);
            TokenContextMenu.FinaliseMenu();

            ContextMenu.HideAllMenus();//hide other menus
            TokenContextMenu.ShowAtMousePosition();//show the menu
        }
    }
}
