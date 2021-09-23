using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public enum EPlayerType
{
    Crew,
    Imposter
}

public class InGameCharacterMover : CharacterMover
{
    [SyncVar]
    public EPlayerType playerType;

    // player position 동기화
    [ClientRpc]
    public void RpcTeleport(Vector3 position)
    {
        transform.position = position;
    }

    // Imposter가 서로 구분하기 위해 텍스트 색
    public void SetNicknameColor(EPlayerType type)
    {
        if(playerType == EPlayerType.Imposter && type == EPlayerType.Imposter)
        {
            nicknameTxt.color = Color.red;
        }
    }

    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        if(hasAuthority)
        {
            IsMoveable = true;

            var myRoomPlyer = AmongUsRoomPlayer.MyRoomPlayer;
            myRoomPlyer.myCharacter = this;
            CmdSetPlayerCharacter(myRoomPlyer.nickName, myRoomPlyer.playerColor);

        }

        GameSystem.Instace.AddPlayer(this);
    }


    [Command]
    private void CmdSetPlayerCharacter(string nickname, EPlayerColor color)
    {
        this.nickName = nickname;
        playerColor = color;
    }

    
}
