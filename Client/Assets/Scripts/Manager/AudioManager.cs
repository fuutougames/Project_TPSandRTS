using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBase
{
    public enum AudioChannel { Master, Sfx, Music };

    public float masterVolumePercent { get; private set; }
    public float sfxVolumePercent { get; private set; }
    public float musicVolumePercent { get; private set; }

    private AudioSource sfx2DSource;
    private AudioSource[] musicSources;
    private int activeMusicSourceIndex;
    private SoundLibrary library;

    private Transform audioListener;
    private Transform playerT;

    public static AudioManager Instance;

    protected override void OnAwake()
    {
        base.OnAwake();
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            musicSources = new AudioSource[2];
            for (int i = 0; i < 2; i++)
            {
                GameObject newSources = new GameObject("Music Source " + (i + 1));
                musicSources[i] = newSources.AddComponent<AudioSource>();
                newSources.transform.SetParent(this.transform);
            }

            GameObject newSfx2DSources = new GameObject("2D sfx Source ");
            sfx2DSource = newSfx2DSources.AddComponent<AudioSource>();
            newSfx2DSources.transform.SetParent(this.transform);

            audioListener = FindObjectOfType<AudioListener>().transform;

            if (FindObjectOfType<Player>() != null)
            {
                playerT = FindObjectOfType<Player>().transform;
            }
            library = this.GetComponent<SoundLibrary>();
        }
    }

    protected override void OnStart()
    {
        base.OnStart();

        masterVolumePercent = PlayerPrefs.GetFloat("master vol", 1);
        sfxVolumePercent = PlayerPrefs.GetFloat("sfx vol", 1);
        musicVolumePercent = PlayerPrefs.GetFloat("music vol", 1);
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if(playerT != null)
        {
            audioListener.position = playerT.position;
        }
    }

    public void SetVolume(float volumePercent, AudioChannel channel)
    {
        switch(channel)
        {
            case AudioChannel.Master:
                masterVolumePercent = volumePercent;
                break;
            case AudioChannel.Sfx:
                sfxVolumePercent = volumePercent;
                break;
            case AudioChannel.Music:
                musicVolumePercent = volumePercent;
                break;
        }

        musicSources[0].volume = musicVolumePercent * masterVolumePercent;
        musicSources[1].volume = musicVolumePercent * masterVolumePercent;

        PlayerPrefs.SetFloat("master vol", masterVolumePercent);
        PlayerPrefs.SetFloat("sfx vol", sfxVolumePercent);
        PlayerPrefs.SetFloat("music vol", musicVolumePercent);
        PlayerPrefs.Save();
    }

    public void PlayMusic(AudioClip clip, float fadeDuration = 1)
    {
        activeMusicSourceIndex = 1 - activeMusicSourceIndex;
        musicSources[activeMusicSourceIndex].clip = clip;
        musicSources[activeMusicSourceIndex].Play();

        StartCoroutine(AnimatedMusicCrossfade(fadeDuration));
    }

    public void PlaySound(AudioClip clip, Vector3 pos)
    {
        if (clip == null) return;
        if (FindObjectOfType<Player>() != null)
        {
            playerT = FindObjectOfType<Player>().transform;
        }
        AudioSource.PlayClipAtPoint(clip, pos, sfxVolumePercent * masterVolumePercent);
        //AudioSource.PlayClipAtPoint(clip, pos, 1);
    }

    public void PlaySound(string soundName, Vector3 pos)
    {
        PlaySound(library.GetClipFromeName(soundName), pos);
    }

    public void PlaySound2D(string soundName)
    {
        sfx2DSource.PlayOneShot(library.GetClipFromeName(soundName), sfxVolumePercent * masterVolumePercent);
    }

    IEnumerator AnimatedMusicCrossfade(float duration)
    {
        float percent = 0;

        while(percent < 1)
        {
            percent += Time.deltaTime * 1 / duration;
            musicSources[activeMusicSourceIndex].volume = Mathf.Lerp(0, musicVolumePercent * masterVolumePercent, percent);
            musicSources[1 - activeMusicSourceIndex].volume = Mathf.Lerp(musicVolumePercent * masterVolumePercent, 0, percent);
            yield return null;
        }
    }
}
