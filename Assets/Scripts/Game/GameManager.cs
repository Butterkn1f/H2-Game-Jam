using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Common.DesignPatterns.SingletonPersistent<GameManager>
{
    // TODO: Different game states
    // Start is called before the first frame update
    void Start()
    {
        Input.multiTouchEnabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
