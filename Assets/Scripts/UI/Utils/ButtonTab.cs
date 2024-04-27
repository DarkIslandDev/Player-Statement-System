using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class ButtonTab : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{

    public TabGroup TabGroup;
    public Image BackGroundImage;
    public GameObject MenuGameObject;
    public AudioSource SelectSound;

    public UnityEvent OnTabSelected;
    public UnityEvent OnTabDeselected;

    public bool Subscribed { get; set; }

    private void Start()
    {
        BackGroundImage ??= GetComponent<Image>();
    }

    public void Select()
    {
        OnTabSelected.Invoke();

        if (SelectSound != null)
        {
            SelectSound.Play();
        }

        if (MenuGameObject != null)
        {
            MenuGameObject.SetActive(true);
        }
    }

    public void Deselect()
    {
        OnTabDeselected?.Invoke();

        if (MenuGameObject != null)
        {
            MenuGameObject.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData) => TabGroup.OnTabEnter(this);

    public void OnPointerClick(PointerEventData eventData) => TabGroup.OnTabSelected(this);

    public void OnPointerExit(PointerEventData eventData) => TabGroup.OnTabExit();
}