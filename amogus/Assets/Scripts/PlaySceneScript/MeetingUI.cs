using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum EMeetingState
{
    None,
    Meeting,
    Vote
}

public class MeetingUI : MonoBehaviour
{
    [SerializeField]
    private GameObject playerPanelPrefab;

    [SerializeField]
    private Transform playerPanelsParent;

    [SerializeField]
    private GameObject voterPrefab;

    [SerializeField]
    private GameObject skipVoteBtn;

    [SerializeField]
    private GameObject skipVotePlayers;

    [SerializeField]
    private Transform skipVotePlayerTransform;

    [SerializeField]
    private Text meetingTimeTxt;

    private EMeetingState meetingState;

    private List<MeetingPlayerPanel> meetingPlayerPanels = new List<MeetingPlayerPanel>();

    public void Open()
    {
        var myCharacter = AmongUsRoomPlayer.MyRoomPlayer.myCharacter as InGameCharacterMover;
        var myPanel = Instantiate(playerPanelPrefab, playerPanelsParent).GetComponent<MeetingPlayerPanel>();
        myPanel.SetPlayer(myCharacter);
        meetingPlayerPanels.Add(myPanel);

        gameObject.SetActive(true);

        var players = FindObjectsOfType<InGameCharacterMover>();

        foreach(var player in players)
        {
            if(player != myCharacter)
            {
                var panel = Instantiate(playerPanelPrefab, playerPanelsParent).GetComponent<MeetingPlayerPanel>();
                panel.SetPlayer(player);
                meetingPlayerPanels.Add(panel);
            }
        }
    }

    public void ChangeMeetingState(EMeetingState state)
    {
        meetingState = state;
    }

    public void SelectPlayerPanel()
    {
        foreach(var panel in meetingPlayerPanels)
        {
            panel.Unselect();
        }
    }

    public void UpdateVote(EPlayerColor voterColor, EPlayerColor ejectColor)
    {
        foreach(var panel in meetingPlayerPanels)
        {
            //투표 받은 자
            if(panel.targetPlayer.playerColor == ejectColor)
            {
                panel.UpdatePanel(voterColor);
            }

            // 투표 한 자
            if(panel.targetPlayer.playerColor == voterColor)
            {
                panel.UpdateVoteSign(true);
            }
        }
    }


    public void UpdateSkipVotePlayer(EPlayerColor skipVotePlayerColor)
    {
        foreach(var panel in meetingPlayerPanels)
        {
            if(panel.targetPlayer.playerColor == skipVotePlayerColor)
            {
                panel.UpdateVoteSign(true);
            }

            
        }

        var voter = Instantiate(voterPrefab, skipVotePlayerTransform).GetComponent<Image>();
        voter.material = Instantiate(voter.material);
        voter.material.SetColor("_PlayerColor", PlayerColor.GetColor(skipVotePlayerColor));
        
    }

    public void OnClickSkipVoteBtn()
    {
        var myCharacter = AmongUsRoomPlayer.MyRoomPlayer.myCharacter as InGameCharacterMover;
        if (myCharacter.isVote) return;

        myCharacter.CmdSkipVote();
        SelectPlayerPanel();

    }

    public void CompleteVote()
    {
        foreach (var panel in meetingPlayerPanels)
        {
            panel.OpenResult();
        }

        skipVoteBtn.SetActive(false);
        skipVotePlayers.SetActive(true);
    }

    private void Update()
    {
        if(meetingState == EMeetingState.Meeting)
        {
            meetingTimeTxt.text = string.Format("회의시간 : {0}s", (int)Mathf.Clamp(GameSystem.Instace.remainTime,0f,float.MaxValue));
        }
        else if(meetingState == EMeetingState.Vote)
        {
            meetingTimeTxt.text = string.Format("투표시간 : {0}s", (int)Mathf.Clamp(GameSystem.Instace.remainTime , 0f, float.MaxValue));
        }
    }
}
