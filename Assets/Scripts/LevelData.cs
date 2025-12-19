using UnityEngine;
using System;

[CreateAssetMenu(fileName = "LevelData", menuName = "Scriptable Objects/LevelData")]
public class LevelData : ScriptableObject
{
    public EnemyData[] enemies;
    public int defencesUnlocked;

    [Serializable]
    public struct EnemyData
    {
        public GameObject enemy;
        public float spawnDelay; // delay after previous enemy spawns
    }
}
