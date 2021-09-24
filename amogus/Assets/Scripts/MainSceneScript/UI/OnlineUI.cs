using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class OnlineUI : MonoBehaviour
{
    [SerializeField]
    private InputField nicknameInputField;
    [SerializeField]
    private GameObject createRoomUI;

    // make room
  public void OnClickCreateRoomBtn()
  {
        if(nicknameInputField.text != "")
        {
            PlayerSettings.nickname = nicknameInputField.text;
            createRoomUI.SetActive(true);
            gameObject.SetActive(false);
        }
        else
        {
            nicknameInputField.GetComponent<Animator>().SetTrigger("on");
        }
  }
    // join room
    public void OnClickEnterGameRoomBtn()
    {
        if (nicknameInputField.text != "")
        {
            // player nick name save
            PlayerSettings.nickname = nicknameInputField.text;

            var manager = AmongUsRoomManager.singleton;
            manager.StartClient();

        }
        else
        {
            nicknameInputField.GetComponent<Animator>().SetTrigger("on");
        }
    }


}
