using UnityEngine;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance;
    
    void Awake()
    {
        Debug.Log("[GameSession] [Awake] Called : ");
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
    }
    void Start()
    {
        Debug.Log("[GameSession] Start Called :");
        //GameStateManager.SetState(GameState.Playing);
    }

   

}