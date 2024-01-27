using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ArenaManager : MonoBehaviour
{
    //singleton related
    private static ArenaManager instance;
    public static ArenaManager Instance
    {
        get { return instance; }
    }
    public List<PlayerController> players = new List<PlayerController>();
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

      
    }
}
