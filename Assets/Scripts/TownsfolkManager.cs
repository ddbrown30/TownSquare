using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using UnityEngine.EventSystems;

public class TownsfolkManager : MonoBehaviour, IPointerClickHandler
{
    public static TownsfolkManager Instance;
    List<TownsfolkToken> Townsfolk = new List<TownsfolkToken>();
    int TravelerCount;
    
    public GameObject TownsfolkTokenPrefab;
    public ContextMenu ContextMenuPrefab;

    private ModalManager ModalManager;

    public float MinTokenScale = 0.5f;
    public float MaxTokenScale = 2f;

    float TownRadius;
    float MinTokenRadius = 300f;
    float MaxTokenRadius = 500f;

    public Slider RadiusSlider;
    public Slider ScaleSlider;

    public GameObject RoleCountsPanel;
    public GameObject NotEnoughPlayersPanel;
    public GameObject TooManyPlayersPanel;
    public TMP_Text TotalPlayersCountText;
    public TMP_Text TownsfolkCountText;
    public TMP_Text OutsiderCountText;
    public TMP_Text MinionCountText;

    public GameObject TokenAttach;

    public Image BackgroundImage;

    public GameObject Timer;

    ContextMenu TownSquareContextMenu;

    public GameObject EndNominationButton;
    public VoteManager VoteManager;
    public bool IsVoteActive { get; set; }

    struct RoleCount
    {
        public RoleCount(int townsfolk, int outsiders, int minions) { Townsfolk = townsfolk; Outsiders = outsiders; Minions = minions; }
        public int Townsfolk;
        public int Outsiders;
        public int Minions;
    }

    RoleCount[] RoleCounts;

    public TownsfolkManager()
    {
        Instance = this;
    }

    void Awake()
    {
        ModalManager = ModalManager.Instance();
        RoleCounts = new RoleCount[]
        {
            new RoleCount(3,0,1),
            new RoleCount(3,1,1),
            new RoleCount(5,0,1),
            new RoleCount(5,1,1),
            new RoleCount(5,2,1),
            new RoleCount(7,0,2),
            new RoleCount(7,1,2),
            new RoleCount(7,2,2),
            new RoleCount(9,0,3),
            new RoleCount(9,1,3),
            new RoleCount(9,2,3)
        };
    }
    
    void Start()
    {
        TownRadius = Mathf.Lerp(MinTokenRadius, MaxTokenRadius, RadiusSlider.value);
    }
    
    public void AddTownsfolk(string name, bool isTraveler)
    {
        GameObject tokenObj = Instantiate(TownsfolkTokenPrefab, transform);
        tokenObj.transform.SetParent(TokenAttach.transform);
        tokenObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        TownsfolkToken townsfolkToken = tokenObj.GetComponentInChildren<TownsfolkToken>();
        townsfolkToken.name = "Townsfolk:" + name;
        townsfolkToken.transform.localScale = Vector3.one * Mathf.Lerp(MinTokenScale, MaxTokenScale, ScaleSlider.value);
        townsfolkToken.SetPlayerName(name);

        if(isTraveler)
        {
            townsfolkToken.SetIsTraveler(true);
            ++TravelerCount;
        }

        Townsfolk.Add(townsfolkToken);

        UpdateRoleCounts();
        UpdateTown();
    }

    public void RemoveTownsfolk(TownsfolkToken townsfolkToken)
    {
        if (townsfolkToken.IsTraveler)
        {
            --TravelerCount;
        }

        Townsfolk.Remove(townsfolkToken);
        Object.Destroy(townsfolkToken.gameObject);

        UpdateTown();
        UpdateRoleCounts();
    }

    public void RandomizeTownsfolk()
    {
        Townsfolk = Townsfolk.OrderBy(x => Random.value).ToList();
        UpdateTown();
    }

    public void ResetTownsfolk()
    {
        foreach (var townsfolk in Townsfolk)
        {
            Object.Destroy(townsfolk.gameObject);
        }
        Townsfolk.Clear();
        TravelerCount = 0;
        UpdateTown();
        UpdateRoleCounts();
    }

    public Vector2 RotateVector(Vector2 v, float delta)
    {
        return new Vector2(
            v.x * Mathf.Cos(delta) - v.y * Mathf.Sin(delta),
            v.x * Mathf.Sin(delta) + v.y * Mathf.Cos(delta)
        );
    }

    public void UpdateTown()
    {
        int townSize = Townsfolk.Count;
        float angleDivision =(2.0f * Mathf.PI) / townSize;
        for (int i = 0; i < townSize; ++i)
        {
            Vector2 dir = RotateVector(Vector2.up, -angleDivision * i);
            TownsfolkToken token = Townsfolk[i].GetComponentInChildren<TownsfolkToken>();
            token.SetTargetPos(dir * TownRadius);
        }
    }

