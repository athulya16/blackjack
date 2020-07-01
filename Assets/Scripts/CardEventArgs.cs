using System;

public class CardEventArgs : EventArgs
{
    public int CardIndex { get; private set; }
    public bool ShouldMove { get; private set; }

    public CardEventArgs(int cardIndex, bool shouldMove)
    {
        CardIndex = cardIndex;
        ShouldMove = shouldMove;
    }
}   


