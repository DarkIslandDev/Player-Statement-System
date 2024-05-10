using System;
using Cinemachine;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject cameraPrefab;
    
    [Header("Components")]
    [SerializeField] private DungeonGenerator dungeonGenerator;
    [SerializeField] private MinimapCamera minimapCamera;
    [SerializeField] private CinemachineVirtualCamera virtualCamera;
    
    private UIManager uiManager;
    private Player player;

    public static GameManager Instance;
    public Player PlayerPrefab => player;
    public CinemachineVirtualCamera VirtualCamera { get => virtualCamera; private set => virtualCamera = value; }
    public DungeonGenerator DungeonGenerator => dungeonGenerator;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }

        uiManager = UIManager.Instance;
        dungeonGenerator ??= GetComponent<DungeonGenerator>();
    }

    private void Start()
    {
        SpawnPlayerOnStart();
    }

    public void SpawnPlayerOnStart()
    {
        GameObject p = Instantiate(
            playerPrefab,
            dungeonGenerator.GetSafeRoomPosition(),
            playerPrefab.transform.localRotation);
            
        player = p.GetComponent<Player>();
        player.Init(virtualCamera, uiManager.InteractionButton);
    }
}