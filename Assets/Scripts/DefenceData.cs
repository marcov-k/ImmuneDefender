using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "DefenceData", menuName = "Scriptable Objects/DefenceData")]
public class DefenceData : ScriptableObject
{
    public string defenceName;
    public string description;
    public float damage;
    public float speed;
    public bool piercing;
    public bool area;
    public float areaDamageInterval;
    public float range;
    public float cooldown;
    public int shotCount;
    public float spread;
    public Sprite icon;
    public List<string> strengths;
    public GameObject prefab;
}
