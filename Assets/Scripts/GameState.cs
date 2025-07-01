using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : MonoBehaviour
{
    public static GameState Singleton { get; private set; }
    
    [Serializable]
    public struct PlayerScore
    {
        public bool passed;
        
        public int miniGameScore;

        public bool Pass
        {
            get => passed;
            set => passed = value;
        }
    }

    public struct GameRecord
    {
        public int Score;
        public bool Passed;
        
        public bool Pass
        {
            get => Passed;
            set => Passed = value;
        }
        
        public int NumberRecord
        {
            get => Score;
            set => Score = value;
        }

        public float TimeSpanRecord
        {
            get => Score * 0.001f;
            set => Score = Mathf.RoundToInt(value*1000.0f);
        }
    }

    public GameRecord LastMiniGameRecord { get; private set; }
    public bool IsPracticeMode { get; private set; }

    private void Awake()
    {
        //싱글턴 간단하게 구현
        Singleton = this;
            
        DontDestroyOnLoad(this);
    }
    
    public bool IsLastMiniGameSuccess()
    {
        PlayerScore playerScore = CalcMiniGameResultScores(LastMiniGameRecord);
        return playerScore.passed;
    }
    
    public void SetCurrentMiniGameRecord(GameRecord gameRecord)
    {
        LastMiniGameRecord = gameRecord;
    }
    
    public PlayerScore CalcMiniGameResultScores(GameRecord gameRecord)
    {
        PlayerScore score = new PlayerScore
        {
            passed = gameRecord.Passed,
            miniGameScore = gameRecord.Score,
        };
        
        return score;
    }
    
    public void SetPracticeMode(bool isPractice)
    {
        IsPracticeMode = isPractice;
    }

    public bool CheckPlayerReady()
    {
        //TODO player ready 상태 체크
        return true;
    }
}
