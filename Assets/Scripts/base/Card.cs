using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    public static string[] CARDNUM = { "2", "3", "4", "5", "6", "7", "8", "9", "jack", "queen", "king", "ace" };
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

    private SpriteRenderer mSpriteRenderer;

    private bool mIsSelected = false;

    public int getCardValue()
    {
        string[] cardNameParts = cardName.Split("_of_");

        int cardNumValue1 = cardPointMultiplier[cardNameParts[0].Replace("_of_", "")];
        int cardNumValue2 = cardPoints[cardNameParts[1].Replace("_of_", "")];

        return cardNumValue1 + cardNumValue2;
    }

    public bool setCardFace(string pCardName, bool pSetNameOnly = false)
    {
        cardName = pCardName;

        if (pSetNameOnly)
        {
            return true;
        }

        if (ResourceManager.Instance.isSpriteAvailable(pCardName))
        {
            mSpriteRenderer.sprite = ResourceManager.Instance.getSprite(pCardName);
            transform.parent.name = pCardName;

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
