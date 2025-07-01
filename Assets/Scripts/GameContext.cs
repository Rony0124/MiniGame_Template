using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameContext : MonoBehaviour
{
    public static GameContext Singleton { get; private set; }

    public MiniGameDirectorBase LocalDirector { get; set; }

    private void Awake()
    {
        //싱글턴 간단하게 구현
        Singleton = this;
            
        DontDestroyOnLoad(this);
    }
    
    public static T GetDirector<T>() where T : MiniGameDirectorBase
    {
        if (Singleton)
        {
            return Singleton.LocalDirector as T;
        }
        
        Debug.LogWarning("Singleton이 유효하지 않습니다. 순서 문제가 잠재해있을 수 있습니다.");
        return null;
    }
}
