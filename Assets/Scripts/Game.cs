using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : GameBase
{
    public override void OnGameInit()
    {
        CardHolder firstPlayer = Game.Instance.ThisPlayer;
        foreach (CardHolder player in Game.Instance.OtherPlayers)
        {
            Debug.Log("Other player lowest card is " + player.getLowestCard().cardName);

            if (player.getLowestCard().getCardValue() < firstPlayer.getLowestCard().getCardValue())
            {
                firstPlayer = player;
            }
        }

        Game.Instance.ActivePlayer = firstPlayer;
        Game.Instance.ActivePlayer.enabled = true;
    }

    public override void OnGameLoop()
    {
    }

    public override void OnGameEnd()
    {
    }

    public override bool OnConfirmMove(List<Card> pCardsToPlay)
    {
        if (pCardsToPlay.Count > 1)
        {
            return false;
        }

        if (Game.Instance.Table.cardsInTable.Count != 0 && pCardsToPlay.Count != Game.Instance.Table.cardsInTable.Count)
        {
            return false;
        }

        if ((pCardsToPlay.Count == 1 && Validate1CardPlay(pCardsToPlay))
            || (pCardsToPlay.Count == 2 && Validate2CardPlay(pCardsToPlay))
            || (pCardsToPlay.Count == 3 && Validate3CardPlay(pCardsToPlay))
            || (pCardsToPlay.Count == 5 && Validate5CardPlay(pCardsToPlay))
            )
        {
            return true;
        }

        return false;
    }

    public override void OnPlayerWin(CardHolder pPlayer)
    {
        if (pPlayer == Game.Instance.ThisPlayer)
        {
            Debug.Log("You Win!");
        }
        else
        {
            Debug.Log("You Lose!");
        }

        Debug.Assert(false);
    }

    private bool Validate1CardPlay(List<Card> pCardsToPlay)
    {
        int cardOnTableValue = Game.Instance.Table.cardsInTable.Count < 1 ? 0 : Game.Instance.Table.cardsInTable[0].getCardValue();
        int cardOnHandValue = pCardsToPlay[0].getCardValue();

        return cardOnHandValue > cardOnTableValue;
    }

    private bool Validate2CardPlay(List<Card> pCardsToPlay)
    {
        return true;
    }

    private bool Validate3CardPlay(List<Card> pCardsToPlay)
    {
        return true;
    }

    private bool Validate5CardPlay(List<Card> pCardsToPlay)
    {
        return true;
    }
}
