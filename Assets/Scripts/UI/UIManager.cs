using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject guiMenu;

    public static UIManager Instance { get; private set; }
    public GameObject GUIMenu { get => guiMenu; set => guiMenu = value; }

    void Awake()
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