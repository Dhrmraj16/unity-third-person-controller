using UnityEngine;
using UnityEngine.SceneManagement;
public class DeathMenu : MonoBehaviour
{
    
    public void DeathMenuShow()
    {
        gameObject.SetActive(true);
        //GameStateManager.SetState(GameState.Dead);
    }

    public void DeathRestart()
    {
        gameObject.SetActive(false);
        GameStateManager.SetState(GameState.Playing);
        GameEventManager.ResetState();
        DamageSystem.ResetState();
        Debug.Log("[DeathMenu] Scene Loaded and Game Restarted : ");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
