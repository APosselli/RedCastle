using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound2
{
    public string name;
    public AudioClip clip;

    public AudioMixerGroup output;
    
    public bool mute = false;
    public bool bypassEffects = false;
    public bool bypassListenerEffects = false;
    public bool bypassReverbZones = false;
    public bool playOnAwake = false;
    public bool loop = false;

    [Range(0, 256)]
    public int priority = 128;
    [Range(0f, 1f)]
    public float volume = 1f;
    [Range(-3f, 3f)]
    public float pitch = 1f;
    [Range(-1f, 1f)]
    public float stereoPan = 0f;
    [Range(0f, 1f)]
    public float spatialBlend = 0.5f;
    [Range(0f, 1.1f)]
    public float reverbZoneMix = 1f;

    [Range(0f, 0.5f)]
    public float randomVolume = 0.1f;
    [Range(0f, 0.5f)]
    public float randomPitch = 0.1f;

    private AudioSource source;

    public void SetSource(AudioSource _source)
    {
        source = _source;
        source.clip = clip;
        source.outputAudioMixerGroup = output;
        source.mute = mute;
        source.bypassEffects = bypassEffects;
        source.bypassListenerEffects = bypassListenerEffects;
        source.bypassReverbZones = bypassReverbZones;
        source.playOnAwake = playOnAwake;
        source.loop = loop;
        source.priority = priority;
        source.spatialBlend = spatialBlend;
        source.reverbZoneMix = reverbZoneMix;
    }

    public void Play(bool layered = false, bool stopCurrent = false, float volumeModifier = 1f)
    {
        source.volume = (volume * (1 + Random.Range(-randomVolume / 2f, randomVolume / 2f)) * volumeModifier);
        source.pitch = pitch * (1 + Random.Range(-randomPitch / 2f, randomPitch / 2f));
        source.panStereo = stereoPan;
        if (layered == false) {
            if (source.isPlaying) {
                if (stopCurrent) {
                    source.Stop();
                    source.Play();
                }
            } else {
                source.Play();
            }
        } else {
            source.Play();
        }
    }

    public void Stop()
    {
        source.Stop();
    }
}

public class AudioManager2 : MonoBehaviour {

    //public static AudioManager instance;

    [SerializeField]
    Sound2[] sounds;

    //void Awake()
    //{
        //if (instance != null)
        //{
            //Debug.LogError("More than one AudioManager in the scene.");
            //if (instance != this)
            //{
                //Destroy(this.gameObject);
                //this.gameObject.SetActive(false);
            //}
        //}
        //else
        //{
            //instance = this;
            //DontDestroyOnLoad(this);
        //}
    //}

    void Start()
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            GameObject _go = new GameObject("Sound_" + i + "_" + sounds[i].name);
            _go.transform.SetParent(this.transform);
            _go.transform.localPosition = new Vector3(0, 0, 0);
            sounds[i].SetSource(_go.AddComponent<AudioSource>());
        }

        //PlaySound("Music");

    }

    public void PlaySound(string _name, bool layered = false, bool stopCurrent = false, float volume = 1f)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                sounds[i].Play(layered, stopCurrent, volume);
                return;
            }
        }

        // No sound with _name
        Debug.LogWarning("AudioManager: Sound not found in list, " + _name);
    }

    public void StopSound(string _name)
    {
        for (int i = 0; i < sounds.Length; i++)
        {
            if (sounds[i].name == _name)
            {
                sounds[i].Stop();
                return;
            }
        }

        // No sound with _name
        Debug.LogWarning("AUdioManager: Sound not found in list, " + _name);
    }

}
