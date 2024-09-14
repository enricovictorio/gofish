using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class CardHolder : MonoBehaviour
{
    protected List<Card> cardsOnHand = new List<Card>();
    protected List<Card> mSelectedCards = new List<Card>();

    public async virtual void initializeCards() { await Task.Delay(1); }
    public virtual bool hasInitialized() { return false; }

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

    public Card getLowestCard()
    {
        Card lowestCard = cardsOnHand[0];
        foreach (Card card in cardsOnHand)
        {
            if (card != lowestCard && card.getCardValue() < lowestCard.getCardValue())
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
            if (card != highestCard && card.getCardValue() > highestCard.getCardValue())
            {
                highestCard = card;
            }
        }

        return highestCard;
    }

    public bool playSelectedCards()
    {
        if (mSelectedCards.Count == 0)
        {
            GameBase.Instance.Table.Clear();
            return true;
        }

        List<Card> selectedCards = new List<Card>(mSelectedCards);

        if (!GameBase.Instance.OnConfirmMove(selectedCards))
        {
            return false;
        }

        while (mSelectedCards.Count > 0)
        {
            cardsOnHand.Remove(mSelectedCards[0]);

            mSelectedCards[0].transform.gameObject.SetActive(false);
            mSelectedCards.RemoveAt(0);
        }
        GameBase.Instance.Table.SetActiveCards(selectedCards);

        return true;
    }

    void Start()
    {
        cardsOnHand.AddRange(this.transform.GetComponentsInChildren<Card>());
        this.enabled = false;
    }

}
