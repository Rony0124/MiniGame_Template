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
}
