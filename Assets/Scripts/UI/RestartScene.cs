using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class RestartScene : MonoBehaviour
{
    [SerializeField] private Button restartButton;

    public void RestartLevel()
    {
        SceneManager.LoadScene(0);
    }
}