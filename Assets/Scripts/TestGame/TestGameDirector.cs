using UnityEngine;

public class TestGameDirector : MiniGameDirectorBase
{
    protected override void OnMiniGameStateChanged(MiniGameState newValue)
    {
        base.OnMiniGameStateChanged(newValue);
        
        switch (newValue)
        {
            case MiniGameState.PrePlaying:
                //초기화
                break;
            case MiniGameState.Playing:
                
                break;
        }
    }
    
    public void OnTimeOver()
    {
        Debug.Log("Director Timer over");
        
        var testPlayer = playerController as TestPlayerController;
        if(testPlayer)
            testPlayer.OnTimeOver();
    }
}
