using System;
using UnityEngine;

public abstract class MiniGamePlayerController : MonoBehaviour
{
    public virtual void OnMiniGameStateChanged(MiniGameDirectorBase.MiniGameState newValue)
    {
        switch (newValue)
        {
            case MiniGameDirectorBase.MiniGameState.None:
                break;
            case MiniGameDirectorBase.MiniGameState.Intro:
                OnIntro();
                break;
            case MiniGameDirectorBase.MiniGameState.PrePlaying:
                OnPrePlaying();
                break;
            case MiniGameDirectorBase.MiniGameState.Playing:
                OnPlaying();
                break;
            case MiniGameDirectorBase.MiniGameState.PostPlaying:
                OnPostPlaying();
                break;
            case MiniGameDirectorBase.MiniGameState.OutroSuccess:
            case MiniGameDirectorBase.MiniGameState.OutroFailed:
            case MiniGameDirectorBase.MiniGameState.OutroReset:
            case MiniGameDirectorBase.MiniGameState.Exit:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newValue), newValue, null);
        }
    }
    
    public virtual void OnIntro() {}
    public virtual void OnPrePlaying(){}
    public virtual void OnPlaying(){}
    public virtual void OnPostPlaying() {}
    
    public virtual void FinishGame(bool timeOut)
    {
        Debug.LogError($"FinishGame()이 PlayerController에 구현되지 않았습니다. TimeOut:{timeOut}");
    }
}
