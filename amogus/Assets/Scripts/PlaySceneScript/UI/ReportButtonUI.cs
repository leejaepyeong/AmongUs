using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ReportButtonUI : MonoBehaviour
{
    [SerializeField]
    private Button reportBtn;

    public void SetInteractable(bool isInteractable)
    {
        reportBtn.interactable = isInteractable;
    }

    // Push Report Button
    public void OnClickBtn()
    {
        var character = AmongUsRoomPlayer.MyRoomPlayer.myCharacter as InGameCharacterMover;
        character.Report();
    }
}
