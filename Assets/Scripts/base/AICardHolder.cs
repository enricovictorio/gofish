using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AICardHolder : Player
{
    private int mAIState = -1;
    private bool mPlayerHasInitialized = false;

    private float mSimulateThinkingDelay = 0;

    public override IEnumerator initializeCards()
    {
        int numCardsInHand = GetComponentsInChildren<Card>().Length;

        int i = 0;
        while (i < numCardsInHand)
        {
            string thisCardRank = Card.CARDRANK[UnityEngine.Random.Range(0, Card.CARDRANK.Length)];
            string thisCardSuit = Card.CARDSUITS[UnityEngine.Random.Range(0, Card.CARDSUITS.Length)];
            string cardName = thisCardRank + "_of_" + thisCardSuit;

            if (!GameBase.Instance.cardsInPlay.Contains(cardName) && cardsOnHand[i].setCardFace(cardName, true))
            {
                GameBase.Instance.cardsInPlay.Add(cardName);

                i++;
            }
            else
            {
                yield return null;
            }
        }

        updateHandLayout();

        mPlayerHasInitialized = true;
    }

    public override bool hasInitialized() { return mPlayerHasInitialized; }

    public override void receiveCard(Card pCard)
    {
        pCard.clearCardFace();
        base.receiveCard(pCard);
    }

    public override void onEndTurn()
    {
        mAIState = -1;
        this.enabled = false;
    }

    void Update()
    {
        switch (mAIState)
        {
            case 0:
            {
                mSimulateThinkingDelay = UnityEngine.Random.Range(1, 5);
                mAIState = 1;
                break;
            }
            case 1:
            {
                mSimulateThinkingDelay -= Time.deltaTime;
                if (mSimulateThinkingDelay > 0)
                {
                    break;
                }

                mSelectedCards.Clear();
                selectCards();

                if (mSelectedCards.Count == 1)
                {
                    Game.Instance.OnConfirmMove(mSelectedCards);
                }

                if (mSelectedCards.Count == 1)
                {
                    Game.Instance.PlayNextTurn();
                }
                else if (mSelectedCards[0].cardRankIndex == mSelectedCards[1].cardRankIndex)
                {
                    playSelectedCards();
                    mAIState = 0;
                }
                break;
            }
        }
    }

    void OnEnable()
    {
        mAIState = 0;
    }

    void selectCards()
    {
        for (int i = 0; i < cardsOnHand.Count; i++)
        {
            Card selectedCard1 = cardsOnHand[UnityEngine.Random.Range(0, cardsOnHand.Count)];

            mSelectedCards.Clear();
            mSelectedCards.Add(selectedCard1);

            for (int j = 0 + 1; j < cardsOnHand.Count; j++)
            {
                Card selectedCard2 = cardsOnHand[j];
                if (selectedCard2 != selectedCard1 && selectedCard1.cardRankIndex == selectedCard2.cardRankIndex)
                {
                    mSelectedCards.Add(selectedCard2);
                    break;
                }
            }

            if (mSelectedCards.Count == 2)
            {
                break;
            }
        }
    }
}
