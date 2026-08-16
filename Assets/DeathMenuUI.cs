using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject deathPanel;

    void OnEnable()
    {
        GameStateManager.OnGameStateChanged += HandleGameStateChanged;
    }

    void OnDisable()
    {
        GameStateManager.OnGameStateChanged -= HandleGameStateChanged;
    }
    public void HandleGameStateChanged(GameState State)
    {
        if (State != GameState.Dead) return;

        deathPanel.SetActive(true);
    }

    public void RestartGame()
    {
        Debug.Log($"Restart Button pressed and Frame : {Time.frameCount} Instance ID : {GetInstanceID()} ");

        GameStateManager.Resume();

        GameEventManager.ResetState();

        Debug.Log("Before Load Scene-----------------1---------------------");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Debug.Log("After Load Scene-----------------2---------------------");
    }

}