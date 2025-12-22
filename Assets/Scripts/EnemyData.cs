using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/EnemyData")]
public class EnemyData : ScriptableObject
{
    public string invaderName;
    public string description;
    public string type;
    public float damage;
    public float speed;
    public float health;
    public float score;
    public string moveLogicName;
    public bool boss;
    public Sprite icon;
}
