using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSelectAudio : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        AudioManager.Instance.PlayAudio(SoundUID.MAP_SELECT_AUDIO);   
    }
}
