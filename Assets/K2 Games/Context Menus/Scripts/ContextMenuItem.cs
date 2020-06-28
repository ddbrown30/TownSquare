using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class ContextMenuItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public delegate void StandardDelegate();

    public TMP_Text textLabel;
    public ContextMenu parentMenu, subMenu;//Which menu this is a part of and which sub menu is opened when selected
    public TMP_Text subMenuArrow;//enabled when there is a sub menu

    [HideInInspector]
    public bool keepMenuOpen = false;

    StandardDelegate onClickMethod;

    public void Initialise(ContextMenu parentMenu, string description, StandardDelegate onClickMethod, ContextMenu subMenu)
    {
        transform.name = description;
        this.parentMenu = parentMenu;
        textLabel.text = description;
        this.onClickMethod = onClickMethod;
        this.subMenu = subMenu;

        if(subMenu == null)
            subMenuArrow.gameObject.SetActive(false);//hide the arrow that would show more options
        else
            subMenu.parentMenu = parentMenu;//show our menu has a parent
    }

    public void OnClick()
    {
        if(subMenu == null)//don't bother clicking menu's with sub items
            if(onClickMethod != null)
                onClickMethod.Invoke();
    }

    #region Mouse Over
    public void OnPointerEnter(PointerEventData eventData)
    {
        keepMenuOpen = false;

        if(parentMenu != null && parentMenu.selector != null)
            if(!parentMenu.selector.activeSelf)
                parentMenu.selector.SetActive(true);

        if(parentMenu.parentMenu != null)
            parentMenu.parentMenu.SupressHiding();

        parentMenu.CloseOtherSubMenus(this);

        parentMenu.selector.transform.position = transform.position;

        if(subMenu != null && !subMenu.gameObject.activeSelf)
            StartCoroutine(HoverFinished());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if(subMenu != null)
        {
            StopAllCoroutines();
            StartCoroutine(Hide());
        }
    }

    IEnumerator HoverFinished()
    {
        yield return new WaitForSeconds(ContextMenu.HoverTimer);

        Vector2 highestPivot = ContextMenu.GetHighestMenu(parentMenu).pivot;//grab the parent menu's pivot, determine if we are moving left or right

        subMenu.transform.position = transform.position;//start in the same place as the menu item clicked
        Vector3 startPosition = subMenu.transform.localPosition;
        
        startPosition += new Vector3(((RectTransform)parentMenu.transform).sizeDelta.x * 0.5f, 0, 0);//move out from the middle of the menu to the side

        float padding = ((RectTransform)transform).sizeDelta.y * 0.5f + subMenu.menuLayout.padding.bottom;//calculate any paddings

        if(ContextMenu.startingClickPosition.y > Screen.height / 2)//if starting on the top of the screen
            startPosition.y -= ((RectTransform)subMenu.transform).sizeDelta.y - padding;
        else
            startPosition.y -= padding;

        if(ContextMenu.startingClickPosition.x > Screen.width / 2)//if starting on the right hand side of the screen
            startPosition.x -= ((RectTransform)subMenu.transform).sizeDelta.x * 2;

        subMenu.transform.localPosition = startPosition;

        subMenu.Show();
    }

    IEnumerator Hide()
    {
        yield return new WaitForSeconds(ContextMenu.HoverTimer * 2);

        if(!keepMenuOpen)
            subMenu.Hide();
    }
    #endregion
}
