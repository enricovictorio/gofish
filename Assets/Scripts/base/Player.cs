using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : CardHolder
{
    public bool playSelectedCards()
    {
        List<Card> selectedCards = new List<Card>(mSelectedCards);

        while (mSelectedCards.Count > 0)
        {
            cardsOnHand.Remove(mSelectedCards[0]);

            mSelectedCards[0].transform.parent.gameObject.SetActive(false);
            mSelectedCards[0].transform.parent.parent = null;
            mSelectedCards.RemoveAt(0);
        }
        GameBase.Instance.Table.SetActiveCards(selectedCards);

        updateHandLayout();

        return true;
    }
}
