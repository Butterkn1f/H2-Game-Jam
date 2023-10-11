using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Audio List", menuName = "Other Objects/Audio List", order = 2)]
public class AudioList : ScriptableObject
{
    public List<AudioTrack> _tracks;
}

[Serializable]
public class AudioTrack
{
    public SoundUID UniqueIdentifier;
    public AudioType Type;
    public AudioClip Clip;

    // Settings
    [Tooltip("Loop (for music only)")]
    public bool Loop = false;
}
