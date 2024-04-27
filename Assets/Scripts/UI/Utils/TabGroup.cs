using System.Collections.Generic;
using UnityEngine;

public class TabGroup : MonoBehaviour
{
    [Header("Tab Colors")] 
    [SerializeField] private Color tabIdle;
    [SerializeField] private Color tabHover;
    [SerializeField] private Color tabActive;
    [Space] 
    [SerializeField] private List<ButtonTab> tabButtons;
    [SerializeField] private ButtonTab selectedButtonTab;

    private void Start()
    {
        GetChildTabButtons();

        if (selectedButtonTab == null)
        {
            selectedButtonTab = tabButtons[0];
        }

        foreach (ButtonTab tabButton in tabButtons)
        {
            tabButton.TabGroup = this;

            if (tabButton != null)
            {
                Subscribe(tabButton);
            }
        }
    }

    private void GetChildTabButtons()
    {
        foreach (Transform child in transform)
        {
            ButtonTab buttonTab = child.GetComponent<ButtonTab>();
            buttonTab.TabGroup = this;

            Subscribe(buttonTab);
        }
    }

    public void Subscribe(ButtonTab buttonTab)
    {
        if (!buttonTab.Subscribed)
        {
            buttonTab.Subscribed = true;
            tabButtons.Add(buttonTab);
        }
    }

    public void OnTabEnter(ButtonTab buttonTab)
    {
        ResetTabs();

        if (selectedButtonTab == null || buttonTab != selectedButtonTab)
        {
            buttonTab.BackGroundImage.color = tabHover;
        }
    }

    public void OnTabSelected(ButtonTab buttonTab)
    {
        if (selectedButtonTab != buttonTab)
        {
            selectedButtonTab.Deselect();
        }

        selectedButtonTab = buttonTab;

        selectedButtonTab.Select();

        ResetTabs();

        selectedButtonTab.BackGroundImage.color = tabActive;
    }

    public void OnTabExit() => ResetTabs();

    private void ResetTabs()
    {
        foreach (ButtonTab tabButton in tabButtons)
        {
            if (tabButton != selectedButtonTab)
            {
                tabButton.BackGroundImage.color = tabIdle;
            }
        }
    }
}