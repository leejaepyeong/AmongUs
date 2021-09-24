using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KillButtonUI : MonoBehaviour
{
    [SerializeField]
    private Button killBtn;

    [SerializeField]
    private Text cooldownTxt;

    private InGameCharacterMover targetPlayer;

    public void Show(InGameCharacterMover player)
    {
        gameObject.SetActive(true);
        targetPlayer = player;
    }

    private void Update()
    {
        if(targetPlayer != null)
        {
            if(!targetPlayer.isKillable)
            {
                cooldownTxt.text = targetPlayer.KillCooldown > 0 ? ((int)targetPlayer.KillCooldown).ToString() : "";
                killBtn.interactable = false;
            }
            else
            {
                cooldownTxt.text = "";
                killBtn.interactable = true;
            }
        }
    }


    public void OnClickKillBtn()
    {
        targetPlayer.Kill();
    }
}
