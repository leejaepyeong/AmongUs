using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CreateRoomUI : MonoBehaviour
{
    [SerializeField]
    private List<Image> crewImg; // player Img


    [SerializeField]
    private List<Button> imposterCntBtns; // imposter count button

    [SerializeField]
    private List<Button> maxPlayerCntBtns; // max player count button

    private CreateGameRoomData roomData;


    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < crewImg.Count; i++)
        {
            Material materialInstance = Instantiate(crewImg[i].material);
            crewImg[i].material = materialInstance;
        }

        roomData = new CreateGameRoomData() { imposterCnt = 1, maxPlayerCnt = 10 };
        UpdateCrewImages();
    }


    public void UpdateImposterCnt(int _cnt)
    {
        roomData.imposterCnt = _cnt;

        for (int i = 0; i < imposterCntBtns.Count; i++)
        {
            if(i == _cnt - 1)
            {
                imposterCntBtns[i].image.color = new Color(1f, 1f, 1f, 1f);
            }
            else
            {
                imposterCntBtns[i].image.color = new Color(1f, 1f, 1f, 0f);
            }
        }

        // set maxPlayer by imposter count
        int limitMaxPlayer = _cnt == 1 ? 4 : _cnt == 2 ? 7 : 9;

        if(roomData.maxPlayerCnt < limitMaxPlayer)
        {
            UpdateMaxPlayerCnt(limitMaxPlayer);
        }
        else
        {
            UpdateMaxPlayerCnt(roomData.maxPlayerCnt);
        }

        for (int i = 0; i < maxPlayerCntBtns.Count; i++)
        {
            var text = maxPlayerCntBtns[i].GetComponentInChildren<Text>();

            if(i < limitMaxPlayer - 4)
            {
                maxPlayerCntBtns[i].interactable = false;
                text.color = Color.gray;
            }
            else
            {
                maxPlayerCntBtns[i].interactable = true;
                text.color = Color.white;
            }
        }
    }

    public void UpdateMaxPlayerCnt(int _cnt)
    {
        roomData.maxPlayerCnt = _cnt;

        for (int i = 0; i < maxPlayerCntBtns.Count; i++)
        {
            if(i == _cnt - 4)
            {
                maxPlayerCntBtns[i].image.color = new Color(1f,1f,1f,1f);

            }
            else
            {
                maxPlayerCntBtns[i].image.color = new Color(1f,1f,1f,0f);
            }
        }

        UpdateCrewImages();

    }


    // imposter : red / other : white
    private void UpdateCrewImages()
    {
        for (int i = 0; i < crewImg.Count; i++)
        {
            crewImg[i].material.SetColor("_PlayerColor", Color.white);
        }

        int imposterCnt = roomData.imposterCnt;
        int idx = 0;

        while(imposterCnt != 0)
        {
            if(idx >= roomData.maxPlayerCnt)
            {
                idx = 0;
            }

            if(crewImg[idx].material.GetColor("_PlayerColor") != Color.red && Random.Range(0,5) == 0)
            {
                crewImg[idx].material.SetColor("_PlayerColor", Color.red);
                imposterCnt--;
            }
            idx++;
            
        }

        for (int i = 0; i < crewImg.Count; i++)
        {
            if(i<roomData.maxPlayerCnt)
            {
                crewImg[i].gameObject.SetActive(true);
            }
            else
            {
                crewImg[i].gameObject.SetActive(false);
            }
        }
    }

    public void CreateRoom()
    {
        var manager = NetworkManager.singleton as AmongUsRoomManager;
        // setting Room

        manager.minPlayerCnt = roomData.imposterCnt == 1 ? 4 : roomData.imposterCnt == 2 ? 7 : 9;
        manager.imposterCnt = roomData.imposterCnt;
        manager.maxConnections = roomData.maxPlayerCnt;

        manager.StartHost();
    }

}

public class CreateGameRoomData
{
    public int imposterCnt;
    public int maxPlayerCnt;

}