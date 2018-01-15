using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoSingleton<MusicManager>
{
    private AudioClip mainTheme;
    private AudioClip menuTheme;

    string sceneName;

    protected override void OnStart()
    {
        base.OnStart();
        mainTheme = ResourceManager.Instance.LoadResource<AudioClip>("Audios/Music/Main theme - Thiago Adamo");
        menuTheme = ResourceManager.Instance.LoadResource<AudioClip>("Audios/Music/Menu theme - Thiago Adamo"); 
        OnLevelWasLoaded(0);
    }

    void OnLevelWasLoaded(int sceneIndex)
    {
        string newSceneName = SceneManager.GetActiveScene().name;
        if(newSceneName != sceneName)
        {
            sceneName = newSceneName;
            Invoke("PlayMusic", .2f);
        }
    }

    void PlayMusic()
    {
        AudioClip clipToPlay = null;

        if(sceneName == "Menu" || sceneName == "Basement")
        {
            clipToPlay = menuTheme;
        }
        else if(sceneName == "Game3")
        {
            clipToPlay = mainTheme;
        }

        if(clipToPlay != null)
        {
            AudioManager.Instance.PlayMusic(clipToPlay, 2);
            Invoke("PlayMusic", clipToPlay.length);
        }
    }
}
