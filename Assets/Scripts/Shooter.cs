using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using System.Collections;

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
    Player player;
    bool shooting = false;

    void Awake()
    {
        InitValues();
    }

    void InitValues()
    {
        player = FindFirstObjectByType<Player>();
        inputs = new();
        projectilePrefab = data.prefab;
        shotCount = data.shotCount;
        spreadAngle = data.spread;
        cooldown = 1.0f / data.firerate;
    }

    void Start()
    {
        StartCoroutine(ShootCoroutine());
    }

    void Update()
    {
        UpdateShooting();
    }

    void OnEnable()
    {
        inputs.Enable();
    }

    void OnDisable()
    {
        inputs.Disable();
    }

    void UpdateShooting()
    {
        if (inputs.Player.Attack.ReadValue<float>() != 0.0f)
        {
            shooting = true;
        }
        else shooting = false;
    }

    IEnumerator ShootCoroutine()
    {
        while (true)
        {
            if (active && shooting && !onCooldown)
            {
                player.Shoot();
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
            yield return new WaitForEndOfFrame();
        }
    }

    void ResetCooldown()
    {
        onCooldown = false;
    }
}
