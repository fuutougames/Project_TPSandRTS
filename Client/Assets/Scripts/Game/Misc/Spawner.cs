﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The enemy spawner
/// </summary>
public class Spawner : MonoBase
{
    public bool devMode;

    public Wave[] waves;
    private Enemy enemyPrefab;

    private LivingEntity playerEntity;
    private Transform playerT;

    private Wave currentWave;
    private int currentWaveNumber;
    private int enemiesRemainingToSpawn;
    private int enemiesRemainingAlive;
    private float nextSpawnTime;

    private MapGenerator map;

    private float timeBetweenCampingChecks = 2;
    private float campThresholdDistance = 1.5f;
    private float nextCampCheckTime;
    private Vector3 campPositionOld;
    bool isCamping;
    bool isDisabled;

    public event System.Action<int> OnNewWave;
    public event System.Action OnWaveEnd;

    protected override void OnAwake()
    {
        base.OnAwake();
        enemyPrefab = ResourceManager.Instance.LoadResource<Enemy>("Prefabs/Enemy");
        map = FindObjectOfType<MapGenerator>();
        playerEntity = FindObjectOfType<Player>();
        playerEntity.OnDeath += OnPlayerDeath;
        playerT = playerEntity.transform;
        //playerT.gameObject.SetActive(false);
        nextCampCheckTime = timeBetweenCampingChecks + Time.time;
        campPositionOld = playerT.position;
    }


    protected override void OnStart()
    {
        base.OnStart();
        //NextWave();
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();
        if (isDisabled) return;
        if (currentWave == null) return;

        if(Time.time > nextCampCheckTime)
        {
            nextCampCheckTime = Time.time + timeBetweenCampingChecks;

            isCamping = (Vector3.Distance(playerT.position, campPositionOld)) < campThresholdDistance;
            campPositionOld = playerT.position;
        }

        if((enemiesRemainingToSpawn > 0 || currentWave.infinite) && Time.time > nextSpawnTime)
        {
            enemiesRemainingToSpawn--;
            nextSpawnTime = Time.time + currentWave.timeBetweenSpawns;

            StartCoroutine(SpawnEnemy());
        }

        if(devMode)
        {
            if(Input.GetKeyDown(KeyCode.Return))
            {
                //StopCoroutine("SpawnEnemy");
                StopAllCoroutines();
                foreach(Enemy enemy in FindObjectsOfType<Enemy>())
                {
                    GameObject.Destroy(enemy.gameObject);
                }
                NextWave();
            }
        }
    }

    IEnumerator SpawnEnemy()
    {
        float spawnDelay = 1;
        float tileFlashSpeed = 4;
        //Transform spawnTile = map.GetRandomOpenTile();
        //if(isCamping)
        //{
        //    spawnTile = map.GetTileFromPosition(playerT.position);
        //}
        //Material tileMat = spawnTile.GetComponent<Renderer>().material;
        //Color initialColor = Color.white;
        //Color flashColor = Color.red;
        float spawnTimer = 0;

        while(spawnTimer < spawnDelay)
        {
            //tileMat.color = Color.Lerp(initialColor, flashColor, Mathf.PingPong(spawnTimer * tileFlashSpeed, 1));
            spawnTimer += Time.deltaTime;
            yield return null;
        }

        Vector3 spawnPos = map.GetRandomOpenPos();
        //Enemy spawnedEnemey = Instantiate(enemyPrefab, spawnTile.position + Vector3.up, Quaternion.identity) as Enemy;
        Enemy spawnedEnemey = Instantiate(enemyPrefab, spawnPos + Vector3.up, Quaternion.identity) as Enemy;
        spawnedEnemey.OnDeath += OnEnemyDeath;
        spawnedEnemey.SetCharacteristics(currentWave.moveSpeed, 
                                         currentWave.hitsToKillPlayer, 
                                         currentWave.enemyHealth, 
                                         currentWave.skinColor);

        Vector3 partolTragetPos = map.GetRandomOpenPosFromeRegion(spawnPos, 10);
        spawnedEnemey.SetPartolPath(partolTragetPos + Vector3.up);
    }

    private void OnPlayerDeath()
    {
        isDisabled = true;
    }

    private void OnEnemyDeath()
    {
        enemiesRemainingAlive--;
        if(enemiesRemainingAlive == 0)
        {
            //NextWave();
            if (OnWaveEnd != null)
            {
                OnWaveEnd();
            }
        }
    }

    void ResetPlayerPosition()
    {
        playerT.position = map.GetCenterPosition() + Vector3.up * 3; //map.GetTileFromPosition(Vector3.zero).position + Vector3.up * 3;
    }

    public void NextWave()
    {
        if(currentWaveNumber > 0)
        {
            AudioManager.Instance.PlaySound2D("LevelComplete");
        }
        currentWaveNumber++;
        if(currentWaveNumber - 1 < waves.Length)
        {
            currentWave = waves[currentWaveNumber - 1];
            enemiesRemainingToSpawn = currentWave.enemyCount;
            enemiesRemainingAlive = enemiesRemainingToSpawn;

            if(OnNewWave != null)
            {
                OnNewWave(currentWaveNumber);
            }

            ResetPlayerPosition();
        }      
    }

    [System.Serializable]
    public class Wave
    {
        public bool infinite;
        public int enemyCount;
        public float timeBetweenSpawns;

        public float moveSpeed;
        public int hitsToKillPlayer;
        public float enemyHealth;
        public Color skinColor;
    }
}
