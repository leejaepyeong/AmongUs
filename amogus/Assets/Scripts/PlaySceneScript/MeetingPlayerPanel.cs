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

    public InGameCharacterMover targetPlayer;


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

        deadPlayerBlock.SetActive((targetPlayer.playerType & EPlayerType.Ghost) == EPlayerType.Ghost);

        reportSign.SetActive(targetPlayer.isReporter);

    }
}
