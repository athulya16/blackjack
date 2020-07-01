using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DisplayCardValue : MonoBehaviour
{
    public GameObject valueBg;
    public Vector3 PlayerValuePosition;
    public Vector3 dealerValuePosition;
    public Text handValueText;

    Vector3 minScale = new Vector3(1f, 1f, 0);
    Vector3 maxScale = new Vector3(1.5f,1.5f, 0);

    public void DisplayPlayerHandValue(int handValue)
    {
        valueBg.SetActive(true);
        handValueText.text = handValue.ToString();
        valueBg.transform.position = PlayerValuePosition;
    }

    public void DisplayDealerHandValue(int handValue)
    {
        valueBg.SetActive(true);
        handValueText.text = handValue.ToString();
        valueBg.transform.position = dealerValuePosition;
    }
}

