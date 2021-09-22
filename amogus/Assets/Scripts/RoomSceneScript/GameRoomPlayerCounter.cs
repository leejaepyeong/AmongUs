using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class GameRoomPlayerCounter : NetworkBehaviour
{
    [SyncVar]
    private int minPlayer;
    [SyncVar]
    private int maxPlayer;

    [SerializeField]
    private Text playerCntTxt;

    public void UpdatePlayerCnt()
    {
        var player = FindObjectsOfType<AmongUsRoomPlayer>();
        bool isStartable = player.Length >= minPlayer;
        playerCntTxt.color = isStartable ? Color.white : Color.red;
        playerCntTxt.text = string.Format("{0}/{1}", player.Length, maxPlayer);

        LobbyUIManager.Instance.SetInteractableStartBtn(isStartable);
    }

    private void Start()
    {
        if(isServer)
        {
            var manager = NetworkManager.singleton as AmongUsRoomManager;
            minPlayer = manager.minPlayerCnt;
            maxPlayer = manager.maxConnections;
        }
    }
}
