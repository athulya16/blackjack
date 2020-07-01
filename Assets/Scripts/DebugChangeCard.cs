using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugChangeCard : MonoBehaviour
{
    CardModel cardModel;
    CardFlipper cardFlipper;
    int cardIndex = 0;
    public GameObject card;

    void Awake()
    {
        cardModel = card.GetComponent<CardModel>();
        cardFlipper = card.GetComponent<CardFlipper>();
    }

    void OnGUI()
    {
        if(GUI.Button(new Rect(10,10,100,28), "Hit Now!"))
        {
           

            if(cardIndex >= cardModel.faces.Length)
            {
                cardIndex = 0;
                cardFlipper.FlipCard(cardModel.faces[cardModel.faces.Length - 1], cardModel.cardBack, -1);
            }
            else
            {
                if(cardIndex > 0)
                {
                    cardFlipper.FlipCard(cardModel.faces[cardIndex - 1], cardModel.faces[cardIndex], cardIndex);
                }
                else
                {
                    cardFlipper.FlipCard(cardModel.cardBack, cardModel.faces[cardIndex], cardIndex);
                }
                cardIndex++;
            }
          
        }
    }

}
