using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CardStack))]
public class CardStackView : MonoBehaviour
{
    CardStack deck;
    Dictionary<int, CardView> fetchedCards;

    public Vector3 start;
    public float cardOffset;
    public bool faceUp = false;
    public bool reverseLayerOrder = false;
    public GameObject cardPrefab;

    public Button splitButton;

    public GameObject valueBg;
    public Vector3 ValuePos;
    public Text handValueText;

    public GameObject messageLabel;
    public Text messageText;

    public GameObject insurancePanel;
    Vector3 cardMinScale = new Vector3(1.5f, 1.5f, 0);
    Vector3 cardMaxScale = new Vector3(2, 2, 0);
    Vector3 valueMinScale = new Vector3(1f, 1f, 0);
    Vector3 valueMaxScale = new Vector3(1.3f, 1.3f, 0);
    float scalingTime = 1f;
    float scalingSpeed = 15f;
    int dealerFirstCardValue = 0;
    int playersFirstCardValue = 0;

    bool isSplited = false;

    public void Toggle(int card, bool isFaceUp)
    {
        fetchedCards[card].isFaceUp = isFaceUp;
    }

    public void Clear()
    {
        isSplited = false;
        deck.Reset();
        foreach (CardView view in fetchedCards.Values)
        {
            Destroy(view.Card);
        }
        fetchedCards.Clear();
        if(valueBg)
        {
            valueBg.SetActive(false);
            handValueText.text = " ";
        }
    }
    void Awake()
    {
        fetchedCards = new Dictionary<int, CardView>();
        deck = GetComponent<CardStack>();
        ShowCards();

        deck.CardRemoved += deck_CardRemoved;
        deck.CardAdded += deck_CardAdded;
    }

    private void deck_CardAdded(object sender, CardEventArgs e)
    {
        float co = cardOffset * deck.CardCount;
        Vector3 temp = start + new Vector3(co, 0f);
        AddCard(temp, e.CardIndex, deck.CardCount, e.ShouldMove);
    }

    void deck_CardRemoved(object sender, CardEventArgs e)
    {
        if (fetchedCards.ContainsKey(e.CardIndex))
        {
            Destroy(fetchedCards[e.CardIndex].Card);
            fetchedCards.Remove(e.CardIndex);
        }
    }

    void Update()
    {
        ShowCards();
    }

    public void ShowCards()
    {
        int cardCount = 0;
        if (deck.HasCards)
        {
            foreach (int i in deck.GetCards())
            {
                float co = cardOffset * cardCount;
                Vector3 temp = start + new Vector3(co, 0f);
                AddCard(temp, i, cardCount, true);
                cardCount++;
            }
        }
    }

    void AddCard(Vector3 position, int cardIndex, int positionalIndex, bool shouldMove)
    {
        if (fetchedCards.ContainsKey(cardIndex))
        {
            if (!faceUp)
            {
                CardModel model = fetchedCards[cardIndex].Card.GetComponent<CardModel>();
                model.ToggleFace(fetchedCards[cardIndex].isFaceUp);
            }
            return;
        }
        GameObject cardCopy = (GameObject)Instantiate(cardPrefab);
        if(shouldMove)
        {
            Vector3 cardStackPos = new Vector3(6, 3, 0);
            cardCopy.transform.position = cardStackPos;
        }
        else
        {
            cardCopy.transform.position = position;
        }
        
        CardModel cardModel = cardCopy.GetComponent<CardModel>();
        cardModel.cardIndex = cardIndex;
        cardModel.ToggleFace(faceUp);

        SpriteRenderer spriteRenderer = cardCopy.GetComponent<SpriteRenderer>();
        if (reverseLayerOrder)
        {
            spriteRenderer.sortingOrder = 51 - positionalIndex;
        }
        else
        {
            spriteRenderer.sortingOrder = positionalIndex;
        }
        fetchedCards.Add(cardIndex, new CardView(cardCopy));
        float time = 0.4f;
        if(gameObject.name == "TheDeck")
        {
            time = 0f;
        }
        iTween.MoveTo(cardCopy, iTween.Hash("position", position, "islocal", true, "time", time, "oncomplete", "ShowHandValue", "oncompletetarget", gameObject));
    }

    public String GetName()
    {
        return gameObject.name;
    }

