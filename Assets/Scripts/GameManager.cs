using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject cameraPrefab;
    
    [Header("Components")]
    [SerializeField] private DungeonGenerator dungeonGenerator;
    [SerializeField] private MinimapCamera minimapCamera;
    
    private UIManager uiManager;
    private Player player;
    private CameraHandler cameraHandler;

    public static GameManager Instance;
    public Player PlayerPrefab => player;
    public CameraHandler CameraHandler => cameraHandler;
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
    }

    private void Start()
    {
        SpawnPlayerOnStart();
    }

    private void SpawnPlayerOnStart()
    {
        if (playerPrefab != null)
        {
            GameObject p = Instantiate(
                playerPrefab, 
                dungeonGenerator.GetSafeRoomPosition(), 
                playerPrefab.transform.localRotation);
            player = p.GetComponent<Player>();

            GameObject c = Instantiate(
                cameraPrefab,
                 Vector3.zero,
                  Quaternion.identity);
            cameraHandler = c.GetComponent<CameraHandler>();

            player.Init(cameraHandler, minimapCamera, uiManager.GUIMenu);
            cameraHandler.Init(player.transform);
        }
    }
}