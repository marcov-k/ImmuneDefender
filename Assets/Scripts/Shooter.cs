using UnityEngine;
using System.Collections.Generic;

public class Shooter : MonoBehaviour
{
    public DefenceData data;
    GameObject projectilePrefab;
    int shotCount;
    float spreadAngle;
    float cooldown;
    bool onCooldown = false;
    public bool active;
    InputActions inputs;

    void Awake()
    {
        InitValues();
    }

    void InitValues()
    {
        inputs = new();
        projectilePrefab = data.prefab;
        shotCount = data.shotCount;
        spreadAngle = data.spread;
        cooldown = data.cooldown;
    }

    void Start()
    {
        InitInputs();
    }

    void InitInputs()
    {
        inputs.Player.Attack.performed += ctx => OnAttack();
    }

    void OnEnable()
    {
        inputs.Enable();
    }

    void OnDisable()
    {
        inputs.Disable();
    }

    void Shoot()
    {
        if (!onCooldown)
        {
            List<Projectile> projs = new();
            if (shotCount == 1)
            {
                var proj = Instantiate(projectilePrefab, transform.position, transform.rotation);
                projs.Add(proj.GetComponent<Projectile>());
            }
            else
            {
                float shotAngle = transform.rotation.eulerAngles.z;
                float minAngle = shotAngle - spreadAngle / 2.0f;
                float maxAngle = shotAngle + spreadAngle / 2.0f;
                float angleStep = spreadAngle / (shotCount + 1);

                for (int i = 0; i < shotCount; i++)
                {
                    float angle = minAngle + angleStep * (i + 1);
                    var proj = Instantiate(projectilePrefab, transform.position, Quaternion.Euler(0, 0, angle));
                    projs.Add(proj.GetComponent<Projectile>());
                }
            }

            foreach (var proj in projs)
            {
                proj.SetData(data);
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
