using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardModel : MonoBehaviour
{
    SpriteRenderer spriteRender;

    public Sprite[] faces;
    public Sprite cardBack;

    public int cardIndex;

    public void ToggleFace(bool showFace)
    {
        if(showFace)
        {
            spriteRender.sprite = faces[cardIndex];
        }
        else
        {
            spriteRender.sprite = cardBack;
        }
    }

    void Awake()
    {
        spriteRender = GetComponent<SpriteRenderer>();
    }
}
