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

    public void ReloadGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public override void OnGameInit()
    {
        Game.Instance.ActivePlayer = (Random.value < 0.5f) ? Game.Instance.ThisPlayer : Game.Instance.OtherPlayers[0];
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
        CardHolder otherPlayer = (Game.Instance.ActivePlayer == Game.Instance.ThisPlayer) ? Game.Instance.OtherPlayers[0] : Game.Instance.ThisPlayer;

        Card cardToGive;

        cardToGive = otherPlayer.getCard(otherPlayer.hasCardRank(pCardsToPlay[0].cardRankIndex));
        if (cardToGive != null)
        {
            pCardsToPlay.Add(cardToGive);
            return true;
        }

        cardToGive = Pond.GetTopCard();
        if (cardToGive != null)
        {
            if (cardToGive.cardRankIndex == pCardsToPlay[0].cardRankIndex)
            {
                return true;
            }

            Game.Instance.ActivePlayer.receiveCard(cardToGive);
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
        return true;
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
}
