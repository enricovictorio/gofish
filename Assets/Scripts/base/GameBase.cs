using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public abstract class GameBase : MonoBehaviour
{
    public enum GameState
    {
        Invalid = 0,
        PreInit,
        Init,
        Loop,
        End
    }

    public static GameBase Instance { get; private set; }

    public CardHolder ThisPlayer;
    public List<CardHolder> OtherPlayers;
    public Table Table;

    [HideInInspector]
    public List<string> cardsInPlay = new List<string>();

    public CardHolder ActivePlayer {  
        get {  return this.mActivePlayer; } 
        set
        {
            bool isValidPlayer = false;

            if (value == ThisPlayer)
            {
                isValidPlayer = true;
            }
            else
            {
                for (int i = 0; i < OtherPlayers.Count; i++)
                {
                    if (value.Equals(OtherPlayers[i]))
                    {
                        isValidPlayer = true;
                        break;
                    }
                }
            }

            Debug.Assert(isValidPlayer);

            mActivePlayer = value;
            for (int i = 0; i < OtherPlayers.Count; i++)
            {
                if (value.Equals(OtherPlayers[i]))
                {
                    mOtherHandsIdx = i;
                    break;
                }
            }
        }
    }

    private GameState mGameState = GameState.PreInit;

    private CardHolder mActivePlayer;
    private int mOtherHandsIdx;

    public abstract void OnGameInit();
    public abstract void OnGameLoop();
    public abstract void OnGameEnd();

    public abstract bool OnConfirmMove(List<Card> pCardsToPlay);

    public abstract void OnPlayerWin(CardHolder pPlayer);

    public void PlayNextTurn()
    {
        if (mActivePlayer.remainingCardsOnHand() == 0)
        {
            OnPlayerWin(mActivePlayer);
            mGameState = GameState.End;
        }
        else
        {
            mActivePlayer.enabled = false;
            mOtherHandsIdx++;

            if (mOtherHandsIdx == OtherPlayers.Count)
            {
                mOtherHandsIdx = -1;
                mActivePlayer = ThisPlayer;
            }
            else
            {
                mActivePlayer = OtherPlayers[mOtherHandsIdx];
            }
            mActivePlayer.enabled = true;
        }
    }

    void Start()
    {
        ResourceManager.Initialize();

        Instance = this;
    }

    void Update()
    {
        switch(mGameState)
        {
            case GameState.PreInit:
            {
                PreInitGame();
                break;
            }
            case GameState.Init:
            {
                InitGame();
                break;
            }
            case GameState.Loop:
            {
                LoopGame();
                break;
            }
            case GameState.End:
            {
                EndGame();
                break;
            }
        }

    }

    void PreInitGame()
    {
        ThisPlayer.initializeCards();
        foreach (CardHolder player in OtherPlayers)
        {
            player.initializeCards();
        }

        mGameState = GameState.Init;
    }

    void InitGame()
    {
        bool preInitIsDone = true;

        if (!ThisPlayer.hasInitialized())
        {
            preInitIsDone = false;
        }

        foreach (CardHolder player in OtherPlayers)
        {
            if (!player.hasInitialized())
            {
                preInitIsDone = false;
                break;
            }
        }

        if (preInitIsDone)
        {
            OnGameInit();
            mGameState = GameState.Loop;
        }
    }

    void LoopGame()
    {
        OnGameLoop();
    }

    void EndGame()
    {
        OnGameEnd();
        mGameState = GameState.Invalid;
    }

}
