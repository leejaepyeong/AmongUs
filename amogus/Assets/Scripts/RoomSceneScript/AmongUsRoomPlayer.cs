using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class AmongUsRoomPlayer : NetworkRoomPlayer
{

    // Find myRoomPlayer  by singleton
    // 자주 사용할 것 같으니 설정
    private static AmongUsRoomPlayer myRoomPlayer;

    public static AmongUsRoomPlayer MyRoomPlayer
    {
        get
        {
            if(myRoomPlayer == null)
            {
                var players = FindObjectsOfType<AmongUsRoomPlayer>();
                foreach(var player in players)
                {
                    if(player.hasAuthority)
                    {
                        myRoomPlayer = player;
                    }
                }

            }
            return myRoomPlayer;
        }
    }

    // 동기화
    //RoomPlayer 의 색이 벼경 될때마다 호출할 함수
    //UpdateColorButton 호출
    [SyncVar(hook = nameof(SetPlayerColor_Hook))]
    public EPlayerColor playerColor;
    public void SetPlayerColor_Hook(EPlayerColor oldColor, EPlayerColor newColor)
    {
        LobbyUIManager.Instance.customizeUI.UpdateUnSelectColorButton(oldColor);
        LobbyUIManager.Instance.customizeUI.UpdateSelectColorButton(newColor);
    }


    [SyncVar]
    public string nickName;

    public CharacterMover myCharacter;

    public void Start()
    {
        base.Start();

        // in Server
        if(isServer)
        {
            SpawnLobbyPlayerCharacter();
            LobbyUIManager.Instance.ActiveStartBtn(); // set active StartButton
        }

        if(isLocalPlayer)
        {
            CmdSetNickName(PlayerSettings.nickname);
        }

        LobbyUIManager.Instance.GameRoomPlayerCounter.UpdatePlayerCnt();
    }

    // When Player Exit
    // ColorBtn Setactive true
    private void OnDestroy()
    {
        if(LobbyUIManager.Instance != null)
        {
            LobbyUIManager.Instance.GameRoomPlayerCounter.UpdatePlayerCnt();
            LobbyUIManager.Instance.CustomizeUI.UpdateUnSelectColorButton(playerColor);
        }
            

    }

    [Command]
    public void CmdSetNickName(string nick)
    {
        nickName = nick;
        myCharacter.nickName = nick;
    }


    // Mirror API에서 제공하는 것으로
    // 클라이언트에서 함수를 호출하면 함수 내부의 동작이 서버에서 실행되도록 만들어줌
    //Cmd를 접두로 붙인다
    [Command]
    public void CmdSetPlayerColor(EPlayerColor color)
    {
        playerColor = color;
        myCharacter.playerColor = color;
    }

    // Player create
    private void SpawnLobbyPlayerCharacter()
    {
        // room Player 가져오기
        var roomSlots = (NetworkManager.singleton as AmongUsRoomManager).roomSlots;
        EPlayerColor color = EPlayerColor.Red;

        for (int i = 0; i < (int)EPlayerColor.Lime; i++)
        {
            bool isFindSameColor = false;

            foreach(var roomPlayer in roomSlots)
            {
                var amongUsRoomPlayer = roomPlayer as AmongUsRoomPlayer;
                if(amongUsRoomPlayer.playerColor == (EPlayerColor)i && roomPlayer.netId != netId)
                {
                    isFindSameColor = true;
                    break;
                }
            }

            if(!isFindSameColor)
            {
                color = (EPlayerColor)i;
                break;
            }
        }

        playerColor = color;

        var spawnPosition = FindObjectOfType<SpawnPositioner>();
        int index = spawnPosition.Index;
        Vector3 spawnPos = spawnPosition.GetSpawnPosition();
        

        var playerCharacter = Instantiate(AmongUsRoomManager.singleton.spawnPrefabs[0], spawnPos, Quaternion.identity).GetComponent<LobbyCharacterMover>();
        playerCharacter.transform.localScale = index < 5 ? new Vector3(0.6f, 0.6f, 1f) : new Vector3(-0.6f,0.6f,1f);
        NetworkServer.Spawn(playerCharacter.gameObject, connectionToClient);
        playerCharacter.ownerNetId = netId;
        playerCharacter.playerColor = color;
        // player spawn  connect
    }

}
