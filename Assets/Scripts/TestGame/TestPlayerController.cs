using System.Collections;
using UnityEngine;

public class TestPlayerController : MiniGamePlayerController
{
    private int score;
    public int Score
    {
        get => score;
        set
        {
            if (score != value)
            {
                OnScoreChanged(score, value);    
            }

            score = value;
        }
    }
    
    protected override IEnumerator InitOnStart()
    {
        yield return base.InitOnStart();
        
        //init 
    }

    private void OnScoreChanged(int oldScore, int newScore)
    {
        if (oldScore < newScore)
        {
            if (miniGameDirector)
                miniGameDirector.SetNumberRecord(newScore);
        }
    }
  
    public void OnTimeOver()
    {
        Debug.Log("Player Timer over");
    }
}
