using System;
using System.Collections;
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
    }
    
    [SerializeField] 
    protected MiniGamePlayerController playerController;
    
    [Header("MiniGame Info")]
    public MiniGameInfo miniGameInfo;
    
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
        miniGameTimer = MiniGameFramework.Singleton.MiniGameTimer;
        
        //페이드 설정
    }
    
    private void StartGame()
    {
        OnMiniGameStateChanged(MiniGameState.Playing);
    }
    
    public void ResetGame(bool realGame)
    {
        //게임 결과 clear
        
        RealGame = realGame;
        
        OnMiniGameStateChanged(realGame ? MiniGameState.Intro : MiniGameState.PrePlaying);
    }
    
    public virtual void FinishGame()
    {
        playerController.FinishGame(true);
        OnMiniGameStateChanged(MiniGameState.PostPlaying);
    }
    
    protected virtual void OnMiniGameStateChanged(MiniGameState newMiniGameState)
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
                StartCoroutine(StartExitCoroutine());
                break;
        }

        playerController.OnMiniGameStateChanged(newMiniGameState);

        currentMiniGameSate = newMiniGameState;
    }
    
    protected virtual IEnumerator StartIntroCoroutine()
    {
        //intro가 있다면 여기서 플레이
        if(miniGameInfo.completeWithinTimeLimit)
            MiniGameFramework.Singleton.SetTimer(miniGameInfo);
        
        yield return null;
        OnMiniGameStateChanged(MiniGameState.PrePlaying);
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
            OnMiniGameStateChanged(IsMiniGameResultsSuccess()
                ? MiniGameState.OutroSuccess
                : MiniGameState.OutroFailed);
        }
        else
        {
            OnMiniGameStateChanged(MiniGameState.OutroReset);
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
        
        OnMiniGameStateChanged(MiniGameState.Exit);
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
                
        OnMiniGameStateChanged(MiniGameState.PostPlaying);

        if (RealGame)
        {
            gameEnded = true;
        }
    }
    
    public virtual void SetNumberRecord(int number)
    { 
        var currentRecord = GameState.Singleton.LastMiniGameRecord;
        currentRecord.Pass = false; 
        currentRecord.PassedInGame = true;
        currentRecord.ScoreRank = 0;
        currentRecord.NumberRecord = number;
        
        SetGameRecord(currentRecord);
    }
      
    public virtual void EndNumberRecord()
    {
        var currentRecord = GameState.Singleton.LastMiniGameRecord;
        currentRecord.Final = true;
        currentRecord.PassedInGame = true;
        
        if (currentRecord.ScoreRank == 0)
            currentRecord.PassedInGame = false;

        SetGameRecord(currentRecord);
    }
    
    public virtual void StartTimeRecordServerRpc(float startTime)
    {
        GameState.GameRecord gameRecord = default; 
        gameRecord.StartTime = startTime;
        gameRecord.Pass = false;
        gameRecord.PassedInGame = true;
        gameRecord.ScoreRank = 0;
        gameRecord.AscendingOrder = (miniGameInfo.resultRecordOrder == MiniGameInfo.ResultRecordOrder.LesserTimeSpan);
        
        SetGameRecord(gameRecord);
    }
    
    public virtual void EndTimeRecord(float endTime, bool timeOut)
    {
        var currentRecord = GameState.Singleton.LastMiniGameRecord;
        currentRecord.TimeSpanRecord = timeOut ? miniGameTimer.firstGoalTimeSecond : endTime - currentRecord.StartTime;
        currentRecord.ScoreRank = GetScoreRank(currentRecord.TimeSpanRecord, currentRecord.AscendingOrder);
        if (currentRecord.ScoreRank == 0)
            currentRecord.PassedInGame = false;
        
        currentRecord.Final = true;
            
        SetGameRecord(currentRecord);
    }
    
    private int GetScoreRank(float score, bool ascendingOrder)
    {
        var currentDifficulty = GameState.Singleton.CurrentMiniGameDifficulty;
        var cutoffs = new float[3];
        cutoffs[0] = miniGameInfo.gameCutOffs[0].Evaluate(currentDifficulty);
        cutoffs[1] = miniGameInfo.gameCutOffs[1].Evaluate(currentDifficulty);
        cutoffs[2] = miniGameInfo.gameCutOffs[2].Evaluate(currentDifficulty);
        
        if (!ascendingOrder)
        {
            if (score < cutoffs[0])
                return 0;
            if (score >= cutoffs[0] && score < cutoffs[1])
                return 1;
            if (score >= cutoffs[1] && score < cutoffs[2])
                return 2;
                    
            return 3;
        }
        
        if (miniGameInfo.completeWithinTimeLimit)
        {
            cutoffs[0] = miniGameTimer.firstGoalTimeSecond;
            
            if (score >= cutoffs[0])
                return 0;
            if (score < cutoffs[0] && score > cutoffs[1])
                return 1;
            if (score <= cutoffs[1] && score > cutoffs[2])
                return 2;
                    
            return 3;
        }
                
        if (score > cutoffs[0])
            return 0;
        if (score <= cutoffs[0] && score > cutoffs[1])
            return 1;
        if (score <= cutoffs[1] && score > cutoffs[2])
            return 2;
                    
        return 3;
    }
    
    public bool CheckPlayerReady()
    {
        return GameState.Singleton && GameState.Singleton.CheckPlayerReady();
    }
}
