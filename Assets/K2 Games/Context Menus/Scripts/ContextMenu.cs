using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ContextMenu : MonoBehaviour, IPointerExitHandler
{
    public static Vector3 startingClickPosition;

    Canvas canvas;//which GUI canvas this is a part of

    public TransitionalObject transition;

    public VerticalLayoutGroup menuLayout;
    public GameObject selector;

    public ContextMenuItem menuItemPrefab;

    [HideInInspector]
    public ContextMenu parentMenu;

    [HideInInspector]
    public List<ContextMenuItem> menuItems = new List<ContextMenuItem>();

    public const float HoverTimer = 0.2f;//how much time you need to spend hovering over an item before its sub menu is displayed
    
    void Awake()
    {
        #region Canvas Search
        Transform lastChecked = transform.parent;

        while(canvas == null && lastChecked != null)
        {
            canvas = lastChecked.GetComponent<Canvas>();//recursively look through the parent objects until we find the canvas we are a part of

            lastChecked = lastChecked.parent;
        }

#if(UNITY_EDITOR)
        if(canvas == null)
            Debug.LogError("Failed to find parent canvas for " + transform.name);
#endif
        #endregion

        gameObject.SetActive(false);
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))//if left clicking
            Hide();//close the menu (will be called by all open menus)
    }

    public static void HideAllMenus()
    {
        ContextMenu[] openMenus = FindObjectsOfType<ContextMenu>();//find all currently active menus

        for(int i = 0; i < openMenus.Length; i++)
            openMenus[i].Hide();//close them
    }

    public void Show()
    {
        transition.TriggerTransition();
    }

    public void ShowAtMousePosition()
    {
        transition.TriggerTransition();

        RectTransform transform = ((RectTransform)base.transform);

        startingClickPosition = Input.mousePosition;

        if(canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            transform.position = startingClickPosition;
        else
            transform.position = ConvertMouseToGUISpace(canvas);

        if(startingClickPosition.y > Screen.height / 2)//if on the top half of the screen
            transform.anchoredPosition -= new Vector2(0, transform.sizeDelta.y);//then display below the mouse

        if(startingClickPosition.x > Screen.width / 2)//if on the right
            transform.anchoredPosition -= new Vector2(transform.sizeDelta.x, 0);

        transform.SetAsLastSibling();
    }

    public void Hide()
    {
        transition.TriggerFadeOutIfActive();
        selector.SetActive(false);

        for(int i = 0; i < menuItems.Count; i++)
            if(menuItems[i].subMenu != null)
            {
                menuItems[i].StopAllCoroutines();
                menuItems[i].keepMenuOpen = false;
                menuItems[i].subMenu.Hide();
            }
    }

    public void AddMenuItem(string description, ContextMenuItem.StandardDelegate onClickMethod)
    {
        AddMenuItem(description, onClickMethod, null);
    }

    /// <summary>
    /// Adds a menu item and the option to include a sub menu
    /// </summary>
    public void AddMenuItem(string description, ContextMenuItem.StandardDelegate onClickMethod, ContextMenu subMenu)
    {
        ContextMenuItem temp = Instantiate<GameObject>(menuItemPrefab.gameObject).GetComponent<ContextMenuItem>();

        temp.transform.SetParent(menuLayout.transform);
        temp.transform.localScale = Vector3.one;//reset the scale, the position is handled auotmatically
        temp.transform.localPosition = Vector3.zero;

        temp.Initialise(this, description, onClickMethod, subMenu);

        menuItems.Add(temp);
    }

    public void FinaliseMenu()
    {
        transition.InitialiseAlphaTransition();

        ((RectTransform)transform).sizeDelta = new Vector2(((RectTransform)transform).sizeDelta.x, menuItems.Count * ((RectTransform)selector.transform).sizeDelta.y + menuLayout.padding.top + menuLayout.padding.bottom);//resize the menu according to what is in it
    }

    /// <summary>
    /// This is called when hovering over an item that is a sub menu, it prevents the parent menu from closing
    /// </summary>
    public void SupressHiding()
    {
        for(int i = 0; i < menuItems.Count; i++)
            menuItems[i].keepMenuOpen = true;

        selector.SetActive(true);//keep the selector active
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        selector.SetActive(false);
    }

    public void CloseOtherSubMenus(ContextMenuItem caller)
    {
        for(int i = 0; i < menuItems.Count; i++)
            if(menuItems[i].subMenu != null)
                if(!caller.Equals(menuItems[i]))//ignore the item calling this method
                    menuItems[i].subMenu.Hide();
    }

    /// <summary>
    /// Finds the highest menu and returns its transform. Useful for calculating positions
    /// </summary>
    /// <param name="menu">Starting menu</param>
    /// <returns></returns>
    public static RectTransform GetHighestMenu(ContextMenu menu)
    {
        while(menu.parentMenu != null)
            menu = menu.parentMenu;

        return menu.transform as RectTransform;
    }

    static Vector3 ConvertMouseToGUISpace(Canvas canvas)
    {
        Vector2 position;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform, Input.mousePosition, canvas.worldCamera, out position);
        return canvas.transform.TransformPoint(position);
    }

    public void OnMenuCloseFinished()
    {
        Object.Destroy(gameObject);
    }
}
