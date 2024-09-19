using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CardHolder : MonoBehaviour
{
    protected List<Card> cardsOnHand = new List<Card>();
    protected List<Card> mSelectedCards = new List<Card>();

    public virtual IEnumerator initializeCards() { return null; }
    public virtual bool hasInitialized() { return false; }
    public virtual void onEndTurn() { }

    public int remainingCardsOnHand() { return cardsOnHand.Count; }

    public bool hasCard(string pCardName)
    {
        foreach (Card card in cardsOnHand)
        {
            if (card.cardName == pCardName)
            {
                return true;
            }
        }

        return false;
    }

    public string hasCardRank(int pRankIdx)
    {
        foreach (Card card in cardsOnHand)
        {
            if (card.cardRankIndex == pRankIdx)
            {
                return card.cardName;
            }
        }

        return "";
    }

    public Card getCard(string pCardName)
    {
        int cardIndexToGive = -1;
        for(int i = 0; i < cardsOnHand.Count; i++)
        {
            if (cardsOnHand[i].cardName == pCardName)
            {
                cardIndexToGive = i;
                break;
            }
        }

        if (cardIndexToGive == -1)
        {
            return null;
        }

        Card retVal = cardsOnHand[cardIndexToGive];
        cardsOnHand.Remove(retVal);

        updateHandLayout();

        return retVal;
    }

    public virtual void receiveCard(Card pCard)
    {
        pCard.transform.parent.SetParent(transform, false);
        cardsOnHand.Add(pCard);

        updateHandLayout();
    }

    public Card getLowestCard()
    {
        Card lowestCard = cardsOnHand[0];
        foreach (Card card in cardsOnHand)
        {
            if (card != lowestCard && card.cardValue < lowestCard.cardValue)
            {
                lowestCard = card;
            }
        }
        
        return lowestCard;
    }

    public Card geHighestCard()
    {
        Card highestCard = cardsOnHand[0];
        foreach (Card card in cardsOnHand)
        {
            if (card != highestCard && card.cardValue > highestCard.cardValue)
            {
                highestCard = card;
            }
        }

        return highestCard;
    }

    public void updateHandLayout()
    {
        int CARDSPREADCOUNT = cardsOnHand.Count;
        float ANGLEPERCARD = 140.0f / (float)cardsOnHand.Count;
        float FIRSTCARDANGLE = 0 - (ANGLEPERCARD * (CARDSPREADCOUNT / 2.0f)) + (ANGLEPERCARD / 2.0f);

        for (int i = 0; i < cardsOnHand.Count; i++)
        {
            Transform transform = cardsOnHand[i].transform.parent;

            transform.localPosition = new Vector3(0, 0, i * 0.1f);
            transform.localEulerAngles = new Vector3(0, 0, FIRSTCARDANGLE + (ANGLEPERCARD * i));
            transform.localScale = Vector3.one;
        }
    }

    [ContextMenu("Test")]
    public void Test()
    {
        int CARDSPREADCOUNT = cardsOnHand.Count;
        float ANGLEPERCARD = 140.0f / (float)cardsOnHand.Count;
        float FIRSTCARDANGLE = 0 - (ANGLEPERCARD * (CARDSPREADCOUNT / 2.0f)) + (ANGLEPERCARD / 2.0f);

        for (int i = 0; i < cardsOnHand.Count; i++)
        {
            Transform transform = cardsOnHand[i].transform.parent;

            transform.localPosition = new Vector3(0, 0, i * 0.1f);
            transform.localEulerAngles = new Vector3(0, 0, FIRSTCARDANGLE + (ANGLEPERCARD * i));
            transform.localScale = Vector3.one;
        }
    }

    void Start()
    {
        cardsOnHand.AddRange(this.transform.GetComponentsInChildren<Card>());
        this.enabled = false;
    }

}
