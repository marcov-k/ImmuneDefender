using UnityEngine;
using System;
using System.Collections;

public class LevelManager : MonoBehaviour
{
    [SerializeField] LevelData levelData;
    public PosWrapper[] positions = new PosWrapper[10] { new() { row = new Position[10] }, new() { row = new Position[10] }, new() { row = new Position[10] },
        new() { row = new Position[10] }, new() { row = new Position[10] }, new() { row = new Position[10] }, new() { row = new Position[10] },
        new() { row = new Position[10] }, new() { row = new Position[10] }, new() { row = new Position[10] } };

    void Start()
    {
        StartCoroutine(SpawnCoroutine());
    }

    IEnumerator SpawnCoroutine()
    {
        for (int i = 0; i < levelData.enemies.Length; i++)
        {
            yield return new WaitForSeconds(levelData.enemies[i].spawnDelay);
            while (positions[0].row[0].filled != 0 || PauseMenu.paused)
            {
                yield return new WaitForEndOfFrame();
            }
            var enemy = Instantiate(levelData.enemies[i].enemy, positions[0].row[0].positionTrans.position, Quaternion.identity);
            var enemyScript = enemy.GetComponent<Enemy>();
            enemyScript.pos = new(0, 0);
        }
    }

    [Serializable]
    public struct Position
    {
        public Transform positionTrans;
        public int filled;
    }

    [Serializable]
    public struct PosWrapper
    {
        public Position[] row;
    }
}
