
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CardStack : MonoBehaviour
{
    List<int> cards;
    public bool isGameDeck;
    public bool HasCards
    {
        get { return cards != null && cards.Count > 0; }
    }

    public event CardEventHandler CardRemoved;
    public event CardEventHandler CardAdded;

    public int CardCount
    {
        get
        {
            if(cards == null)
            {
                return 0;
            }
            else
            {
                return cards.Count;
            }
        }
    }

    public IEnumerable<int> GetCards()
    {
        foreach (int i in cards)
        {
            yield return i;
        }
    }

    public int Pop()
    {
        int temp = cards[0];
        cards.RemoveAt(0);
        if(CardRemoved != null)
        {
            CardRemoved(this, new CardEventArgs(temp, false));
        }
        return temp;
    }

    public void Push(int card, bool shouldMove)
    {
        cards.Add(card);

        if(CardAdded != null)
        {
            CardAdded(this, new CardEventArgs(card, shouldMove));
        }
    }

    public int HandValue()
    {
        int total = 0; 
        int aces = 0;
        foreach(int card in GetCards())
        {
            int cardRank = card % 13;
            if(cardRank == 0)
            {
                aces++;
            }
            else if(cardRank > 0 && cardRank<9)
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

        for(int i = 0; i < aces; i++)
        {
            if(total + 11 <= 21)
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

    public bool IsBlackjack()
    {
        bool isBlackjack = false;
        if(CardCount == 2 && HandValue() == 21)
        {
            isBlackjack = true;
        }
        return isBlackjack;
    }

    public int onSplit()
    {
        int cardIndex = Pop();
        return cardIndex;
    }
    public void CreateDeck()
    {
        cards.Clear();

        for(int i = 0;i < 52; i++)
        {
            cards.Add(i);
        }

        int n = cards.Count;
        while (n > 1)
        {
            n--;
            int k = Random.Range(0, n + 1);
            int temp = cards[k];
            cards[k] = cards[n];
            cards[n] = temp;
        }
        /*For Debugging  
        cards[0] = 11;
       cards[1] = 12;
       cards[2] = 9;
       cards[3] = 9; 
       cards[4] = 0; 
       cards[5] = 11; 
       cards[6] = 11; 
       cards[7] = 10; */
    }

    public void Reset()
    {
        cards.Clear();
    }
    void Awake()
    {
        cards = new List<int>();
        if(isGameDeck)
        {
            CreateDeck();
        }
    }
}
  