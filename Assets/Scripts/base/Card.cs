using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public static string[] CARDRANK = { "2", "3", "4", "5", "6", "7", "8", "9", "jack", "queen", "king", "ace" };
    public static string[] CARDSUITS = { "clubs", "spades", "hearts", "diamonds" };

    public static Dictionary<string, int> cardPoints = new Dictionary<string, int>()
    {
        { "clubs", 1 },
        { "spades", 2 },
        { "hearts", 3 },
        { "diamonds", 4 },
    };
    public static Dictionary<string, int> cardPointMultiplier = new Dictionary<string, int>()
    {
        { "3", 10 },
        { "4", 20 },
        { "5", 30 },
        { "6", 40 },
        { "7", 50 },
        { "8", 60 },
        { "9", 70 },
        { "10", 80 },
        { "jack", 90 },
        { "queen", 100 },
        { "king", 110 },
        { "ace", 120 },
        { "2", 130 },
    };


    public string cardName {  get; private set; }
    public int cardValue
    {
        get
        {
            int cardNumValue1 = cardPointMultiplier[CARDRANK[cardRankIndex]];
            int cardNumValue2 = cardPoints[CARDSUITS[cardSuitIndex]];

            return cardNumValue1 + cardNumValue2;
        }
    }
    public int cardSuitIndex { get; private set; }
    public int cardRankIndex { get; private set; }

    private SpriteRenderer mSpriteRenderer;

    private bool mIsSelected = false;

    public bool setCardFace(string pCardName, bool pSetNameOnly = false)
    {
        string[] nameParts = pCardName.Split("_of_");
        string cardRank = nameParts[0].Replace("_of_", "");
        string cardSuit = nameParts[1].Replace("_of_", "");

        int cardRankIndex = -1;
        for (int i = 0; i < CARDRANK.Length; i++)
        {
            if (CARDRANK[i] == cardRank)
            {
                cardRankIndex = i;
                break;
            }
        }
        int cardSuitIndex = -1;
        for (int i = 0; i < CARDSUITS.Length; i++)
        {
            if (CARDSUITS[i] == cardSuit)
            {
                cardSuitIndex = i;
                break;
            }
        }

        return setCardFace(cardRankIndex, cardSuitIndex, pSetNameOnly);
    }

    public bool setCardFace(int pRankIndex, int pSuitIndex, bool pSetNameOnly = false)
    {
        cardSuitIndex = pSuitIndex;
        cardRankIndex = pRankIndex;
        cardName = CARDRANK[pRankIndex] + "_of_" + CARDSUITS[pSuitIndex];

        if (pSetNameOnly)
        {
            return true;
        }

        if (ResourceManager.Instance.isSpriteAvailable(cardName))
        {
            mSpriteRenderer.sprite = ResourceManager.Instance.getSprite(cardName);
            transform.parent.name = cardName;

            return true;
        }
        else
        {
            return false;
        }
    }

    public void select(bool pSelect)
    {
        if (pSelect)
        {
            transform.localPosition += (Vector3.up * 0.2f);
        }
        else
        {
            transform.localPosition -= (Vector3.up * 0.2f);
        }

        mIsSelected = pSelect;
    }

    public bool isSelected() { return mIsSelected; }

    void Start()
    {
        mSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        
    }
}
