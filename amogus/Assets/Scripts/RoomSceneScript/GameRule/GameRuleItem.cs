using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRuleItem : MonoBehaviour
{
    [SerializeField]
    private GameObject inactiveObject;

    private void Start()
    {
        if(!AmongUsRoomPlayer.MyRoomPlayer.isServer)
        {
            inactiveObject.SetActive(false);
        }
    }


}
