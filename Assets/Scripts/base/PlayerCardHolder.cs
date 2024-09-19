using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class PlayerCardHolder : Player
{
    [SerializeField]
    private GameObject mPlayButton;

    private int mPlayerState = -1;
    private bool mPlayerHasInitialized = false;

    public override IEnumerator initializeCards()
    {
        int numCardsInHand = GetComponentsInChildren<Card>().Length;

        int i = 0;
        while (i < numCardsInHand)
        {
            string thisCardRank = Card.CARDRANK[UnityEngine.Random.Range(0, Card.CARDRANK.Length)];
            string thisCardSuit = Card.CARDSUITS[UnityEngine.Random.Range(0, Card.CARDSUITS.Length)];
            string cardName = thisCardRank + "_of_" + thisCardSuit;

            if (!Game.Instance.cardsInPlay.Contains(cardName) && cardsOnHand[i].setCardFace(cardName))
            {
                Game.Instance.cardsInPlay.Add(cardName);
                i++;
            }
            else
            {
                yield return null;
            }
        }

        updateHandLayout();

        mPlayButton.SetActive(false);
        mPlayerHasInitialized = true;
    }
    public override bool hasInitialized() 
    { 
        return mPlayerHasInitialized; 
    }

    public override void onEndTurn()
    {
        mPlayButton.SetActive(false);

        mPlayerState = -1;
        this.enabled = false;
    }

    public void OnPlayBtnClick()
    {
        if (mSelectedCards.Count == 0)
        {
            Debug.Log("Must select a card first!");
            return;
        }

        mPlayerState = 1;
    }

    void Update()
    {
        switch(mPlayerState)
        {
            case 0:
            {
                mPlayButton.SetActive(true);
                if (mSelectedCards.Count < 2)
                {
                    mPlayButton.GetComponentInChildren<TextMeshProUGUI>().text = "Ask!";
                }
                else if (mSelectedCards.Count < 3)
                {
                    mPlayButton.GetComponentInChildren<TextMeshProUGUI>().text = "Play!";
                }

                selectCard();
                break;
            }
            case 1:
            {
                if (mSelectedCards.Count == 1)
                {
                    Game.Instance.OnConfirmMove(mSelectedCards);
                }

                if (mSelectedCards.Count == 1)
                {
                    while(mSelectedCards.Count > 0)
                    {
                        mSelectedCards[0].select(false);
                        mSelectedCards.Remove(mSelectedCards[0]);
                    }
                    Game.Instance.PlayNextTurn();
                }
                else if (mSelectedCards[0].cardRankIndex == mSelectedCards[1].cardRankIndex)
                {
                    playSelectedCards();
                    mPlayerState = 0;
                }
                break;
            }
        }
    }

    void OnEnable()
    {
        mSelectedCards.Clear();
        mPlayerState = 0;
    }

    bool selectCard()
    {
        Vector2 touchPos = Vector2.zero;

#if UNITY_EDITOR
        if (Input.GetButtonDown("Fire1"))
        {
            touchPos = Input.mousePosition;
        }
#else
        if (Input.touches.Length > 0)
        {
            touchPos = Input.touches[0].position;
        }
#endif

        if (touchPos != Vector2.zero)
        {
            Ray touch3D = Camera.main.ScreenPointToRay(touchPos);
            RaycastHit[] selectedCards = Physics.RaycastAll(touch3D);
            if (selectedCards.Length > 0)
            {
                Array.Sort(selectedCards, (a, b) =>
                {
                    if (a.transform.position.z < b.transform.position.z) return -1;
                    else if (a.transform.position.z > b.transform.position.z) return 1;
                    else return 0;
                });

                RaycastHit selectedCardHit = selectedCards[0];
                Card selectedCard = selectedCardHit.transform.GetComponent<Card>();

                Debug.Assert(selectedCard != null);

                if (mSelectedCards.Contains(selectedCard))
                {
                    selectedCard.select(false);
                    mSelectedCards.Remove(selectedCard);
                }
                else if (mSelectedCards.Count < 2)
                {
                    selectedCard.select(true);
                    mSelectedCards.Add(selectedCard);
                }
                else
                {
                    Debug.Log("Unable to select card.");
                }
            }
        }

        return true;
    }
}
