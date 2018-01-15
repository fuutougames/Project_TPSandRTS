using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class MapGenerator : MonoBase
{
    public Map[] maps;
    public int mapIndex;

    private Transform tilePrefab;
    public Transform obstaclePrefab;
    public Transform mapFloor;
    public Transform navmeshFloor;
    public Transform navemeshMaskPrefab;
    public Vector2 maxMapSize;
   
    [Range(0, 1)]
    public float outlinePercent;

    public float tileSize;

    public int randomSeed = 0;

    private List<Coord> allTileCoords;
    private List<Coord> allOpenCoords;
    private Queue<Coord> shuffledTileCoords;
    private Queue<Coord> shuffledOpenTileCoords;
    private Transform[,] tileMap;

    private Map currentMap;
    protected override void OnAwake()
    {
        base.OnAwake();
        tilePrefab = ResourceManager.Instance.LoadResource<Transform>("Prefabs/Tile");
        obstaclePrefab = ResourceManager.Instance.LoadResource<Transform>("Prefabs/Obstacle");
        //FindObjectOfType<Spawner>().OnNewWave += OnNewWave;
        //GenerateMap();
    }

    protected override void OnStart()
    {
        base.OnStart();
    }

    #region Public
    private Vector3 tilePosition;
    public void GenerateMap()
    {
        currentMap = maps[mapIndex];
        tileMap = new Transform[currentMap.mapSize.x, currentMap.mapSize.y];
        System.Random prng = new System.Random(currentMap.seed);

        // Generating coords
        allTileCoords = new List<Coord>();
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                allTileCoords.Add(new Coord(x, y));
            }
        }
        shuffledTileCoords = new Queue<Coord>(Utility.ShuffleArray(allTileCoords.ToArray(), currentMap.seed));

        // Create map holder object
        string holderName = "Generated Map";

        if (transform.Find(holderName))
        {
            DestroyImmediate(transform.Find(holderName).gameObject);
        }

        Transform mapHolder = new GameObject(holderName).transform;
        mapHolder.SetParent(this.transform);

        // Spawning tiles
        MapTile mapTile = this.transform.Find("MapTile").GetComponent<MapTile>();
        mapTile.BuildMesh();
        mapTile.transform.position = CoordToPosition(0, currentMap.mapSize.y - 1) - new Vector3(tileSize * 0.5f, 0, -tileSize * 0.5f);
        for (int x = 0; x < currentMap.mapSize.x; x++)
        {
            for (int y = 0; y < currentMap.mapSize.y; y++)
            {
                //tilePosition = CoordToPosition(x, y);
                //Transform newTile = Instantiate(tilePrefab, tilePosition, Quaternion.Euler(Vector3.right * 90)) as Transform;
                //newTile.localScale = Vector3.one * (1 - outlinePercent) * tileSize;
                //newTile.SetParent(mapHolder);
                //tileMap[x, y] = newTile;
            }
        }

        // Spawing obstacles
        bool[,] obstacleMap = new bool[(int)currentMap.mapSize.x, (int)currentMap.mapSize.y];

        int obstacleCount = (int)(currentMap.obstaclePercent * currentMap.mapSize.x * currentMap.mapSize.y);
        int currentObstacleCount = 0;
        allOpenCoords = new List<Coord>(allTileCoords);

        for (int i = 0; i < obstacleCount; i++)
        {
            Coord randomCoord = GetRandomCoord();
            obstacleMap[randomCoord.x, randomCoord.y] = true;
            currentObstacleCount++;
            if (randomCoord != currentMap.mapCentre && MapIsFullyAccessible(obstacleMap, currentObstacleCount))
            {
                float obstacleHeight = Mathf.Lerp(currentMap.minObstacleHeight, currentMap.maxObstacleHeight, (float)prng.NextDouble());
                Vector3 obstaclePosition = CoordToPosition(randomCoord.x, randomCoord.y);
                Transform newObstacle = Instantiate(obstaclePrefab, obstaclePosition + Vector3.up * obstacleHeight / 2, Quaternion.identity) as Transform;
                newObstacle.SetParent(mapHolder);
                newObstacle.localScale = new Vector3((1 - outlinePercent) * tileSize, obstacleHeight, (1 - outlinePercent) * tileSize);

                Renderer obstacleRenderer = newObstacle.GetComponent<Renderer>();
                Material obstacleMaterial = new Material(obstacleRenderer.sharedMaterial);
                float colorPercent = randomCoord.y / (float)currentMap.mapSize.y;
                obstacleMaterial.color = Color.Lerp(currentMap.foregroundColor, currentMap.backgroundColor, colorPercent);
                obstacleRenderer.sharedMaterial = obstacleMaterial;

                allOpenCoords.Remove(randomCoord);
            }
            else
            {
                obstacleMap[randomCoord.x, randomCoord.y] = false;
                currentObstacleCount--;
            }
        }

        shuffledOpenTileCoords = new Queue<Coord>(Utility.ShuffleArray(allOpenCoords.ToArray(), currentMap.seed));

        // Creating navemesh mask
        Transform maskLeft = Instantiate(navemeshMaskPrefab, Vector3.left * (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity) as Transform;
        maskLeft.SetParent(mapHolder);
        maskLeft.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

        Transform maskRight = Instantiate(navemeshMaskPrefab, Vector3.right * (currentMap.mapSize.x + maxMapSize.x) / 4f * tileSize, Quaternion.identity) as Transform;
        maskRight.SetParent(mapHolder);
        maskRight.localScale = new Vector3((maxMapSize.x - currentMap.mapSize.x) / 2f, 1, currentMap.mapSize.y) * tileSize;

        Transform maskTop = Instantiate(navemeshMaskPrefab, Vector3.forward * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
        maskTop.SetParent(mapHolder);
        maskTop.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;

        Transform maskBottom = Instantiate(navemeshMaskPrefab, Vector3.back * (currentMap.mapSize.y + maxMapSize.y) / 4f * tileSize, Quaternion.identity) as Transform;
        maskBottom.SetParent(mapHolder);
        maskBottom.localScale = new Vector3(maxMapSize.x, 1, (maxMapSize.y - currentMap.mapSize.y) / 2f) * tileSize;

        navmeshFloor.localScale = new Vector3(maxMapSize.x, maxMapSize.y) * tileSize;
        mapFloor.localScale = new Vector3(currentMap.mapSize.x * tileSize, currentMap.mapSize.y * tileSize);
    }

    public Vector3 GetCenterPosition()
    {
        return CoordToPosition(currentMap.mapCentre.x, currentMap.mapCentre.y);
    }

    public Transform GetTileFromPosition(Vector3 position)
    {
        int x = Mathf.RoundToInt(position.x / tileSize + (currentMap.mapSize.x - 1) / 2f);
        int y = Mathf.RoundToInt(position.z / tileSize + (currentMap.mapSize.y - 1) / 2f);

        x = Mathf.Clamp(x, 0, tileMap.GetLength(0) - 1);
        y = Mathf.Clamp(y, 0, tileMap.GetLength(1) - 1);

        return tileMap[x, y];
    }

    public Vector3 GetRegionPosFromPosition(Vector3 position, Vector2 regionSize)
    {
        Vector2 regionIndex = CalRegionIndexFromPos(position, regionSize);
        float regionRealSizeX = regionSize.x * tileSize;
        float regionRealSizeY = regionSize.y * tileSize;

        return new Vector3(regionIndex.x * regionRealSizeX, 0.2f, regionIndex.y * regionRealSizeY) 
            - new Vector3(currentMap.mapSize.x * tileSize / 2, 0, currentMap.mapSize.y * tileSize / 2) 
            + new Vector3(regionRealSizeX / 2, 0, regionRealSizeY / 2) ;
    }

    public Coord GetRandomCoord()
    {
        Coord randomCoord = shuffledTileCoords.Dequeue();
        shuffledTileCoords.Enqueue(randomCoord);
        return randomCoord;
    }

    public Transform GetRandomOpenTile()
    {
        Coord randomCoord = shuffledOpenTileCoords.Dequeue();
        shuffledOpenTileCoords.Enqueue(randomCoord);
        return tileMap[randomCoord.x, randomCoord.y];
    }

    public Vector3 GetRandomOpenPos()
    {
        Coord randomCoord = shuffledOpenTileCoords.Dequeue();
        shuffledOpenTileCoords.Enqueue(randomCoord);
        return CoordToPosition(randomCoord.x, randomCoord.y);
    }

    private Dictionary<Vector2, List<Coord>> OpenTileCoordDic = new Dictionary<Vector2, List<Coord>>();
    public Transform GetRandomOpenTileFromRegion(Vector2 region, Vector2 regionSize)
    {
        OpenTileCoordDic.Clear();
        Vector2 mapRegions = CalMapRegions(regionSize);
        for(int i = 0; i < allOpenCoords.Count; i++)
        {
            Coord openCoord = allOpenCoords[i];
            Vector3 coordPos = CoordToPosition(openCoord.x, openCoord.y);
            Vector2 regionIndex = CalRegionIndexFromPos(coordPos, regionSize);
            if(OpenTileCoordDic.ContainsKey(regionIndex))
            {
                OpenTileCoordDic[regionIndex].Add(openCoord);
            }
            else
            {
                OpenTileCoordDic.Add(regionIndex, new List<Coord>() { openCoord });
            }
        }

        if(OpenTileCoordDic.ContainsKey(region))
        {
            Queue<Coord> randOpenCoords = new Queue<Coord>(Utility.ShuffleArray(OpenTileCoordDic[region].ToArray(), UnityEngine.Random.Range(0, 1000)));
            Coord openCoord = randOpenCoords.Dequeue();
            Vector3 coordPos = CoordToPosition(openCoord.x, openCoord.y);
            return GetTileFromPosition(coordPos);
        }
        else
        {
            return null;
        }
    }

    private List<Vector3> tempRandPosList = new List<Vector3>();
    public Vector3 GetRandomOpenPosFromeRegion(Vector3 centerPos, float radius)
    {
        tempRandPosList.Clear();
        for (int i = 0; i < allOpenCoords.Count; i++)
        {
            Coord openCoord = allOpenCoords[i];
            Vector3 coordPos = CoordToPosition(openCoord.x, openCoord.y);
            float distance = Vector3.Distance(centerPos, coordPos);
            if(distance > 5 && distance <= radius)
            {
                tempRandPosList.Add(coordPos);
            }
        }

        return tempRandPosList[UnityEngine.Random.Range(0, tempRandPosList.Count)];
    }

    public Vector2 CalRegionIndexFromPos(Vector3 position, Vector2 regionSize)
    {
        Transform tile = GetTileFromPosition(position);
        if (tile == null) return Vector2.zero;

        Vector2 regionIndex = Vector2.zero;
        float regionRealSizeX = regionSize.x * tileSize;
        float regionRealSizeY = regionSize.y * tileSize;
        Vector2 mapRegions = CalMapRegions(regionSize);
        regionIndex.x = Mathf.RoundToInt(tile.position.x / regionRealSizeX + (mapRegions.x - 1) / 2f);
        regionIndex.y = Mathf.RoundToInt(tile.position.z / regionRealSizeY + (mapRegions.y - 1) / 2f);
        return regionIndex;
    }

    public void GenerateRandomMap()
    {
        randomSeed = UnityEngine.Random.Range(0, 100);
        maps[0] = Map.CreateRandom(randomSeed, new Coord() { x = (int)maxMapSize.x, y = (int)maxMapSize.y});
        mapIndex = 0;
        GenerateMap();
        if(currentMap == null)
        {
            Debug.Log("Currentmap = null");
            return;
        }
        Camera.main.GetComponent<GradientBackground>().UpdateBackground(currentMap.foregroundColor, currentMap.backgroundColor);
    }
    #endregion

    #region Private

    private void OnNewWave(int waveNumber)
    {
        mapIndex = waveNumber - 1;
        GenerateMap();
    }

    private bool MapIsFullyAccessible(bool[,] obstacleMap, int currentObstacleCount)
    {
        int obstacleMapLengthX = obstacleMap.GetLength(0);
        int obstacleMapLengthY = obstacleMap.GetLength(1);
        bool[,] mapFlags = new bool[obstacleMapLengthX, obstacleMapLengthY];
        Queue<Coord> queue = new Queue<Coord>();
        queue.Enqueue(currentMap.mapCentre);
        mapFlags[currentMap.mapCentre.x, currentMap.mapCentre.y] = true;

        int accessibleTileCount = 1;

        while (queue.Count > 0)
        {
            Coord tile = queue.Dequeue();
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    int neighbourX = tile.x + x;
                    int neighbourY = tile.y + y;
                    if (x == 0 || y == 0)
                    {
                        if (neighbourX >= 0 && neighbourX < obstacleMapLengthX && neighbourY >= 0 && neighbourY < obstacleMapLengthY)
                        {
                            if (!mapFlags[neighbourX, neighbourY] && !obstacleMap[neighbourX, neighbourY])
                            {
                                mapFlags[neighbourX, neighbourY] = true;
                                queue.Enqueue(new Coord(neighbourX, neighbourY));
                                accessibleTileCount++;
                            }
                        }
                    }
                }
            }
        }

        int targetAccessibleTileCount = (int)(currentMap.mapSize.x * currentMap.mapSize.y - currentObstacleCount);
        return targetAccessibleTileCount == accessibleTileCount;
    }

    private Vector3 CoordToPosition(int x, int y)
    {
        return new Vector3(-currentMap.mapSize.x / 2f + 0.5f + x, 0, -currentMap.mapSize.y / 2f + 0.5f + y) * tileSize;
    }

    private Vector2 CalMapRegions(Vector2 regionSize)
    {
        Vector2 mapRegions = Vector2.zero;
        mapRegions.x = currentMap.mapSize.x / regionSize.x;
        mapRegions.y = currentMap.mapSize.y / regionSize.y;
        return mapRegions;
    }
    #endregion

    [System.Serializable]
    public struct Coord
    {
        public int x;
        public int y;

        public Coord(int _x, int _y)
        {
            x = _x;
            y = _y;
        }

        public static bool operator ==(Coord c1, Coord c2)
        {
            return c1.x == c2.x && c1.y == c2.y;
        }

        public static bool operator !=(Coord c1, Coord c2)
        {
            return !(c1.x == c2.x && c1.y == c2.y);
        }
    }

    [System.Serializable]
    public class Map
    {
        public Coord mapSize;
        public int seed;
        [Range(0, 1)]
        public float obstaclePercent;
        public float minObstacleHeight;
        public float maxObstacleHeight;
        public Color foregroundColor;
        public Color backgroundColor;

        public Coord mapCentre
        {
            get
            {
                return new Coord(mapSize.x / 2, mapSize.y / 2);
            }
        }

        public static Map CreateRandom(int seed, Coord mapSize)
        {
            Map randMap = new Map();

            randMap.seed = seed;
            randMap.mapSize = mapSize;
            System.Random prng = new System.Random(randMap.seed);
            randMap.obstaclePercent = (float)(prng.NextDouble() * (0.4f - 0.2f) + 0.2f);
            randMap.minObstacleHeight = 3f;
            randMap.maxObstacleHeight = 5;
            UnityEngine.Random.InitState(seed);
            randMap.foregroundColor = UnityEngine.Random.ColorHSV(0.5f, 1f);
            randMap.backgroundColor = UnityEngine.Random.ColorHSV(0f, 0.5f);
            return randMap;
        }
    }
}
