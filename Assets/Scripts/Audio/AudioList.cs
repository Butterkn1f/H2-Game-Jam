using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Audio
{
    // Acts as a container for all the audio objects
    [CreateAssetMenu(menuName = "Custom data containers/Audio list")]
    public class AudioList : ScriptableObject
    {
        public List<AudioTrack> _tracks;
    }
}
