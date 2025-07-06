using System;
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
        public bool Counting;
        public bool Passed;
        public bool Final;
        public bool PassedInGame;
        public float StartTime;
        public int ScoreRank;
        public bool AscendingOrder;

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
            set => Score = Mathf.RoundToInt(value * 1000.0f);
        }

        public bool Count
        {
            get => Counting;
            set => Counting = value;
        }
    }

    public GameRecord LastMiniGameRecord { get; private set; }

    private int currentMiniGameDifficulty;
    public int CurrentMiniGameDifficulty
    {
        get
        {
            if (hasMiniGameDifficultyTestValue)
                return miniGameDifficultyForTest;

            return currentMiniGameDifficulty;
        }
    }

    public bool IsPracticeMode { get; private set; }

    [SerializeField] 
    private bool hasMiniGameDifficultyTestValue;
    [SerializeField] 
    private int miniGameDifficultyForTest;

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
