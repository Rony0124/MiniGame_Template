using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MiniGameDirectorBase : MonoBehaviour
{
    public enum MiniGameState
    {
        None,
        Intro,              // 시작시 짧은 Intro Timeline을 재생 단계
        PrePlaying,         // 플레이 준비 단계 (퍼즐을 섞는 다던지, 룰 이후에 보여줘야할 것들)
        Playing,            // 실제 플레이 단계
        PostPlaying,        // 타임아웃이나 클리어 등의 조건으로 플레이가 종료된 단계 (여기서 아웃트로를 선택)
        OutroSuccess,
        OutroFailed,
        OutroReset,
        Exit,               // 화면 끄는 연출하고 스테이지로 돌아간다.
    };


    private MiniGamePlayerController playerController;
    private MiniGameTimer miniGameTimer;
    private bool realGame;
    private MiniGameState currentMiniGameSate;
    private float gameDifficulty;
    private bool gameEnded;

    public bool RealGame
    {
        get => realGame;
        private set
        {
            OnRealGameValueChanged(value);
            realGame = value;
        }
    }

    public MiniGameState CurrentMiniGameSate => currentMiniGameSate;
    public float GameDifficulty => gameDifficulty;

    protected virtual void Awake()
    {
        GameContext.Singleton.LocalDirector = this;
    }

    protected virtual void Start()
    {
        //타이머
        
        //info 설정
        
        //페이드
     
        //stage info 설정
        
        //playercontroller
    }
    
    private void StartGame()
    {
        ChangeMiniGameState(MiniGameState.Playing);
    }
    
    public void ResetGame(bool realGame)
    {
        //게임 결과 clear
        
        RealGame = realGame;
        
        ChangeMiniGameState(realGame ? MiniGameState.Intro : MiniGameState.PrePlaying);
    }
    
    public virtual void FinishGame()
    {
        playerController.FinishGame(true);
        ChangeMiniGameState(MiniGameState.PostPlaying);
    }
    
    private void ChangeMiniGameState(MiniGameState newMiniGameState)
    {
        Debug.Log($"현재 MiniGameState:{newMiniGameState} / RealGame:{RealGame}");
        switch (newMiniGameState)
        {
            case MiniGameState.Intro:
                //인트로는 본 게임에서만 재생
                RealGame = true;

                StartCoroutine(StartIntroCoroutine());
                break;
            case MiniGameState.PrePlaying:
                StartCoroutine(StartPrePlayingCoroutine());
                break;
            case MiniGameState.Playing:
                if (miniGameTimer)
                    miniGameTimer.BeginTimer(true);
                break;
            case MiniGameState.PostPlaying:
                if (miniGameTimer)
                    miniGameTimer.PauseTimer();
                
                StartCoroutine(StartPostPlayingCoroutine());
                break;
            case MiniGameState.OutroSuccess:
            case MiniGameState.OutroFailed:
                StartCoroutine(StartOutroCoroutine(newMiniGameState));
                break;
            case MiniGameState.OutroReset:
                StartCoroutine(StartOutroResetCoroutine());
                break;
            case MiniGameState.Exit:
                break;
        }

        currentMiniGameSate = newMiniGameState;
    }
    
    protected virtual IEnumerator StartIntroCoroutine()
    {
        //intro가 있다면 여기서 플레이
        yield return null;
        ChangeMiniGameState(MiniGameState.PrePlaying);
    }
    
    private IEnumerator StartPrePlayingCoroutine()
    {
        //preplay 과정은 여기서 철기
        yield return null;

        StartGame();
    }
    
    private IEnumerator StartPostPlayingCoroutine()
    {
        yield return null;
            
        if (RealGame)
        {
            ChangeMiniGameState(IsMiniGameResultsSuccess()
                ? MiniGameState.OutroSuccess
                : MiniGameState.OutroFailed);
        }
        else
        {
            ChangeMiniGameState(MiniGameState.OutroReset);
        }
    }
    
    private IEnumerator StartOutroCoroutine(MiniGameState currentMiniGameState)
    {
        if (miniGameTimer)
            miniGameTimer.PauseTimer();
        
        if (currentMiniGameState == MiniGameState.OutroSuccess)
        {
            Debug.Log("성공 outro");
        }
            
        if (currentMiniGameState == MiniGameState.OutroFailed)
        {
            Debug.Log("실패 outro");
        }
        
        yield return new WaitForSeconds(1.0f);
        
        //게임 result UI 보여주기
        
        
        if (!GameState.Singleton.IsPracticeMode)
        {
            Debug.Log("실게임 이후");
        }
        
        ChangeMiniGameState(MiniGameState.Exit);
    }
    
    private IEnumerator StartOutroResetCoroutine()
    {
        yield return null;
        ResetGame(CheckPlayerReady());
    }
    
    private IEnumerator StartExitCoroutine()
    {
        //exit ui 보여주기

        yield return null;

        if (GameState.Singleton.IsPracticeMode)
        {
            //연습모드에서 불린다면 뒤로 돌아가기
        }
        else
        {
            Debug.Log("해당 미니게임은 종료");
        }
    }

    
    private void OnRealGameValueChanged(bool isRealGame)
    {
        //실게임, 연습게임 UI구분
    }
    
    protected virtual bool IsMiniGameResultsSuccess()
    {
        //게임 결과 실패, 성공 판단
        return GameState.Singleton.IsLastMiniGameSuccess();
    }
    
    public virtual void SetGameRecord(GameState.GameRecord gameRecord)
    {
        if (gameEnded)
            return;

        if (CurrentMiniGameSate != MiniGameState.Playing)
            return;

        GameState.Singleton.SetCurrentMiniGameRecord(gameRecord);
                
        ChangeMiniGameState(MiniGameState.PostPlaying);

        if (RealGame)
        {
            gameEnded = true;
        }
    }
    
    public bool CheckPlayerReady()
    {
        return GameState.Singleton && GameState.Singleton.CheckPlayerReady();
    }
}
