using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class CharacterMover : NetworkBehaviour
{
    private bool isMoveable;

    public bool IsMoveable
    {
        get
        {
            return isMoveable;
        }
        set
        {
            if(!value)
            {
                anim.SetBool("isWalk",false);
            }
            isMoveable = value;
        }
    }

    [SyncVar] // netwirk 동기화
    public float speed = 2.0f;

    [SerializeField]
    private float characterSize = 0.6f;

    [SerializeField]
    private float cameraSize = 2.5f;

    private SpriteRenderer spriteRenderer;

    // SyncBar로 동기화 된 변수가 서버에서 변경 시 hook로 등록 된 함수가 클라이언트에서 호출
    // if change playercolor  SetPlayerColor_Hook  return
    [SyncVar(hook = nameof(SetPlayerColor_Hook))]
    public EPlayerColor playerColor;

    // Player Name
    [SyncVar(hook = nameof(SetNickName_Hook))]
    public string nickName;
    [SerializeField]
    protected Text nicknameTxt;

    public void SetNickName_Hook(string _, string value)
    {
        nicknameTxt.text = value;
    }


    private Animator anim;

    // Start is called before the first frame update
    public virtual void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.material.SetColor("_PlayerColor",PlayerColor.GetColor(playerColor));

        anim = GetComponent<Animator>();

        // cam set
        if(hasAuthority)
        {
            Camera cam = Camera.main;
            cam.transform.SetParent(transform);
            cam.transform.localPosition = new Vector3(0f,0f,-10f);
            cam.orthographicSize = cameraSize;
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        Move();
    }

    // player Move
    public void Move()
    {
        // 권한 유무 확인
        if(hasAuthority && isMoveable)
        {
            bool isMove = false;

            // ketboard & mouse  or  only mouse
            if(PlayerSettings.controlType == EControlType.KeyboardMouse)
            {
                Vector3 dir = Vector3.ClampMagnitude(new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f), 1f);
                if (dir.x < 0f) transform.localScale = new Vector3(-1 * characterSize, characterSize, 1f);
                else if (dir.x > 0f) transform.localScale = new Vector3(characterSize, characterSize, 1f);

                transform.position += dir * speed * Time.deltaTime;

                isMove = dir.magnitude != 0f;
            }
            else
            {
                if(Input.GetMouseButton(0))
                {
                    Vector3 dir = (Input.mousePosition - new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f)).normalized;
                    if (dir.x < 0f) transform.localScale = new Vector3(-1 * characterSize, characterSize, 1f);
                    else if (dir.x > 0f) transform.localScale = new Vector3(characterSize, characterSize, 1f);

                    transform.position += dir * speed * Time.deltaTime;

                    isMove = dir.magnitude != 0f;
                }
            }
            anim.SetBool("isMove",isMove);


            // Nick name 바로해주기
            if(transform.localScale.x < 0)
            {
                nicknameTxt.transform.localScale = new Vector3(-1f,1f,1f);
            }
            else if(transform.localScale.x > 0)
            {
                nicknameTxt.transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }
    }


    public void SetPlayerColor_Hook(EPlayerColor oldColor, EPlayerColor newColor)
    {
        if(spriteRenderer == null)
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        spriteRenderer.material.SetColor("_PlayerColor",PlayerColor.GetColor(newColor));
    }
}
