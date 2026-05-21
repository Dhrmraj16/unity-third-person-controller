using UnityEngine;

public class GameInput : MonoBehaviour
{
    [SerializeField] PauseMenu pausePanel;
    [SerializeField] DeathMenu deathPanel;
    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.F))
        {
            GameEventManager.RaiseOnAttackInput();
        }
        */
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameStateManager.IsPlaying())
            {
                pausePanel.Show();
            }
            else if(GameStateManager.IsPaused())
            {
                pausePanel.Resume();
            } 
            else
            if (GameStateManager.IsDead())
            {
                deathPanel.DeathMenuShow();

            }

        }
        
    
    }
   


}
