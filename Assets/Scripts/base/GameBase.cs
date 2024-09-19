using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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
    public Pond Pond;

    [HideInInspector]
    public List<string> cardsInPlay = new List<string>();

    Task loadTask;

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

            mOtherHandsIdx = -1;
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
        else if (Pond.remainingCardsOnHand() < 1)
        {
            OnPlayerWin(null);
            mGameState = GameState.End;
        }
        else
        {
            mActivePlayer.onEndTurn();

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

    public IEnumerator ForceEndGame()
    {
        yield return new WaitForSeconds(1.0f);

        if (mActivePlayer.remainingCardsOnHand() == 0)
        {
            OnPlayerWin(mActivePlayer);
            mGameState = GameState.End;
        }
        else
        {
            OnPlayerWin(null);
            mGameState = GameState.End;
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
                StartCoroutine(PreInitGame());
                mGameState = GameState.Init;
                break;
            }
            case GameState.Init:
            {
                if (InitGame())
                {
                    mGameState = GameState.Loop;
                }
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

    IEnumerator PreInitGame()
    {
        StartCoroutine(ThisPlayer.initializeCards());
        while (!ThisPlayer.hasInitialized())
        {
            yield return null;
        }

        foreach (CardHolder player in OtherPlayers)
        {
            StartCoroutine(player.initializeCards());
            while (!player.hasInitialized())
            {
                yield return null;
            }
        }

        StartCoroutine(Pond.initializeCards());
        while (!Pond.hasInitialized())
        {
            yield return null;
        }

    }

    bool InitGame()
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

        if (!Pond.hasInitialized())
        {
            preInitIsDone = false;
        }

        if (preInitIsDone)
        {
            OnGameInit();
            return true;
        }
        
        return false;
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
