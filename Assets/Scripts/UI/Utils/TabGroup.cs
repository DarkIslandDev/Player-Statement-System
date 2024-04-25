using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class TabGroup : MonoBehaviour
{
    [Header("Tab Colors")] 
    [SerializeField] private Color tabIdle;
    [SerializeField] private Color tabHover;
    [SerializeField] private Color tabActive;
    [Space]
    [SerializeField] private List<TabButton> tabButtons;
    
    private GameObject[] objectsToSwap;
    private TabButton selectedTab;

    private void Start()
    {
        selectedTab = tabButtons[0];
        objectsToSwap = GetObjectsToSwap().ToArray();
        objectsToSwap[0].SetActive(true);
    }
    
    public void Subscribe(TabButton tabButton) => tabButtons ??= new List<TabButton> { tabButton };

    public void OnTabEnter(TabButton tabButton)
    {
        ResetTabs();

        if (selectedTab == null || tabButton != selectedTab)
        {
            tabButton.BackGroundImage.color = tabHover;
        }
    }

    public void OnTabSelected(TabButton tabButton)
    {
        selectedTab = tabButton;
        
        ResetTabs();

        tabButton.BackGroundImage.color = tabActive;

        int index = tabButton.transform.GetSiblingIndex();

        for (int i = 0; i < objectsToSwap.Length; i++)
        {
            objectsToSwap[i].SetActive(i == index);
        }
    }

    public void OnTabExit() => ResetTabs();

    private void ResetTabs()
    {
        foreach (TabButton tabButton in tabButtons)
        {
            if (tabButton != selectedTab)
            {
                tabButton.BackGroundImage.color = tabIdle;
            }
        }
    }
    
    private IEnumerable<GameObject> GetObjectsToSwap()
    {
        foreach (Transform child in transform)
        {
            if (child.gameObject.activeSelf)
            {
                continue;
            }

            yield return child.gameObject;
        }
    }
}