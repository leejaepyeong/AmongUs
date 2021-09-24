using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

// 0x : 0 crew 1 - impo  이진법
// x0 : 0 - live 1 - death
// 00 01  01 11
public enum EPlayerType
{
    Crew = 0,
    Imposter = 1,
    Ghost = 2,
    Crew_Alive = 0,
    Imposter_Alive = 1,
    Crew_Ghost = 2,
    Imposter_Ghost = 3
    
}

public class InGameCharacterMover : CharacterMover
{
    [SyncVar(hook = nameof(SetPlayerType_Hook))]
    public EPlayerType playerType;

    // playerType이 변경 시 호출 될 함수
    private void SetPlayerType_Hook(EPlayerType _, EPlayerType type)
    {
        if(hasAuthority && type == EPlayerType.Imposter)
        {
            IngameUIManager.Instance.KillButtonUI.Show(this);
            playerFinder.SetKillRange(GameSystem.Instace.killRange + 1f);
        }
    }

    [SerializeField]
    private PlayerFinder playerFinder;

    [SyncVar]
    private float killCooldown;
    public float KillCooldown { get { return killCooldown; } }

    public bool isKillable { get { return killCooldown < 0f && playerFinder.targets.Count != 0; } }    // cool time 초기화

    public EPlayerColor foundDeadBodyColor;

    [SyncVar]
    public bool isReporter = false;

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

    public void SetKillCooldown()
    {
        if(isServer)
        {
            killCooldown = GameSystem.Instace.killCooldown;
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

    private void Update()
    {
        if(isServer && playerType == EPlayerType.Imposter)
        {
            killCooldown -= Time.deltaTime;
        }
    }

    public void Kill()
    {
        CmdKill(playerFinder.GetFirstTarget().netId);
    }

    [Command]
    private void CmdKill(uint targetNetId)
    {
        InGameCharacterMover target = null;

        foreach(var player in GameSystem.Instace.GetPlayerList())
        {
            if(player.netId == targetNetId)
            {
                target = player;
            }
        }

        if(target != null)
        {
            RpcTeleport(target.transform.position); // target position teleport

            target.Dead(playerColor);
            killCooldown = GameSystem.Instace.killCooldown;
        }
        
    }

    public void Dead(EPlayerColor imposterColor)
    {
        playerType = EPlayerType.Ghost;
        RpcDead(imposterColor, playerColor);
        var manager = NetworkManager.singleton as AmongUsRoomManager;
        var deadbody = Instantiate(manager.spawnPrefabs[1], transform.position, transform.rotation).GetComponent<DeadBody>();
        NetworkServer.Spawn(deadbody.gameObject);
        deadbody.RpcSetColor(playerColor);
    }

    [ClientRpc]
    private void RpcDead(EPlayerColor imposterColor, EPlayerColor crewColor)
    {
        if(hasAuthority)
        {
            anim.SetBool("isGhost",true);
            IngameUIManager.Instance.KillUI.Open(imposterColor, crewColor);

            var players = GameSystem.Instace.GetPlayerList();
            foreach(var player in players)
            {
                if((player.playerType & EPlayerType.Ghost) == EPlayerType.Ghost)
                {
                    player.SetVisibility(true);
                }
            }

            GameSystem.Instace.ChangeLightMode(EPlayerType.Ghost);
        }
        else
        {
            var myPlayer = AmongUsRoomPlayer.MyRoomPlayer.myCharacter as InGameCharacterMover;
            if(((int)myPlayer.playerType & 0x02) != (int)EPlayerType.Ghost)
            {
                SetVisibility(false);
            }
                   
        }

        var collider = GetComponent<BoxCollider2D>();
        if(collider)
        {
            collider.enabled = false;
        }
    }

    public void Report()
    {
        CmdReport(foundDeadBodyColor);
    }

    [Command]
    public void CmdReport(EPlayerColor deadbodyColor)
    {
        isReporter = true;
        GameSystem.Instace.StartReportMeeing(deadbodyColor);
    }


    public void SetVisibility(bool isVisable)
    {
        if(isVisable)
        {
            var color = PlayerColor.GetColor(playerColor);
            color.a = 1f;
            spriteRenderer.material.SetColor("_PlayerColor", color);
            nicknameTxt.text = nickName;
        }
        else
        {
            var color = PlayerColor.GetColor(playerColor);
            color.a = 0f;
            spriteRenderer.material.SetColor("_PlayerColor", color);
            nicknameTxt.text = "";
        }
    }

}
