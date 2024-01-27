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
    private int playersConnected = 0;
    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        for (int i = 0; i < GameManager.Devices.Count; i++)
        {
            if (i >= players.Count)
                break;

            RegisterPlayer(GameManager.Devices[i].deviceId);
        }
    }

    public void RegisterPlayer(int deviceID)
    {
        if (playersConnected >= players.Count)
        {
            Debug.LogWarning("no caben mas jugadores");
            return;
        }
        var p = players.FirstOrDefault(player => player.LinkedDeviceID == -1);
        if (p != null)
        {
            p.LinkDevice(deviceID);
            playersConnected++;
        }
        else
        {
            Debug.Log("no more amigo");
        }
    }
    public void UnRegisterPlayer(int deviceID)
    {
        var p = players.FirstOrDefault(player => player.LinkedDeviceID == deviceID);

        if (p != null)
        {
            p.LinkDevice(-1);
            playersConnected--;
        }
        else
        {
            Debug.Log("player not in arena");
        }
    }
}
