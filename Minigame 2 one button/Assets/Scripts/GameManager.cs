using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    //data we save for rewind system
    public bool key = false;
    

    //awake
    private void Awake()
    {
        if (Instance == null)
        //if no gamemanager exist
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else{
            Destroy(gameObject);
        }
    }
}