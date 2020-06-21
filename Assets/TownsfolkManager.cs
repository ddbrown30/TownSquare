using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class TownsfolkManager : MonoBehaviour
{
    public static TownsfolkManager Instance;
    List<TownsfolkToken> Townsfolk = new List<TownsfolkToken>();
    int TravelerCount;
    
    public GameObject TownsfolkTokenObject;

    private ModalPanel ModalPanel;

    public float MinTokenScale = 0.5f;
    public float MaxTokenScale = 2f;
    public Slider ScaleSlider;

    private float TownRadius;
    public float MinTokenRadius = 300f;
    public float MaxTokenRadius = 600f;
    public Slider RadiusSlider;

    public TMP_Text TotalPlayersCountText;
    public TMP_Text TownsfolkCountText;
    public TMP_Text OutsiderCountText;
    public TMP_Text MinionCountText;

    public GameObject Timer;

    public TMP_Text NominateButtonText;
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

    void Awake()
    {
        ModalPanel = ModalPanel.Instance();
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
        Instance = this;

        TownRadius = Mathf.Lerp(MinTokenRadius, MaxTokenRadius, RadiusSlider.value);
    }
    
    public void AddTownsfolk(string name, bool isTraveler)
    {
        GameObject tokenObj = Instantiate(TownsfolkTokenObject, transform);
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
        Vector2 pos2d = gameObject.GetComponent<RectTransform>().anchoredPosition;
        int townSize = Townsfolk.Count;
        float angleDivision =(2.0f * Mathf.PI) / townSize;
        for (int i = 0; i < townSize; ++i)
        {
            Vector2 dir = RotateVector(Vector2.up, -angleDivision * i);
            TownsfolkToken token = Townsfolk[i].GetComponentInChildren<TownsfolkToken>();
            token.SetTargetPos(pos2d + (dir * TownRadius));
        }
    }

    public void UpdateRoleCounts()
    {
        int townSize = Townsfolk.Count - TravelerCount;
        int countIndex = Mathf.Clamp(townSize - 5, 0, RoleCounts.Length);
        TotalPlayersCountText.text = townSize.ToString();
        TownsfolkCountText.text = RoleCounts[countIndex].Townsfolk.ToString();
        OutsiderCountText.text = RoleCounts[countIndex].Outsiders.ToString();
        MinionCountText.text = RoleCounts[countIndex].Minions.ToString();
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

    public void ToggleNomination()
    {
        IsVoteActive = !IsVoteActive;
        Timer.gameObject.SetActive(!IsVoteActive);
        VoteManager.gameObject.SetActive(IsVoteActive);
        VoteManager.transform.SetAsLastSibling();
        VoteManager.ResetHands();

        if (IsVoteActive)
        {
            NominateButtonText.text = "Stop Nomination";
        }
        else
        {
            NominateButtonText.text = "Start Nomination";
        }
    }

    public void OnClickReset()
    {
        ModalPanel.MessageBox("Reset the Town Square?", ResetTownsfolk, null, null, null, "YesNo");
    }

    public void OnClickRandomize()
    {
        ModalPanel.MessageBox("Randomize player positions?", RandomizeTownsfolk, null, null, null, "YesNo");
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
}
