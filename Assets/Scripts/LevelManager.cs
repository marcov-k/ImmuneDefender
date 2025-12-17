using UnityEngine;
using System.Collections;
using Unity.Mathematics;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    [SerializeField] LevelData levelData;
    [SerializeField] int2 gridDims = new(10, 10);
    [SerializeField] float3 padding = new(20, 20, 100); // left/right, top, bottom
    [SerializeField] GameObject posPrefab;
    [SerializeField] EndScreen endScreen;
    public Position[,] positions;
    float totalScore;
    readonly List<Enemy> killedEnemies = new();
    PauseMenu pauseMenu;

    void Start()
    {
        InitValues();
        CreatePositions();
        StartCoroutine(SpawnCoroutine());
    }

    void InitValues()
    {
        foreach (var enemy in levelData.enemies)
        {
            totalScore += enemy.enemy.GetComponent<Enemy>().score;
        }
        pauseMenu = FindFirstObjectByType<PauseMenu>();
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
    }

    IEnumerator SpawnCoroutine()
    {
        for (int i = 0; i < levelData.enemies.Length; i++)
        {
            yield return new WaitForSeconds(levelData.enemies[i].spawnDelay);
            while (positions[0, 0].filled != 0 || PauseMenu.paused)
            {
                yield return new WaitForEndOfFrame();
            }
            var enemy = Instantiate(levelData.enemies[i].enemy, positions[0, 0].positionTrans.position, Quaternion.identity);
            var enemyScript = enemy.GetComponent<Enemy>();
            enemyScript.pos = new(0, 0);
            enemyScript.manager = this;
            positions[0, 0].filled = 1;
        }
    }

    void ShowEndScreen(bool won)
    {
        pauseMenu.PermPause(true);
        float score = 0.0f;
        foreach (var enemy in killedEnemies)
        {
            score += enemy.score;
        }
        float relScore = score / totalScore;
        int stars = Mathf.RoundToInt(relScore * 3.0f);
        endScreen.ShowResults(won, stars, score, totalScore);
    }

    public void PlayerKilled()
    {
        ShowEndScreen(false);
    }

    public void EnemyKilled(Enemy enemy)
    {
        killedEnemies.Add(enemy);
    }

    public void BossKilled()
    {

    }

    public void Continue()
    {
        pauseMenu.PermPause(false);
        SceneManager.LoadScene("LevelSelect");
    }

    public struct Position
    {
        public Transform positionTrans;
        public int filled;
    }
}
