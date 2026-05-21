using Unity.Hierarchy;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenu : MonoBehaviour
{
    
    public void Show()
    {
        gameObject.SetActive(true);
        GameStateManager.Paused();
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Resume()
    {
        
        Hide();
        GameStateManager.Resume();
    }


    public void Restart()
    {
        gameObject.SetActive(false);
        GameEventManager.ResetState();
        DamageSystem.ResetState();
        GameStateManager.SetState(GameState.Playing);
        Debug.Log("[PauseMenu] Scene Loaded and Game Restarted : ");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}



