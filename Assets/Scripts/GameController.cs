using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GameController : MonoBehaviour
{
    int dealersFirstCard = -1;
    int dealersFirstCardHandValue = 0;
    public int numberOfPlayers = 3;
    public int[] betStructure = new int[] { 100, 1000, 10000, 100000, 1000000 };

    public CardStack player0;
    public CardStack player1;
    public CardStack player2;
    public CardStack player3;
    public CardStack player4;
    public CardStack player5;
    public CardStack dealer;
    public CardStack deck;
    public CoinManger coinManager;

    public Button hitButton;
    public Button standButton;
    public Button betDownButton;
    public Button betUpButton;
    public Button dealButton;
    public Button maxBetButton;

    public GameObject messageLabelBg;

    public Text message;
    public Text betLabel;
    public Text winText;

    public int currentBet = 100;
    int totalWin = 0;
    bool isDealerBlackjack = false;
    bool[] isInsuranceSelected = new bool[3];

    CardStack[]  playerArray= new CardStack[6];
    CardStack[] activePlayerArray = new CardStack[3];

    int activePlayer = 0;
    float cardDrawSpeed = 1.6f;
    bool isDealerActive = false;

    private void Start()
    {
        SetPlayers();
        ResetGame();
    }

    void SetPlayers()
    {
        activePlayerArray[0] = player0;
        activePlayerArray[1] = player2;
        activePlayerArray[2] = player4;

        playerArray[0] = player0;
        playerArray[1] = player1;
        playerArray[2] = player2;
        playerArray[3] = player3;
        playerArray[4] = player4;
        playerArray[5] = player5;

        for (int i = 0; i < numberOfPlayers; i++)
        {
            isInsuranceSelected[i] = false;
        }
    }

    public void DealButtonClicked()
    {
        if (coinManager.GetPlayerBalance() >= currentBet)
        {
            ResetGame();
            SetButtonStatesOnGamePlay();
            SetBetButtonStates(false);
            StartGame();
        }
        else
        {
            Debug.Log("Out of coins!!");
        }
    }

    public void ResetGame()
    {
        DisableAllButtons();
        ClearCards();
        CheckForCardCount();

        totalWin = 0;
        message.text = " ";
        dealersFirstCard = -1;
        for (int i = 0; i < numberOfPlayers; i++)
        {
            activePlayerArray[i].GetComponent<CardStackView>().ClearWinMessages();
        }
        dealer.GetComponent<CardStackView>().ClearWinMessages();
        isDealerBlackjack = false;
        activePlayer = 0;

        SetButtonsOnGameOver();
        SetAndDisplayBet();
        SetAndDisplayWin();
    }

    private void ClearCards()
    {
        for(int i =0;i<playerArray.Length;i++)
        {
            playerArray[i].GetComponent<CardStackView>().Clear();
        }
        dealer.GetComponent<CardStackView>().Clear();
    }

    void SetDelayForDeal()
    {
        for(int j = 0; j <= numberOfPlayers; j++)
        {
            Invoke("SetDelayForHit", (cardDrawSpeed/4) * j);
        }
    }

    void SetDelayForHit()
    {
        if (isDealerActive)
        {
            HitDealer();
            activePlayer = 0;
            isDealerActive = false;
        }
        else
        {
            HitPlayer();
            activePlayer++;

            if(activePlayer >= activePlayerArray.Length)
            {
                activePlayer = 0;
                isDealerActive = true;
            }
        }
    }

    void StartGame()
    {
        coinManager.updateNetUserBalance(-coinManager.GetBetAmount());
        activePlayer = 0;
        for (int i = 0; i < 2; i++)
        {
            Invoke("SetDelayForDeal", cardDrawSpeed * i);
        }
        Invoke("checkForDeactivatedPlayer", cardDrawSpeed *2);
    }

    void checkForDeactivatedPlayer()
    {
        activePlayer = 0;
        if (activePlayer > activePlayerArray.Length - 1)
        {
            hitButton.interactable = false;
            standButton.interactable = false;
            StartCoroutine(DealersTurn());
        }
        else if (activePlayerArray[activePlayer].GetComponent<CardStack>().IsBlackjack())
        {
            ActivateNextPlayer();
        }
        else if(!isDealerBlackjack)
        {
            activePlayerArray[activePlayer].GetComponent<CardStackView>().ActivatePlayer();
        }
    }

    void CheckForCardCount()
    {
        if (deck.CardCount <= 1)
        {
            deck.CreateDeck();
        }
    }

    void ActivateNextPlayer()
    {
        activePlayerArray[activePlayer].GetComponent<CardStackView>().DeactivatePlayer();
        activePlayer++;
        Invoke("SetIntermediateDelay", 0.4f);
    }

    void SetIntermediateDelay()
    {
        if (activePlayer > activePlayerArray.Length - 1)
        {
            hitButton.interactable = false;
            standButton.interactable = false;
            StartCoroutine(DealersTurn());
        }
        else if (activePlayerArray[activePlayer].GetComponent<CardStack>().IsBlackjack())
        {
            ActivateNextPlayer();
        }
        else
        {
            activePlayerArray[activePlayer].GetComponent<CardStackView>().ActivatePlayer();
            EnableButtonsOnGamePlay();
        }
    }

    public void HitPlayer()
    {
        activePlayerArray[activePlayer].GetComponent<CardStackView>().HideSplitButton();
        CheckForCardCount();
        DisableButtonsOnGamePlay();
        activePlayerArray[activePlayer].Push(deck.Pop(), true);
        if (activePlayerArray[activePlayer].CardCount > 2)
        {
            activePlayerArray[activePlayer].GetComponent<CardStackView>().ActivatePlayer();
            if (activePlayerArray[activePlayer].HandValue() >= 21) //Player busted
            {
                if (activePlayerArray[activePlayer].HandValue() > 21)
                {
                    Invoke("PlayerBusted", cardDrawSpeed / 4);
                }
                Invoke("ActivateNextPlayer", 0.4f);
            }
            else
            {
                Invoke("EnableButtonsOnGamePlay", 0.4f);
            }
        }
        else
        {
            if (activePlayerArray[activePlayer].HandValue() == 21)
            {
                activePlayerArray[activePlayer].GetComponent<CardStackView>().DisplayWinMessages("BLACKJACK");
            }
        }
    }

    void PlayerBusted()
    {
        activePlayerArray[activePlayer].GetComponent<CardStackView>().DisplayWinMessages("BUST");
    }

    void PlayerBlackjack()
    {
        activePlayerArray[activePlayer].GetComponent<CardStackView>().DisplayWinMessages("BLACKJACK");
    }

    void HitDealer()
    {
        CheckForCardCount();
        int card = deck.Pop();
        if (dealersFirstCard < 0)
        {
            dealersFirstCard = card;
        }

        dealer.Push(card, true);
      
        if (dealer.CardCount == 2)
        {
            for(int j = 0; j < numberOfPlayers; j++)
            {
                if (!activePlayerArray[j].GetComponent<CardStack>().IsBlackjack())
                {
                    activePlayer = j;
                    break;
                }
            }
            int cardRankToBeDisplayed = dealer.HandValue() - dealersFirstCardHandValue;
            if((cardRankToBeDisplayed == 1) || (cardRankToBeDisplayed == 11))  //checkForInsurance
            {
                DisableAllButtons();
                for(int k = 0; k < activePlayerArray.Length; k++)
                {
                    activePlayerArray[k].GetComponent<CardStackView>().DisplayInsurancePanel();
                }
            }
            else if(dealer.HandValue() == 21)
            {
                isDealerBlackjack = true;
                dealer.GetComponent<CardStackView>().DisplayWinMessages("BLACKJACK");
                StartCoroutine(DealersTurn());
            }
            else
            {
                Invoke("SetDelayForHitDealer", cardDrawSpeed / 4);
            }
        }
        if (dealer.CardCount >= 2)
        {
            CardStackView view = dealer.GetComponent<CardStackView>();
            view.Toggle(card, true);
            if(dealer.CardCount > 2)
            {
                view.ActivatePlayer();
            }
        }
        if(dealer.CardCount == 1)
        {
            dealersFirstCardHandValue = dealer.HandValue();
        }
        if(dealer.HandValue() > 21)
        {
            Invoke("DealerBusted", cardDrawSpeed / 4);
        }
    }

    void DealerBusted()
    {
        dealer.GetComponent<CardStackView>().DisplayWinMessages("BUST");
    }

    void DealerBlackjack()
    {
        dealer.GetComponent<CardStackView>().DisplayWinMessages("BLACKJACK");
        StartCoroutine(DealersTurn());
    }

    void SetDelayForHitDealer()
    {
        EnableButtonsOnGamePlay();
        activePlayerArray[activePlayer].GetComponent<CardStackView>().ActivatePlayer();
    }

    public void onInsuranceSelected()
    {
        String name = EventSystem.current.currentSelectedGameObject.name;
        int playerId = int.Parse(name.Substring(name.Length - 1));
        activePlayerArray[playerId].GetComponent<CardStackView>().HideInsurancePanel();
        isInsuranceSelected[playerId] = true;
        DeductInsuranceCoins();

        for(int i = 0; i < activePlayerArray.Length; i++)
        {
            if(activePlayerArray[i].GetComponent<CardStackView>().insurancePanel.activeSelf)
            {
                break;
            }
            else if(i == activePlayerArray.Length - 1)
            {
                if(dealer.HandValue() == 21)
                {
                    isDealerBlackjack = true;
                    totalWin = 0;
                    for (int j = 0; j < activePlayerArray.Length; j++)
                    {
                        if(isInsuranceSelected[j])
                        {
                            totalWin += currentBet + currentBet / 2;
                        }
                    }
                    StartCoroutine(DealersTurn());
                    SetAndDisplayWin();
                }
                else
                {
                    checkForDeactivatedPlayer();
                    hitButton.interactable = true;
                    standButton.interactable = true;
                }
            }
        }
    }

    public void onInsuranceRejected()
    {
        String name = EventSystem.current.currentSelectedGameObject.name;
        int playerId = int.Parse(name.Substring(name.Length - 1));
        activePlayerArray[playerId].GetComponent<CardStackView>().HideInsurancePanel();
        isInsuranceSelected[playerId] = false;

        for (int i = 0; i < activePlayerArray.Length; i++)
        {
            if (activePlayerArray[i].GetComponent<CardStackView>().insurancePanel.activeSelf)
            {
                break;
            }
            else if (i == activePlayerArray.Length - 1)
            {
                if (dealer.HandValue() == 21)
                {
                    isDealerBlackjack = true;
                    totalWin = 0;

                    for (int j = 0; j < activePlayerArray.Length; j++)
                    {
                        if (isInsuranceSelected[j])
                        {
                            totalWin += currentBet + currentBet / 2;
                        }
                    }
                    StartCoroutine(DealersTurn());
                    SetAndDisplayWin();
                }
                else
                {
                    checkForDeactivatedPlayer();
                    hitButton.interactable = true;
                    standButton.interactable = true;
                }
            }
        }
    }

    void DeductInsuranceCoins()
    {
        int insuranceBet = currentBet / 2;
        coinManager.updateNetUserBalance(-insuranceBet);
    }

    void DisplayDealerHandValue()
    {
        dealer.GetComponent<CardStackView>().DisplayCardValue(dealer.HandValue());
    }

    public void Stand()
    {
        activePlayerArray[activePlayer].GetComponent<CardStackView>().HideSplitButton();
        DisableButtonsOnGamePlay();
        ActivateNextPlayer();
    }

    IEnumerator DealersTurn()
    {
        hitButton.interactable = false;
        standButton.interactable = false;
        CardStackView view = dealer.GetComponent<CardStackView>();
        view.Toggle(dealersFirstCard, true);

        if (dealer.CardCount > 2)
        {
            DisplayDealerHandValue();
        }
        dealer.GetComponent<CardStackView>().DisplayCardValue(dealer.HandValue());
        view.ShowCards();
        if (isDealerBlackjack)
        {
            yield return new WaitForSeconds(1f);
            SetButtonsOnGameOver();
        }
        else
        {
            view.ActivatePlayer();
            while (dealer.HandValue() < 17)
            {
                HitDealer();
                yield return new WaitForSeconds(1f);
            }
            yield return new WaitForSeconds(0.4f);
            int i = 0;
            view.DeactivatePlayer();
            while (i < activePlayerArray.Length)
            {
                if (activePlayerArray[i].HandValue() <= 21 && !activePlayerArray[i].GetComponent<CardStack>().IsBlackjack())
                {
                    DisplayWinMessages(i);
                    yield return new WaitForSeconds(0.5f);
                    i++;
                }
                else
                {
                    i++;
                }
            }
            yield return new WaitForSeconds(1f);
            SetButtonsOnGameOver();
        }
    }

    public void OnSplitButtonClicked()
    {
        String name = EventSystem.current.currentSelectedGameObject.name;
        int player = int.Parse(name.Substring(name.Length - 1));
        int playerId = GetCardIndex(player);
        int len = activePlayerArray.Length - 1;
        for (int i = len; i>=0; i--)
        {
            if (i == playerId)
            {
                Array.Resize<CardStack>(ref activePlayerArray, activePlayerArray.Length + 1);
                for (int j = len; j>=i; j--)
                {
                    activePlayerArray[j + 1] = activePlayerArray[j];
                }
                activePlayerArray[playerId+1] = playerArray[((2 * player) + 1)];
                for (int k = i-1 ; k >= 0; k--)
                {
                    activePlayerArray[k] = activePlayerArray[k];
                }
            }
        }
        activePlayerArray[playerId].GetComponent<CardStackView>().SplitCards();
        StartCoroutine(RemoveAndAddCard(playerId));
        activePlayerArray[playerId].GetComponent<CardStackView>().HideSplitButton();
    }

    IEnumerator RemoveAndAddCard(int playerId)
    {
        yield return new WaitForSeconds(0.4f);
        int cardIndex = activePlayerArray[playerId].GetComponent<CardStack>().onSplit();
        activePlayerArray[playerId + 1].Push(cardIndex, false);
        int cardIndex1 = activePlayerArray[playerId].GetComponent<CardStack>().onSplit();
        activePlayerArray[playerId].Push(cardIndex1, false);
        int temp = activePlayer;
        activePlayer = playerId;
        HitPlayer();
        activePlayer = playerId + 1;
        HitPlayer();
        activePlayer = temp;
        activePlayerArray[playerId].GetComponent<CardStackView>().DisplayCardValue(activePlayerArray[playerId].HandValue());
        activePlayerArray[playerId].GetComponent<CardStackView>().SetSplitState();
        activePlayerArray[playerId + 1].GetComponent<CardStackView>().SetSplitState();
    }

    int GetCardIndex(int playerId)
    {
        int index = 0;
        string name = "Player"+(playerId * 2);

        for(int i = activePlayerArray.Length-1; i >= 0; i--)
        {
            string objName = activePlayerArray[i].GetComponent<CardStackView>().GetName();
            if (objName == name)
            {
                index = i;
            }
        }
        return index;
    }

    private void DisplayWinMessages(int i)
    {
           if(activePlayerArray[i].GetComponent<CardStack>().IsBlackjack())
            {
                totalWin += (int)(currentBet * 2.5f);
            }
           else if((dealer.HandValue() > activePlayerArray[i].HandValue() && dealer.HandValue() <= 21))
            {
                activePlayerArray[i].GetComponent<CardStackView>().DisplayWinMessages("LOST");
                totalWin += 0;
            }
           else if((activePlayerArray[i].HandValue() == dealer.HandValue()) && (activePlayerArray[i].HandValue() <= 21))
            {
                activePlayerArray[i].GetComponent<CardStackView>().DisplayWinMessages("PUSH");
                totalWin += 0;
            }
            else if (dealer.HandValue() > 21 || (activePlayerArray[i].HandValue() <= 21 && (activePlayerArray[i].HandValue() > dealer.HandValue())))
            {
                activePlayerArray[i].GetComponent<CardStackView>().DisplayWinMessages("YOU WON");
                totalWin += currentBet * 2;
        }
        SetAndDisplayWin();
    }

    private void DisableAllButtons()
    {
        hitButton.interactable = false;
        standButton.interactable = false;
        dealButton.interactable = false;
        maxBetButton.interactable = false;
        SetBetButtonStates(false);
    }

    void SetAndDisplayBet()
    {
        int bet = 0;
        for(var i = 0; i < activePlayerArray.Length; i++)
        {
            bet += currentBet;
        }
        coinManager.SetBetAmount(bet);
        betLabel.text = currentBet.ToString();
        messageLabelBg.gameObject.SetActive(false);
    }

    void SetAndDisplayWin()
    {
        coinManager.updateNetUserBalance(totalWin);
        winText.text = totalWin.ToString();
    }

    public void OnMaxBetButtonClicked()
    {
        currentBet = betStructure[betStructure.Length - 1];
        SetAndDisplayBet();
        CheckAndSetBetButtonState();
    }

    public void OnBetUpButtonClicked()
    {
        int index = System.Array.IndexOf(betStructure, currentBet);
        index++;
        if (index < betStructure.Length)
        {
            currentBet = betStructure[index];
        }
        CheckAndSetBetButtonState();
        SetAndDisplayBet();
    }

    public void OnBetDownButtonClicked()
    {
        var index = System.Array.IndexOf(betStructure, currentBet);
        index--;
        if (index >= 0)
        {
            currentBet = betStructure[index];
        }
        CheckAndSetBetButtonState();
        SetAndDisplayBet();
    }

    void CheckAndSetBetButtonState()
    {
        var index = System.Array.IndexOf(betStructure, currentBet);
        if (index <= 0)
        {
            betDownButton.interactable = false;
            betUpButton.interactable = true;
        }
        else if(index >= (betStructure.Length - 1))
        {
            betDownButton.interactable = true;
            betUpButton.interactable = false;
        }
        else
        {
            SetBetButtonStates(true);
        }
    }

    void SetBetButtonStates(bool state)
    {
        betUpButton.interactable = state;
        betDownButton.interactable = state;
    }

    void SetButtonStatesOnGamePlay()
    {
        hitButton.gameObject.SetActive(true);
        standButton.gameObject.SetActive(true);
        dealButton.gameObject.SetActive(false);
        maxBetButton.gameObject.SetActive(false);
    }

    void EnableButtonsOnGamePlay()
    {
        hitButton.interactable = true;
        standButton.interactable = true;
        dealButton.interactable = false;
        maxBetButton.interactable = false;
    }

    void DisableButtonsOnGamePlay()
    {
        hitButton.interactable = false;
        standButton.interactable = false;
        dealButton.interactable = false;
        maxBetButton.interactable = false;
    }


    private void SetButtonsOnGameOver()
    {
        CheckAndSetBetButtonState();
        dealButton.interactable = true;
        maxBetButton.interactable = true;
        dealButton.gameObject.SetActive(true);
        maxBetButton.gameObject.SetActive(true);
        hitButton.gameObject.SetActive(false);
        standButton.gameObject.SetActive(false);
    }
}
