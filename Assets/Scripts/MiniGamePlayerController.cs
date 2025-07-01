using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MiniGamePlayerController : MonoBehaviour
{
    public virtual void FinishGame(bool timeOut)
    {
        Debug.LogError($"FinishGame()이 PlayerController에 구현되지 않았습니다. TimeOut:{timeOut}");
    }
}
