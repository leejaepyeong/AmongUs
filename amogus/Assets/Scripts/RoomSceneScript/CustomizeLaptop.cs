using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomizeLaptop : MonoBehaviour
{
    [SerializeField]
    private Sprite useButtonSprite;
    private SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();

        var init = Instantiate(spriteRenderer.material);
        spriteRenderer.material = init;
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        var character = collision.GetComponent<CharacterMover>();

        if (character != null && character.hasAuthority)
        {

            spriteRenderer.material.SetFloat("_Highlighted",1f);
            LobbyUIManager.Instance.SetUseBtn(useButtonSprite,OnClickUse);

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        var character = collision.GetComponent<CharacterMover>();

        if (character != null && character.hasAuthority)
        {

            spriteRenderer.material.SetFloat("_Highlighted", 0f);
            LobbyUIManager.Instance.UnSetUseBtn();

        }
    }

    public void OnClickUse()
    {
        LobbyUIManager.Instance.CustomizeUI.Open();
    }
}
