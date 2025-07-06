using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGameFramework : MonoBehaviour
{
    public static MiniGameFramework Singleton { get; private set; }
    
    [Header("MiniGameTimer")]
    [SerializeField]
    private MiniGameTimer miniGameTimer;
    public MiniGameTimer MiniGameTimer => miniGameTimer;
    
    private void Awake()
    {
        //싱글턴 간단하게 구현
        Singleton = this;
        
        DontDestroyOnLoad(this);
    }
    
    public void SetTimer(MiniGameInfo miniGameInfo)
    {
        miniGameTimer.SetTimerToFirstCutOff((float)Math.Round(miniGameInfo.gameCutOffs[0]
            .Evaluate(GameState.Singleton.CurrentMiniGameDifficulty), MidpointRounding.AwayFromZero) + 0.001f);
    }
}
