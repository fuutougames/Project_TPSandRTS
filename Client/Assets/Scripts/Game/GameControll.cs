using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameControll : MonoBase
{
    public Spawner spawner;
    public MapGenerator mapGenerator;
    public CamFollow camFollow;

    protected override void OnStart()
    {
        base.OnStart();
        mapGenerator.GenerateRandomMap();
        //mapGenerator.GenerateMap();       
        spawner.OnWaveEnd += Spawner_OnWaveEnd;
        StartCoroutine(DelayStart());
    }

    private void Spawner_OnWaveEnd()
    {
        StartCoroutine(DelayMove());
    }

    IEnumerator DelayStart()
    {
        yield return new WaitForSeconds(1);
        spawner.NextWave();
        camFollow.following = true;
    }

    IEnumerator DelayMove()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("Basement");
    }
}
