using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Mirror;

public class LobbyUIManager : MonoBehaviour
{

    public static LobbyUIManager Instance;

    [SerializeField]
    public CustomizeUI customizeUI;

    public CustomizeUI CustomizeUI { get { return customizeUI; } }

    [SerializeField]
    private Button useBtn;
    [SerializeField]
    private Sprite originUseBtnSprite;

    [SerializeField]
    private GameRoomPlayerCounter gameRoomPlayerCounter;
    public GameRoomPlayerCounter GameRoomPlayerCounter { get { return gameRoomPlayerCounter; } }

    [SerializeField]
    private Button startBtn;

    private void Awake()
    {
        Instance = this;
    }

   // Use Btn setActive true
    public void SetUseBtn(Sprite sprite, UnityAction action)
    {
        useBtn.image.sprite = sprite;
        useBtn.onClick.AddListener(action);
        useBtn.interactable = true;
    }

    // Use Btn setActive false
    public void UnSetUseBtn()
    {
        useBtn.image.sprite = originUseBtnSprite;
        useBtn.onClick.RemoveAllListeners();
        useBtn.interactable = false;
    }

    public void ActiveStartBtn()
    {
        startBtn.gameObject.SetActive(true);
    }

    public void SetInteractableStartBtn(bool isInteractable)
    {
        startBtn.interactable = isInteractable;
    }

    public void OnClickStartBtn()
    {
        var players = FindObjectsOfType<AmongUsRoomPlayer>();
        for (int i = 0; i < players.Length; i++)
        {
            players[i].readyToBegin = true;
        }
        // All Player Ready

        // Change Scene to PlayScene
        var manager = NetworkManager.singleton as AmongUsRoomManager;
        manager.ServerChangeScene(manager.GameplayScene);
    }
}
