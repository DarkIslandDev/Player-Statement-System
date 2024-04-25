using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonSpriteSwapper : MonoBehaviour
{
    private bool chillState = true;
    private bool selectedState = false;
    private bool pressedState = false;
    private bool highlightState = false;
    private bool disabledState = false;
    private Color chillColor = new Color(255,255,255,255);
    private Color pressedColor = new Color(100,100,100,255);
    private Color highlightColor = new Color(128, 128, 128, 255);
    private Color disabledColor = new Color(0, 0, 0, 255);
    
    public Image TargetGraphic { get; set; }
    public Image SelectedSprite { get; set; }
    
    
    private void Awake()
    {
        TargetGraphic ??= gameObject.transform.GetChild(0).GetComponent<Image>();
        SelectedSprite ??= gameObject.transform.GetChild(1).GetComponent<Image>();
    }

    private void Update()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        
        CheckForSwapSprite();
    }

    private void CheckForSwapSprite()
    {
        if (Input.mousePosition == transform.position)
        {
            selectedState = true;
            highlightState = true;

            if (Input.GetMouseButtonDown(0))
            {
                pressedState = true;
                highlightState = false;
            }
        }
        
        if (chillState == true || disabledState == true)
        {
            selectedState = false;
        }
        else
        {
            selectedState = true;
        }

        if (selectedState == true)
        {
            SelectedSprite.gameObject.SetActive(selectedState);
        }

        if (disabledState == true)
        {
            TargetGraphic.color = disabledColor;
        }
        else if (highlightState == true)
        {
            TargetGraphic.color = highlightColor;
        }
        else if (pressedState == true)
        {
            TargetGraphic.color = pressedColor;
        }
        else
        {
            TargetGraphic.color = chillColor;
        }
    }
}
