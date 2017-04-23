using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundLibrary : MonoBase
{
    public List<SoundGroup> soundGroups = new List<SoundGroup>();

    private Dictionary<string, AudioClip[]> groupDictionary = new Dictionary<string, AudioClip[]>();


    protected override void OnAwake()
    {
        base.OnAwake();
        foreach(SoundGroup soundGroup in soundGroups)
        {
            groupDictionary.Add(soundGroup.groupID, soundGroup.group);
        }
    }

    public AudioClip GetClipFromeName(string name)
    {
        if(groupDictionary.ContainsKey(name))
        {
            AudioClip[] sounds = groupDictionary[name];
            return sounds[Random.Range(0, sounds.Length)];
        }
        return null;
    }

    [System.Serializable]
    public class SoundGroup
    {
        public string groupID;
        public AudioClip[] group;
    }
}
