using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioController : MonoBehaviour
{

    public string[] backgroundMusicTrackNames;
    public Sound[] sounds; // array of sounds, edit in the inspector

    Sound currentSong;
    string currentSongName;

    void Awake()
    {

        // makes is so that music doesnt stop when scene is changed / reloaded
        GameObject[] objs = GameObject.FindGameObjectsWithTag("AudioController");

        if (objs.Length > 1) {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(gameObject);

        // loop through and add an audio compenent for each Sound in the array
        foreach (Sound s in sounds) {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
        }
    }

    void Start() {
        // pick a song from the background music selection randomly
        currentSongName = backgroundMusicTrackNames[UnityEngine.Random.Range(0, backgroundMusicTrackNames.Length)];
        currentSong = Array.Find(sounds, sound => sound.name == currentSongName);
        Play(currentSongName);
    }

    void Update() {
        // if the current backgorund music has stopped playing, chose a new one to play
        if (!currentSong.source.isPlaying) {
            currentSongName = backgroundMusicTrackNames[UnityEngine.Random.Range(0, backgroundMusicTrackNames.Length)];
            currentSong = Array.Find(sounds, sound => sound.name == currentSongName);
            Play(currentSongName);
        }
    }

    // function that can be called to Play() one of the sounds in the audio manager
    public void Play (string name) {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        //print("Playing: " + name + " Volume: " + s.volume);
        s.source.PlayOneShot(s.clip, s.volume);
    }

    // custom type, Sound
    [System.Serializable]
    public class Sound {
        public string name;
        public AudioClip clip;
        public AudioSource source;
        [Range(0f, 1f)] public float volume;
    }

}
