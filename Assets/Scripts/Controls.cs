using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controls : MonoBehaviour
{
    private bool Playable;

    // Start is called before the first frame update
    void Start()
    {
        Playable = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool GetPlayable()
    {
        return Playable;
    }

    public void SetPlayable(bool action)
    {
        Playable = action;
    }
}
