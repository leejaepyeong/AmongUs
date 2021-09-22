using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class InGameCharacterMover : CharacterMover
{
    // Start is called before the first frame update
    public override void Start()
    {
        base.Start();

        if(hasAuthority)
        {
            IsMoveable = true;

            var myRoomPlyer = AmongUsRoomPlayer.MyRoomPlayer;
            CmdSetPlayerCharacter(myRoomPlyer.nickName, myRoomPlyer.playerColor);

        }
    }


    [Command]
    private void CmdSetPlayerCharacter(string nickname, EPlayerColor color)
    {
        this.nickName = nickName;
        playerColor = color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
