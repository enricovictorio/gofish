using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AICardHolder : CardHolder
{
    private int mAIState = -1;
    private bool mPlayerHasInitialized = false;

    private float mSimulateThinkingDelay = 0;

    public async override void initializeCards()
    {

        int numCardsInHand = GetComponentsInChildren<Card>().Length;

        int i = 0;
        while (i < numCardsInHand)
        {
            string thisCardNum = Card.CARDNUM[UnityEngine.Random.Range(0, Card.CARDNUM.Length)];
            string thisCardSuit = Card.CARDSUITS[UnityEngine.Random.Range(0, Card.CARDSUITS.Length)];
            string cardName = thisCardNum + "_of_" + thisCardSuit;

            if (!GameBase.Instance.cardsInPlay.Contains(cardName))
            {
                cardsOnHand[i].setCardFace(cardName, true);
                GameBase.Instance.cardsInPlay.Add(cardName);

                i++;
            }
            else
            {
                await Task.Delay(1);
            }
        }

        mPlayerHasInitialized = true;
    }

    public override bool hasInitialized() { return mPlayerHasInitialized; }

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
                if (mSimulateThinkingDelay < 0)
                {
                    do
                    {
                        mSelectedCards.Clear();
                        selectCards();
                    }
                    while (!playSelectedCards());

                    mAIState = -1;
                    this.enabled = false;
                    Game.Instance.PlayNextTurn();
                    break;
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
        if (Game.Instance.Table.cardsInTable.Count > 0)
        {
            if (UnityEngine.Random.value < 0.1f)
            {
                return;
            }
        }

        int numCardsToSelect = Math.Min(cardsOnHand.Count, UnityEngine.Random.Range(1, 6));
        for (int i = 0; i < numCardsToSelect; i++)
        {
            Card selectedCard = cardsOnHand[UnityEngine.Random.Range(0, cardsOnHand.Count)];
            mSelectedCards.Add(selectedCard);
        }
    }
}
