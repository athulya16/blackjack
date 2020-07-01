using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManger : MonoBehaviour
{
    public int netUserBalance = 100;
    private int betAmount = 0;

    public int GetPlayerBalance()
    {
        return netUserBalance;
    }

    public void updateNetUserBalance(int bet)
    {
        netUserBalance += bet;
    }

    public void SetBetAmount(int bet)
    {
        betAmount = bet;
    }

    public int GetBetAmount()
    {
        return betAmount;
    }

    public void ClearBet()
    {
        betAmount = 0;
    }

     
}
