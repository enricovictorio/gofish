using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Pond : CardHolder
{
    public GameObject CardPrefab;

    bool mPondHasInitialized = false;

    public override IEnumerator initializeCards()
    {
        List<string> availableCardNames = new List<string>();

        for (int i = 0; i < Card.CARDSUITS.Length; i++)
        {
            for(int j = 0; j < Card.CARDRANK.Length; j++)
            {
                string thisCardRank = Card.CARDRANK[j];
                string thisCardSuit = Card.CARDSUITS[i];
                string cardName = thisCardRank + "_of_" + thisCardSuit;

                if (!Game.Instance.cardsInPlay.Contains(cardName))
                {
                    availableCardNames.Add(cardName);
                }
                else
                {
                    yield return null;
                }
            }
        }

        cardsOnHand.Clear();
        while (availableCardNames.Count > 0)
        {
            string cardName = availableCardNames[UnityEngine.Random.Range(0, availableCardNames.Count)];
            availableCardNames.Remove(cardName);

            GameObject newCard = GameObject.Instantiate(CardPrefab);
            newCard.transform.SetParent(transform);
            newCard.transform.position = new Vector3(0, -10, 0);

            Card cardComponent = newCard.GetComponentInChildren<Card>();
            Debug.Assert(cardComponent != null);

            if (cardComponent.setCardFace(cardName))
            {
                cardsOnHand.Add(cardComponent);
                Game.Instance.cardsInPlay.Add(cardName);
            }
        }

        mPondHasInitialized = true;
    }
    public override bool hasInitialized() { return mPondHasInitialized; }

    public Card GetTopCard()
    {
        Card retVal = cardsOnHand[0];
        cardsOnHand.Remove(retVal);

        return retVal;
    }
}
