using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeetingPlayerPanel : MonoBehaviour
{
    [SerializeField]
    private Image characterImg;

    [SerializeField]
    private Text nicknameTxt;

    [SerializeField]
    GameObject deadPlayerBlock;

    [SerializeField]
    private GameObject reportSign;

    [SerializeField]
    private GameObject voteBtns;

    [SerializeField]
    private GameObject voteSign; // is Vote

    [SerializeField]
    private GameObject voterPrefab; // who vote

    [SerializeField]
    private Transform voterParentTransform;

    [HideInInspector]
    public InGameCharacterMover targetPlayer;


    public void UpdatePanel(EPlayerColor voterColor)
    {
        var voter = Instantiate(voterPrefab, voterParentTransform).GetComponent<Image>();

        voter.material = Instantiate(voter.material);
        voter.material.SetColor("_PlayerColor", PlayerColor.GetColor(voterColor));
        
    }

    public void OpenResult()
    {
        voterParentTransform.gameObject.SetActive(true);
    }

    public void UpdateVoteSign(bool isVoted)
    {
        voteSign.SetActive(isVoted);
    }

    public void SetPlayer(InGameCharacterMover target)
    {
        Material inst = Instantiate(characterImg.material);
        characterImg.material = inst;

        targetPlayer = target;
        characterImg.material.SetColor("_PlayerColor", PlayerColor.GetColor(targetPlayer.playerColor));
        nicknameTxt.text = target.nickName;

        var myCharacter = AmongUsRoomPlayer.MyRoomPlayer.myCharacter as InGameCharacterMover;
        if(((myCharacter.playerType & EPlayerType.Imposter) == EPlayerType.Imposter) && ((targetPlayer.playerType & EPlayerType.Imposter) == EPlayerType.Imposter))
        {
            nicknameTxt.color = Color.red;
        }

        bool isDead = (targetPlayer.playerType & EPlayerType.Ghost) == EPlayerType.Ghost;

        deadPlayerBlock.SetActive(isDead);
        GetComponent<Button>().interactable = !isDead;

        reportSign.SetActive(targetPlayer.isReporter);

    }

    public void OnClickPlayerPanel()
    {
        var myCharacter = AmongUsRoomPlayer.MyRoomPlayer.myCharacter as InGameCharacterMover;

        if (myCharacter.isVote) return;

        if((myCharacter.playerType & EPlayerType.Ghost) != EPlayerType.Ghost)
        {
            IngameUIManager.Instance.MeetingUI.SelectPlayerPanel();
            voteBtns.SetActive(true);
        }    
    }

  

    public void Select()
    {
        var myCharacter = AmongUsRoomPlayer.MyRoomPlayer.myCharacter as InGameCharacterMover;
        myCharacter.CmdVoteEjectPlayer(targetPlayer.playerColor);
        Unselect();
    }

    public void Unselect()
    {
        voteBtns.SetActive(false);
    }

}
