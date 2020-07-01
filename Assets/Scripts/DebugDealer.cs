using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugDealer : MonoBehaviour
{
    public CardStack dealer;
    public CardStack player;

    //Debug code
    int count = 0;
    int[] cards = new int[] {0, 1, 2 };

    void OnGUI()
    {
        /* if (GUI.Button(new Rect(10, 10, 256, 28), "Hit Me!"))
         {
             player.Push(dealer.Pop());
         }*/

        if (GUI.Button(new Rect(10, 10, 256, 28), "Hit Me!"))
        {
            player.Push(cards[count++], true);
        }
    }
}
