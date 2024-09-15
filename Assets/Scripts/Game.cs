using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Game : GameBase
{
    [SerializeField]
    GameObject mShowDialog;

    [SerializeField]
    TextMeshProUGUI mResultText;

    bool mIsFirstTurn = true;
    Card mLowestCardInPlay = null;

    public void ReloadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public override void OnGameInit()
    {
        CardHolder firstPlayer = Game.Instance.ThisPlayer;
        foreach (CardHolder player in Game.Instance.OtherPlayers)
        {
            Debug.Log("Other player lowest card is " + player.getLowestCard().cardName);

            if (player.getLowestCard().cardValue < firstPlayer.getLowestCard().cardValue)
            {
                firstPlayer = player;
            }
        }

        mLowestCardInPlay = firstPlayer.getLowestCard();
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
        if (pCardsToPlay.Count > 5)
        {
            return false;
        }

        if (mIsFirstTurn && !pCardsToPlay.Contains(mLowestCardInPlay))
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
            mIsFirstTurn = false;
            return true;
        }

        return false;
    }

    public override void OnPlayerWin(CardHolder pPlayer)
    {
        mShowDialog.SetActive(true);
        mResultText.gameObject.SetActive(true);

        if (pPlayer == Game.Instance.ThisPlayer)
        {
            mResultText.text = "You Win!";
        }
        else
        {
            mResultText.text = "You Lose!";
        }

        this.gameObject.SetActive(false);
    }

    private bool Validate1CardPlay(List<Card> pCardsToPlay)
    {
        int cardOnTableValue = Game.Instance.Table.cardsInTable.Count < 1 ? -1 : Game.Instance.Table.cardsInTable[0].cardValue;
        int cardOnHandValue = pCardsToPlay[0].cardValue;

        return cardOnHandValue > cardOnTableValue;
    }

    private bool Validate2CardPlay(List<Card> pCardsToPlay)
    {
        int cardOnHandRank = pCardsToPlay[0].cardRankIndex;
        if (pCardsToPlay[1].cardRankIndex != cardOnHandRank)
        {
            return false;
        }

        int cardOnTableRank = Game.Instance.Table.cardsInTable.Count < 1 ? -1 : Game.Instance.Table.cardsInTable[0].cardRankIndex;
        if (cardOnHandRank > cardOnTableRank)
        {
            return true;
        }
        else if (cardOnHandRank == cardOnTableRank)
        {
            int cardOnTableHighestSuit = 0;
            foreach (Card card in Game.Instance.Table.cardsInTable)
            {
                if (card.cardSuitIndex > cardOnTableHighestSuit)
                {
                    cardOnTableHighestSuit = card.cardSuitIndex;
                }
            }

            int cardOnHandHighestSuit = 0;
            foreach (Card card in pCardsToPlay)
            {
                if (card.cardSuitIndex > cardOnHandHighestSuit)
                {
                    cardOnHandHighestSuit = card.cardSuitIndex;
                }
            }

            if (cardOnHandHighestSuit > cardOnTableHighestSuit)
            {
                return true;
            }
        }

        return false;
    }

    private bool Validate3CardPlay(List<Card> pCardsToPlay)
    {
        int cardOnHandRank = pCardsToPlay[0].cardRankIndex;
        if (pCardsToPlay[1].cardRankIndex != cardOnHandRank || pCardsToPlay[2].cardRankIndex != cardOnHandRank)
        {
            return false;
        }

        int cardOnTableRank = Game.Instance.Table.cardsInTable.Count < 1 ? -1 : Game.Instance.Table.cardsInTable[0].cardRankIndex;
        if (cardOnHandRank > cardOnTableRank)
        {
            return true;
        }
        else if (cardOnHandRank == cardOnTableRank)
        {
            int cardOnTableHighestSuit = 0;
            foreach (Card card in Game.Instance.Table.cardsInTable)
            {
                if (card.cardSuitIndex > cardOnTableHighestSuit)
                {
                    cardOnTableHighestSuit = card.cardSuitIndex;
                }
            }

            int cardOnHandHighestSuit = 0;
            foreach (Card card in pCardsToPlay)
            {
                if (card.cardSuitIndex > cardOnHandHighestSuit)
                {
                    cardOnHandHighestSuit = card.cardSuitIndex;
                }
            }

            if (cardOnHandHighestSuit > cardOnTableHighestSuit)
            {
                return true;
            }
        }

        return false;
    }

    private bool Validate5CardPlay(List<Card> pCardsToPlay)
    {
        pCardsToPlay.Sort((a, b) => a.cardRankIndex - b.cardRankIndex);

        int cardsToPlayRank = getCardSetRank(pCardsToPlay);
        int cardsOnTableRank = getCardSetRank(Game.Instance.Table.cardsInTable);

        return cardsToPlayRank > cardsOnTableRank;
    }

    private int getCardSetRank(List<Card> pCardsToPlay)
    {
        if (pCardsToPlay.Count == 0)
        {
            return 0;
        }
        else if (Validate5CardStraight(pCardsToPlay))
        {
            return (1000 + HighestCardInSet(pCardsToPlay).cardValue);
        }
        else if (Validate5CardFlush(pCardsToPlay))
        {
            return (2000 + HighestCardInSet(pCardsToPlay).cardValue);
        }
        else if (Validate5CardFullHouse(pCardsToPlay))
        {
            return (3000 + HighestCardInSet(pCardsToPlay).cardValue);
        }
        else if (Validate5CardQuads(pCardsToPlay))
        {
            return (4000 + HighestCardInSet(pCardsToPlay).cardValue);
        }
        else if (Validate5CardStraightFlush(pCardsToPlay))
        {
            return (5000 + HighestCardInSet(pCardsToPlay).cardValue);
        }
        else
        {
            return 0;
        }
    }

    private bool Validate5CardStraight(List<Card> pCardsToPlay)
    {
        int lastRank = pCardsToPlay[0].cardRankIndex;
        for (int i = 1; i < pCardsToPlay.Count; i++)
        {
            if (pCardsToPlay[i].cardRankIndex != (lastRank + 1))
            {
                return false;
            }

            lastRank = pCardsToPlay[i].cardRankIndex;
        }

        int cardSuit = pCardsToPlay[0].cardSuitIndex;
        bool allCardsMatchSuits = true;
        for(int i = 1; i < pCardsToPlay.Count; i++)
        {
            if (pCardsToPlay[i].cardSuitIndex != cardSuit)
            {
                allCardsMatchSuits = false;
                break;
            }
        }

        return !allCardsMatchSuits;
    }

    private bool Validate5CardFlush(List<Card> pCardsToPlay)
    {
        int cardSuit = pCardsToPlay[0].cardSuitIndex;
        for (int i = 1; i < pCardsToPlay.Count; i++)
        {
            if (pCardsToPlay[i].cardSuitIndex != cardSuit)
            {
                return false;
            }
        }

        return true;
    }

    private bool Validate5CardFullHouse(List<Card> pCardsToPlay)
    {
        Dictionary<int, List<Card>> cardSets = new Dictionary<int, List<Card>>();

        for (int i = 0; i < pCardsToPlay.Count; i++)
        {
            if (!cardSets.ContainsKey(pCardsToPlay[i].cardRankIndex))
            {
                cardSets.Add(pCardsToPlay[i].cardRankIndex, new List<Card>());
            }
            cardSets[pCardsToPlay[i].cardRankIndex].Add(pCardsToPlay[i]);
        }

        bool has2Pair = false;
        bool has3Pair = false;
        foreach(List<Card> cardset in cardSets.Values)
        {
            if (cardset.Count == 2)
            {
                has2Pair = true;
            }
            else if (cardset.Count == 3)
            {
                has3Pair = true;
            }
        }

        return has2Pair && has3Pair;
    }

    private bool Validate5CardQuads(List<Card> pCardsToPlay)
    {
        Dictionary<int, List<Card>> cardSets = new Dictionary<int, List<Card>>();

        for (int i = 0; i < pCardsToPlay.Count; i++)
        {
            if (!cardSets.ContainsKey(pCardsToPlay[i].cardRankIndex))
            {
                cardSets.Add(pCardsToPlay[i].cardRankIndex, new List<Card>());
            }
            cardSets[pCardsToPlay[i].cardRankIndex].Add(pCardsToPlay[i]);
        }

        bool hasJokerCard = false;
        bool has4Pair = false;

        foreach (List<Card> cardset in cardSets.Values)
        {
            if (cardset.Count == 1)
            {
                hasJokerCard = true;
            }
            else if (cardset.Count == 4)
            {
                has4Pair = true;
            }
        }

        return hasJokerCard && has4Pair;
    }

    private bool Validate5CardStraightFlush(List<Card> pCardsToPlay)
    {
        int lastRank = pCardsToPlay[0].cardRankIndex;
        for (int i = 1; i < pCardsToPlay.Count; i++)
        {
            if (pCardsToPlay[i].cardRankIndex != (lastRank - 1))
            {
                return false;
            }
        }

        int cardSuit = pCardsToPlay[0].cardSuitIndex;
        bool allCardsMatchSuits = true;
        for (int i = 1; i < pCardsToPlay.Count; i++)
        {
            if (pCardsToPlay[i].cardSuitIndex != cardSuit)
            {
                allCardsMatchSuits = false;
                break;
            }
        }

        return allCardsMatchSuits;
    }

    private Card HighestCardInSet(List<Card> pSet)
    {
        Card highestCard = pSet[0];
        foreach(Card card in pSet)
        {
            if (card.cardValue > highestCard.cardValue)
            {
                highestCard = card;
            }
        }

        return highestCard;
    }
}
