using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common.DesignPatterns;
using System.Linq;
using DG.Tweening;
using UniRx;

namespace Audio
{
    [RequireComponent(typeof(AudioSource))]
    // The controller of the audio system 
    public class AudioController : Singleton<AudioController>
    {

        #region variable

        public bool IsDebug;

        [SerializeField] private AudioList _audioTracks;
        private Hashtable _audioTable; // relationship of audio audioIDs and tracks
        private Hashtable _jobTable; // relationship between audio audioIDs and jobs
        
        #endregion

        // Start is called before the first frame update
        void Start()
        {
            _audioTable = new Hashtable();
            _jobTable = new Hashtable();

            GenerateAudioTable();
        }

        void OnDisable()
        {
            // Dispose of all jobs on quit
            Dispose();
        }

        #region public functions
        
        // Play an audio track
        public void PlayAudio(SoundUID audioID, bool loop = false, bool fade = false, float fadeDuration = 1.0f, float delay = 0f)
        {
            AddJob(new AudioJob(AudioAction.START, audioID, loop, fade, delay, fadeDuration));
        }

        // Stop an audio track
        public void StopAudio(SoundUID audioID, bool loop = false, bool fade = false, float fadeDuration = 1.0f, float delay = 0f)
        {
            AddJob(new AudioJob(AudioAction.STOP, audioID, loop, fade, delay, fadeDuration));
        }

        // Restart an audio track
        public void RestartAudio(SoundUID audioID, bool loop = false, bool fade = false, float fadeDuration = 1.0f, float delay = 0f)
        {
            AddJob(new AudioJob(AudioAction.RESTART, audioID, loop, fade, delay, fadeDuration));
        }

        #endregion

        #region private functions

        private void Dispose()
        {
            // Remove all existing audio tracks
            foreach (DictionaryEntry kvp in _jobTable)
            {
                // Get the current job from dictionary and stop it 
                Coroutine _job = (Coroutine)kvp.Value;
                if (_job != null) {
                    StopCoroutine(_job);
                }
            }
        }

        private void AddJob(AudioJob jobToAdd)
        {
            // Cancel any jobs that may be using this job's audio source
            // Prevents double tracks playing <3
            RemoveConflictingJobs(jobToAdd.AudioID);

            Coroutine jobRunner = StartCoroutine(RunAudioJob(jobToAdd));
            _jobTable.Add(jobToAdd.AudioID, jobRunner);
        }

        private void RemoveJob(SoundUID audioID)
        {
            if (!_jobTable.ContainsKey(audioID))
            {
                // Self explainatory error 
                Log("Trying to stop a job that is not currently running");
                return;
            }

            // If it exists, stop it
            Coroutine runningJob = (Coroutine)_jobTable[audioID];
            if (runningJob != null) {
                StopCoroutine(runningJob);
            }
            // Remove it from job table
            _jobTable.Remove(audioID);
        }
        
        // Remove any conflicting jobs 
        private void RemoveConflictingJobs(SoundUID audioID)
        {
            // Cancel the job if one exists with the same audioID
            if (_jobTable.ContainsKey(audioID))
            {
                RemoveJob(audioID);
            }

            // cancel the job if they share the same audio track
            SoundUID conflictAudio = SoundUID.NONE;
            AudioTrack _audioTrackNeeded = GetAudioTrack(audioID, "Get Audio Track Needed");
            foreach (DictionaryEntry entry in _jobTable)
            {
                SoundUID audioTempID = (SoundUID)entry.Key;
                AudioTrack audioTrackInUse = GetAudioTrack(audioTempID, "Get audio track in use");
                if (audioTrackInUse.Source == _audioTrackNeeded.Source)
                {
                    conflictAudio = audioTempID;
                    break;
                }
            }

            if (conflictAudio != SoundUID.NONE)
            {
                RemoveJob(conflictAudio);
            }
        }

        private IEnumerator RunAudioJob(AudioJob job)
        {
            if (job.Delay != null) yield return job.Delay;

            AudioTrack track = GetAudioTrack(job.AudioID);
            track.Source.clip = GetAudioClipFromAudioTrack(job.AudioID, track);

            switch (job.Action)
            {
                case AudioAction.START:
                    track.Source.Play();
                break;

                case AudioAction.STOP:
                    track.Source.Stop();
                break;

                case AudioAction.RESTART:
                    track.Source.Stop();
                    track.Source.Play();
                break;
            }

            float target = 1f;

            // fade volume
            // TODO: make this become editable
            if (job.Fade) {
                track.Source.DOFade(target, 1.0f);

                if (job.Action == AudioAction.STOP) {
                    track.Source.Stop();
                }
            }

            if (job.Loop)
            {
                track.Source.loop = true;
            }

            _jobTable.Remove(job.AudioID);
        }

        private void GenerateAudioTable()
        {
            foreach (AudioTrack track in _audioTracks._tracks)
            {
                if (track.Source == null)
                {
                    track.Source = GetComponent<AudioSource>();
                }

                foreach (AudioObject obj in track.Audio)
                {
                    // do not duplicate keys
                    if (_audioTable.ContainsKey(obj.AudioID))
                    {
                        // Log a warning bestayyyy
                    }
                    else 
                    {
                        _audioTable.Add(obj.AudioID, track);
                    }
                }
            }
        }

        private AudioTrack GetAudioTrack(SoundUID audioID, string job = "")
        {
            if (!_audioTable.ContainsKey(audioID))
            {
                LogWarning("You are trying to " + job + " for " + audioID + " but no track was found");
                return null;
            }

            return (AudioTrack)_audioTable[audioID];
        }

        private AudioClip GetAudioClipFromAudioTrack(SoundUID audioID, AudioTrack track)
        {
            AudioClip result = null;
            // Get the first or null
            result = track.Audio.Where(obj => obj.AudioID == audioID).Select(obj => obj.Clip).FirstOrDefault();

            return result;
        }

        private void Log(string _msg) {
            if (!IsDebug) return;
            Debug.Log("[Audio Controller]: "+_msg);
        }
        
        private void LogWarning(string _msg) {
            if (!IsDebug) return;
            Debug.LogWarning("[Audio Controller]: "+_msg);
        }


        #endregion
    } 
}

