using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MiniGameTimer : MonoBehaviour
{
    public float firstGoalTimeSecond = 30.0f;
    public int countDownDisplayStartTime = 5;
    
    [SerializeField]
    private UnityEvent onTimerBegun;
    [SerializeField]
    private UnityEvent onTimerEnded;
    [SerializeField]
    private UnityEvent onTimerAdded;
    
    private bool timerBegun = false;
    private float timerBeginTime = 0.0f;
    private int lastRemainTime = -1;
    
    void Update()
    {
        if (!timerBegun)
            return;
        
        int remainTime = System.Math.Max(0, (int)System.Math.Ceiling(RemainTimeToGoalTime()));
        
        if (lastRemainTime != remainTime)
        {
            //초가 바뀌는 것 보여주기
            var localMiniGameDirector = GameContext.GetDirector<MiniGameDirectorBase>();

            if (localMiniGameDirector)
            {
                localMiniGameDirector.FinishGame();
            }
            
            Debug.Log($"현재 남아있는 시간(초){remainTime}");
            lastRemainTime = remainTime;
        }
        
        if (remainTime <= 0)
        {
            //게임 디렉터에서 finish game
            timerBegun = false;
            
            onTimerEnded?.Invoke();
        }
    }
    
    public void BeginTimer(bool showUI)
    {
        timerBegun = true;
        timerBeginTime = Time.time;
        lastRemainTime = -1;
       
        onTimerBegun?.Invoke();
    }

    public void PauseTimer()
    {
        timerBegun = false;
    }
    
    public float GetTimeSpan()
    {
        return Time.time - timerBeginTime;
    }
    
    public float RemainTimeToGoalTime()
    {
        return firstGoalTimeSecond - GetTimeSpan();
    }
}
