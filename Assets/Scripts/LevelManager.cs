using UnityEngine;
using System.Collections;
using Unity.Mathematics;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using static PlayerData;

public class LevelManager : MonoBehaviour
{
    [SerializeField] LevelData levelData;
    [SerializeField] int2 gridDims = new(10, 10);
    [SerializeField] float3 padding = new(20, 20, 100); // left/right, top, bottom
    [SerializeField] GameObject posPrefab;
    [SerializeField] EndScreen endScreen;
    [SerializeField] float backgroundSpeed;
    [SerializeField] GameObject[] backgrounds;
    int followBackground = 1;
    float2 screenEdges; // x = bottom, y = top
    float backgroundHeight;
    public Position[,] positions;
    public Position[] spawnPositions;
    float totalScore;
    readonly List<float> killedEnemyScores = new();
    PauseMenu pauseMenu;
    System.Random random;

    void Start()
    {
        InitValues();
        CreatePositions();
        StartCoroutine(SpawnCoroutine());
    }

    void Update()
    {
        ScrollBackground();
    }

    void InitValues()
    {
        foreach (var enemy in levelData.enemies)
        {
            totalScore += enemy.enemy.GetComponent<Enemy>().score;
        }
        pauseMenu = FindFirstObjectByType<PauseMenu>();

        float screenBottom = Camera.main.ScreenToWorldPoint(new(0, 0)).y;
        float screenTop = Camera.main.ScreenToWorldPoint(new(0, Screen.height)).y;
        screenEdges = new(screenBottom, screenTop);
        backgroundHeight = backgrounds[0].GetComponent<SpriteRenderer>().bounds.extents.y;
        backgrounds[1].transform.position = new(backgrounds[1].transform.position.x, backgrounds[0].transform.position.y + 2.0f * backgroundHeight);
        random = new();
    }

    void CreatePositions()
    {
        padding = new(padding.x * Screen.width, padding.y * Screen.height, padding.z * Screen.height);
        positions = new Position[gridDims.y, gridDims.x];
        var width = Screen.width;
        var height = Screen.height;
        float4 edges = new(padding.x, height - padding.y, padding.z, width - padding.x); // left, top, bottom, right
        float xStep = (edges.w - edges.x) / (gridDims.x + 1);
        float yStep = (edges.y - edges.z) / (gridDims.y + 1);
        for (int i = 0; i < gridDims.y; i++)
        {
            for (int j = 0; j < gridDims.x; j++)
            {
                Vector2 pos = Camera.main.ScreenToWorldPoint(new(edges.x + (j + 1) * xStep, edges.y - (i + 1) * yStep));
                var newPos = Instantiate(posPrefab, pos, Quaternion.identity);
                positions[i, j].positionTrans = newPos.transform;
            }
        }

        float top = Camera.main.ScreenToWorldPoint(new(0, Screen.height)).y + 3.0f;
        spawnPositions = new Position[positions.GetLength(1)];
        for (int i = 0; i < positions.GetLength(1); i++)
        {
            Vector2 pos = new(positions[0, i].positionTrans.position.x, top);
            var newPos = Instantiate(posPrefab, pos, Quaternion.identity);
            spawnPositions[i].positionTrans = newPos.transform;
        }
    }

    IEnumerator SpawnCoroutine()
    {
        for (int i = 0; i < levelData.enemies.Length; i++)
        {
            yield return new WaitForSeconds(levelData.enemies[i].spawnDelay);
            int index = FindSpawn();
            var spawnPos = spawnPositions[index];
            while (spawnPos.filled != 0 || PauseMenu.paused)
            {
                yield return new WaitForEndOfFrame();
            }
            var enemy = Instantiate(levelData.enemies[i].enemy, spawnPos.positionTrans.position, Quaternion.identity);
            var enemyScript = enemy.GetComponent<Enemy>();
            enemyScript.pos = new(-1, index);
            enemyScript.manager = this;
            spawnPositions[index].filled = 1;
        }
    }

    int FindSpawn()
    {
        List<int> options = new();
        for (int i = 0; i < spawnPositions.Length; i++)
        {
            if (spawnPositions[i].filled == 0)
            {
                options.Add(i);
            }
        }
        return random.Next(options.Count);
    }

    void ScrollBackground()
    {
        if (followBackground == 1)
        {
            backgrounds[0].transform.position = new(backgrounds[0].transform.position.x, backgrounds[0].transform.position.y - backgroundSpeed * Time.deltaTime);
            backgrounds[1].transform.position = new(backgrounds[1].transform.position.x, backgrounds[0].transform.position.y + 2.0f * backgroundHeight);
            if (backgrounds[0].transform.position.y + backgroundHeight <= screenEdges.x)
            {
                backgrounds[0].transform.position = new(backgrounds[0].transform.position.x, screenEdges.y + backgroundHeight);
                followBackground = 0;
            }
        }
        else
        {
            backgrounds[1].transform.position = new(backgrounds[0].transform.position.x, backgrounds[1].transform.position.y - backgroundSpeed * Time.deltaTime);
            backgrounds[0].transform.position = new(backgrounds[0].transform.position.x, backgrounds[1].transform.position.y + 2.0f * backgroundHeight);
            if (backgrounds[1].transform.position.y + backgroundHeight <= screenEdges.x)
            {
                backgrounds[1].transform.position = new(backgrounds[1].transform.position.x, screenEdges.y + backgroundHeight);
                followBackground = 1;
            }
        }
    }

    void ShowEndScreen(bool won)
    {
        pauseMenu.PermPause(true);

        if (won && startedLevel > lastLevel)
        {
            lastLevel = startedLevel;
        }
        startedLevel = 0;

        float score = 0.0f;
        foreach (var enemyScore in killedEnemyScores)
        {
            score += enemyScore;
        }
        float relScore = score / totalScore;
        int stars = Mathf.RoundToInt(relScore * 3.0f);
        endScreen.ShowResults(won, stars, score, totalScore);
    }

    public void PlayerKilled()
    {
        ShowEndScreen(false);
    }

    public void EnemyKilled(float enemyScore)
    {
        killedEnemyScores.Add(enemyScore);
        if (killedEnemyScores.Count == levelData.enemies.Length)
        {
            ShowEndScreen(true);
        }
    }

    public void BossKilled(float enemyScore)
    {
        killedEnemyScores.Add(enemyScore);
        ShowEndScreen(true);
    }

    public void Continue()
    {
        pauseMenu.PermPause(false);
        SceneManager.LoadScene("LevelSelect");
    }

    public int GetDefencesUnlocked()
    {
        return levelData.defencesUnlocked;
    }

    [System.Serializable]
    public struct Position
    {
        public Transform positionTrans;
        public int filled;
    }
}
