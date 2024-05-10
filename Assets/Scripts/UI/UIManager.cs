using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject guiMenu;
    [SerializeField] private GameObject interactionButton;
    
    public static UIManager Instance { get; private set; }
    public GameObject GUIMenu { get => guiMenu; set => guiMenu = value; }
    public GameObject InteractionButton { get => interactionButton; set => interactionButton = value; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }
}