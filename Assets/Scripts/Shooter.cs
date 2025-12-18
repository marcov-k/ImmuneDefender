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
                float minAngle = transform.rotation.eulerAngles.z - spreadAngle;
                float maxAngle = transform.rotation.eulerAngles.z + spreadAngle;
                float angleDiff = spreadAngle * 2.0f;
                float angleStep = angleDiff / (shotCount + 1.0f);

                for (int i = 0; i < shotCount; i++)
                {
                    float angle = minAngle + angleDiff * (i + 1);
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

    public void OnFire()
    {
        if (active) Shoot();
    }
}
