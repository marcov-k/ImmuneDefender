using UnityEngine;
using System;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
public class LevelData : ScriptableObject
{
    public EnemySpawnData[] enemies;
    public int defencesUnlocked;
    public DefenceData[] newDefences;
    public EnemyData[] newEnemies;

    [Serializable]
    public struct EnemySpawnData
    {
        public GameObject enemy;
        public float spawnDelay; // delay after previous enemy spawns
    }
}