    public void UpdateRoleCounts()
    {
        int townSize = Townsfolk.Count - TravelerCount;

        TotalPlayersCountText.text = townSize.ToString();

        if(townSize < 5)
        {
            RoleCountsPanel.SetActive(false);
            NotEnoughPlayersPanel.SetActive(true);
            TooManyPlayersPanel.SetActive(false);
        }
        else if(townSize > 20)
        {
            RoleCountsPanel.SetActive(false);
            NotEnoughPlayersPanel.SetActive(false);
            TooManyPlayersPanel.SetActive(true);
        }
        else
        {
            int countIndex = Mathf.Clamp(townSize - 5, 0, RoleCounts.Length - 1);
            TownsfolkCountText.text = RoleCounts[countIndex].Townsfolk.ToString();
            OutsiderCountText.text = RoleCounts[countIndex].Outsiders.ToString();
            MinionCountText.text = RoleCounts[countIndex].Minions.ToString();

            RoleCountsPanel.SetActive(true);
            NotEnoughPlayersPanel.SetActive(false);
            TooManyPlayersPanel.SetActive(false);
        }
    }

    public int GetIndexFromPosition(Vector2 position)
    {
        int townSize = Townsfolk.Count;
        float angleDivision =(2.0f * Mathf.PI) / townSize;

        Vector2 pos2d = gameObject.GetComponent<RectTransform>().anchoredPosition;
        Vector2 dir = (position - pos2d).normalized;


        float upAngle = Mathf.Atan2(Vector2.up.y, Vector2.up.x);
        float posAngle = Mathf.Atan2(dir.y, dir.x);
        float angle = Mathf.DeltaAngle(posAngle * Mathf.Rad2Deg, upAngle * Mathf.Rad2Deg) * Mathf.Deg2Rad;
        if (angle < 0)
            angle += Mathf.PI * 2;
        return (int)(angle / angleDivision);
    }

    public float GetAngleOfIndex(int index)
    {
        int townSize = Townsfolk.Count;
        float angleDivision =(2.0f * Mathf.PI) / townSize;
        return angleDivision * index;
    }

    public int GetTownsfolkIndex(TownsfolkToken token)
    {
        return Townsfolk.IndexOf(token);
    }

    public void MoveTownsfolkToIndex(int index, TownsfolkToken token)
    {
        int currentIndex = Townsfolk.IndexOf(token);
        if (currentIndex == index)
        {
            UpdateTown();
            return;
        }

        Townsfolk.Remove(token);
        if (index < Townsfolk.Count)
        {
            Townsfolk.Insert(index, token);
        }
        else
        {
            Townsfolk.Add(token);
        }

        UpdateTown();
    }

    public void StartNomination(TownsfolkToken nominee)
    {
        IsVoteActive = true;

        Timer.gameObject.SetActive(false);
        EndNominationButton.gameObject.SetActive(true);

        VoteManager.gameObject.SetActive(true);
        VoteManager.transform.SetAsLastSibling();
        VoteManager.Reset();
        VoteManager.SelectNominee(nominee);
    }

    public void EndNomination()
    {
        IsVoteActive = false;

        Timer.gameObject.SetActive(true);
        EndNominationButton.gameObject.SetActive(false);

        VoteManager.gameObject.SetActive(false);
        VoteManager.transform.SetAsLastSibling();
        VoteManager.Reset();
    }

    public void OnClickReset()
    {
        ModalManager.MessageBox("Reset the Town Square?", ResetTownsfolk, null, null, null, "YesNo");
    }

    public void OnClickRandomize()
    {
        ModalManager.MessageBox("Randomize player positions?", RandomizeTownsfolk, null, null, null, "YesNo");
    }

    public void ScaleSliderChanged(float value)
    {
        Vector2 scale = Vector3.one * Mathf.Lerp(MinTokenScale, MaxTokenScale, value);

        foreach (var token in Townsfolk)
        {
            token.gameObject.transform.localScale = scale;
        }
    }

    public void RadiusSliderChanged(float value)
    {
        TownRadius = Mathf.Lerp(MinTokenRadius, MaxTokenRadius, value);
        UpdateTown();
    }

    public void SetBackgroundImage(Sprite backgroundImage)
    {
        //Sprite backgroundImage = BackgroundSelectDropdown.options[index].image;
        if (backgroundImage != null)
        {
            BackgroundImage.gameObject.SetActive(true);
            BackgroundImage.sprite = backgroundImage;
        }
        else
        {
            BackgroundImage.gameObject.SetActive(false);
            BackgroundImage.sprite = null;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            TownSquareContextMenu = Instantiate<GameObject>(ContextMenuPrefab.gameObject, GetComponentInParent<Canvas>().transform).GetComponent<ContextMenu>();
            TownSquareContextMenu.transform.localScale = Vector3.one;
            TownSquareContextMenu.transform.localPosition = Vector3.zero;

            TownSquareContextMenu.AddMenuItem("Reset Town Square", OnClickReset);
            TownSquareContextMenu.AddMenuItem("Randomize player positions", OnClickRandomize);
            TownSquareContextMenu.FinaliseMenu();

            ContextMenu.HideAllMenus();//hide other menus
            TownSquareContextMenu.ShowAtMousePosition();//show the menu
        }
    }
}
