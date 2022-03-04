using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameEvent : MonoBehaviour
{
    public static GameEvent current;
    // Start is called before the first frame update
    public void Awake()
    {
        current = this;
    }

    public event Action onNewEpisodeBeginAgent;
    public void NewEpisodeBeginAgent() {
        if (onNewEpisodeBeginAgent != null)
        {
            onNewEpisodeBeginAgent();
        }
    }
}