    void ShowHandValue()
    {
        int handValue = 0;
        int firstCardValue = 0;
        if (gameObject.name == "Dealer")
        {
            int itemCount = 0;
            foreach (KeyValuePair<int, CardView> item in fetchedCards)
            {
                itemCount++;
                if (fetchedCards.Count == 1)
                {
                    dealerFirstCardValue = CalculateHandValue();
                    handValue = 0;
                }
                else if (fetchedCards.Count == 2)
                {
                    if (itemCount == 2 && CalculateHandValue() == 21)
                    {
                        if (CalculateHandValue() - dealerFirstCardValue == 11)
                        {
                            handValue = CalculateHandValue() - dealerFirstCardValue;
                        }
                        else
                        {
                            handValue = CalculateHandValue();
                        }
                    }
                    else
                    {
                        handValue = CalculateHandValue() - dealerFirstCardValue;
                    }
                }
                else
                {
                    handValue = CalculateHandValue();
                }
            }
        }
        else
        {
            handValue = CalculateHandValue();
            int index = 0;
            foreach (KeyValuePair<int, CardView> item in fetchedCards)
            {
                index++;
                if (fetchedCards.Count == 1)
                {
                    playersFirstCardValue = CalculateHandValue();
                }
                else if ((fetchedCards.Count == 2) && (index == 2) && !isSplited)
                {
                    if (playersFirstCardValue == (CalculateHandValue() - playersFirstCardValue))
                    {
                        ShowSplitButton();
                    }
                }
            }
        }
        DisplayCardValue(handValue);
    }

    void ShowSplitButton()
    {
        splitButton.gameObject.SetActive(true);
    }

    public void SplitCards()
    {
        splitButton.gameObject.SetActive(false);
        int index = 0;
        foreach (KeyValuePair<int, CardView> item in fetchedCards)
        {
            index++;
            if (index == 1)
            {
                CardView cardObj = item.Value;
                ShowHandValue();
            }
            else if (index == 2)
            {
                CardView cardObj = item.Value;
            }
        }
    }

    public void SetSplitState()
    {
        isSplited = true;
    }

   void SetSplitCards()
    {
        int index = 0;
        int id = 0;
        foreach (KeyValuePair<int, CardView> item in fetchedCards)
        {
            index++;
            if(index == 1)
            {
                id = item.Key;
            }
        }

        if (fetchedCards.ContainsKey(id))
        {
            Destroy(fetchedCards[id].Card);
            fetchedCards.Remove(id);
        }
    }

    int CalculateHandValue()
    {
        int total = 0;
        int aces = 0;
        foreach (KeyValuePair<int, CardView> item in fetchedCards)
        {
            int cardRank = item.Key % 13;
            if (cardRank == 0)
            {
                aces++;
            }
            else if (cardRank > 0 && cardRank < 9)
            {
                cardRank += 1;
                total = total + cardRank;
            }
            else
            {
                cardRank = 10;
                total = total + cardRank;
            }
        }

        for (int i = 0; i < aces; i++)
        {
            if (total + 11 <= 21)
            {
                total = total + 11;
            }
            else
            {
                total = total + 1;
            }
        }
        return total;
    }

    public void ActivatePlayer()
    {
        foreach (KeyValuePair<int, CardView> item in fetchedCards)
        {
            iTween.ScaleTo(item.Value.Card, iTween.Hash("scale", cardMaxScale, "speed", scalingSpeed, "easetype", "linear"));
        }
        iTween.ScaleTo(valueBg, iTween.Hash("scale", valueMaxScale, "speed", scalingSpeed, "easetype", "linear"));
    }

    public void DeactivatePlayer()
    {

        foreach (KeyValuePair<int, CardView> item in fetchedCards)
        {
            iTween.ScaleTo(item.Value.Card, iTween.Hash("scale", cardMinScale, "speed", scalingSpeed, "easetype", "linear"));
        }
        iTween.ScaleTo(valueBg, iTween.Hash("scale", valueMinScale, "speed", scalingSpeed, "easetype", "linear"));
    }

    void ScaleCard(GameObject obj, float time, float speed, Vector3 minScale, Vector3 maxScale)
    {
        float i = 0.0f;
        float rate = (1.0f / time) * speed;
        while (i < 1.0f)
        {
            i += Time.deltaTime * rate;

            obj.transform.localScale = Vector3.Lerp(minScale, maxScale, i);
        }
    }

    public void DisplayCardValue(int handValue)
    {
        if(valueBg && handValue > 0)
        {
            valueBg.SetActive(true);
            handValueText.text = handValue.ToString();
            valueBg.transform.position = ValuePos;
        }
    }

    public void DisplayWinMessages(String msgTxt)
    {
        messageLabel.SetActive(true);
        messageText.text = msgTxt;
    }

    public void ClearWinMessages()
    {
        messageLabel.SetActive(false);
        messageText.text = "";
    }

    public void DisplayInsurancePanel()
    {
        ActivatePlayer();
        insurancePanel.SetActive(true);
    }

    public void HideInsurancePanel()
    {
        DeactivatePlayer();
        insurancePanel.SetActive(false);
    }

    public void HideSplitButton()
    {
        splitButton.gameObject.SetActive(false);
    }

}