using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// List of all the different speakers
[CreateAssetMenu(menuName = "Custom data containers/Speaker list")]
public class SpeakerList : ScriptableObject
{
    public List<Speaker> speakerList;
}

[System.Serializable]
public class Speaker
{
    // Speaker name
    public string name;
    public SpeakerId SpeakerId;
    // All the different mood sprites
    public List<Sprite> moodImages;
}

public enum SpeakerId
{
    // a unique ID for each character (for easy management)
    TOAST,
    TOASTE
}
