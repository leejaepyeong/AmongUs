using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameSystem : NetworkBehaviour
{
    public static GameSystem Instace;

    private List<InGameCharacterMover> players = new List<InGameCharacterMover>();

    [SerializeField]
    private Transform spawnTransform;

    [SerializeField]
    private float spawnDistance;

    public List<InGameCharacterMover> GetPlayerList()
    {
        return players;
    }

    private void Awake()
    {
        Instace = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        if(isServer)
        StartCoroutine(GameReady());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
      
    public void AddPlayer(InGameCharacterMover player)
    {
        if(!players.Contains(player))
        {
            players.Add(player);
        }
    }


    // set Imposter
    private IEnumerator GameReady()
    {
        var manager = NetworkManager.singleton as AmongUsRoomManager;
        while(manager.roomSlots.Count != players.Count)
        {
            yield return null;
        }

        for (int i = 0; i < manager.imposterCnt; i++)
        {
            var player = players[Random.Range(0, players.Count)];

            if(player.playerType != EPlayerType.Imposter)
            {
                player.playerType = EPlayerType.Imposter;
            }
            else
            {
                i--;
            }
        }

        for (int i = 0; i < players.Count; i++)
        {
            float radian = (2f * Mathf.PI) / players.Count; // divide Circle by player count
            radian *= i;
            players[i].RpcTeleport(spawnTransform.position + (new Vector3(Mathf.Cos(radian), Mathf.Sin(radian), 0f) * spawnDistance));
        }

        yield return new WaitForSeconds(1f);

        RpcStartGame();

    }

    // host local 동시에 불려야하므로 
    [ClientRpc]
    private void RpcStartGame()
    {
        StartCoroutine(StartGameCoroutine());

    }

    private IEnumerator StartGameCoroutine()
    {
        yield return StartCoroutine(IngameUIManager.Instance.IngameIntroUI.ShowIntroSequence());

        InGameCharacterMover myCharacter = null;
        foreach(var player in players)
        {
            myCharacter = player;
            break;
        }

        foreach(var player in players)
        {
            player.SetNicknameColor(myCharacter.playerType);
        }


        yield return new WaitForSeconds(3f);

        // shh, crew imposter 창 닫기
        IngameUIManager.Instance.IngameIntroUI.Close();
    }
}
