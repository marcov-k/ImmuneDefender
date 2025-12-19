using UnityEngine;
using UnityEngine.InputSystem;

public class Shooter : MonoBehaviour
{
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] int shotCount;
    [SerializeField] float spreadAngle;
    [SerializeField] float cooldown;
    bool onCooldown = false;
    public bool active;

    void Shoot()
    {
        if (!onCooldown)
        {
            if (shotCount == 1)
            {
                Instantiate(projectilePrefab, transform.position, transform.rotation);
            }
            else
            {
                float shotAngle = transform.rotation.eulerAngles.z;
                float minAngle = shotAngle - spreadAngle / 2.0f;
                float maxAngle = shotAngle + spreadAngle / 2.0f;
                float angleStep = spreadAngle / (shotCount + 1.0f);

                for (int i = 0; i < shotCount; i++)
                {
                    float angle = minAngle + angleStep * (i + 1);
                    Instantiate(projectilePrefab, transform.position, Quaternion.Euler(0, 0, angle));
                }
            }
            onCooldown = true;
            Invoke(nameof(ResetCooldown), cooldown);
        }
    }

    void ResetCooldown()
    {
        onCooldown = false;
    }

    public void OnAttack()
    {
        if (active) Shoot();
    }
}
