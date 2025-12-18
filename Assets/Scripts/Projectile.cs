using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float speed;
    [SerializeField] float damage;
    [SerializeField] bool piercing;
    [SerializeField] bool area;
    [SerializeField] float radius;
    [SerializeField] float range;

    void Update()
    {
        transform.position += transform.up * speed * Time.deltaTime;
    }
}
