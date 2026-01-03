using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using System.Linq;

public class Enemy : MonoBehaviour
{
    public EnemyData data;
    public int2 pos;
    int2 lastPos;
    public LevelManager manager;
    float damage;
    float health;
    float score;
    float moveDelay;
    float moveTime;
    MoveLogic moveLogic;
    System.Random random;
    Player player;
    bool disableResist = false;
    [SerializeField] ParticleSystem cytokineEm;
    ShakeSystem shakeSystem;
    bool killed = false;

    void Start()
    {
        InitValues();
        StartCoroutine(MoveCoroutine());
    }

    void InitValues()
    {
        shakeSystem = gameObject.AddComponent<ShakeSystem>();
        moveDelay = 1.0f / data.speed;
        moveTime = moveDelay / 2.0f;
        moveLogic = MoveCont.GetMoveLogic(data.moveLogicName);
        random = new();
        player = FindFirstObjectByType<Player>();
        health = data.health;
        damage = data.damage;
        score = data.score;
    }

    IEnumerator MoveCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(moveDelay);

            if (pos.x == manager.positions.GetLength(0) - 1)
            {
                StartCoroutine(Attack());
                break;
            }
            else
            {
                var options = moveLogic.FindNextPos(pos, manager.positions);
                List<int2> openPoses;

                openPoses = FindOpenPoses(lastPos, options, manager);

                while (openPoses.Count == 0 || PauseMenu.paused)
                {
                    yield return new WaitForEndOfFrame();
                    openPoses = FindOpenPoses(lastPos, options, manager);
                }

                if (pos.x == -1) manager.spawnPositions[pos.y].filled = 0;
                else manager.positions[pos.x, pos.y].filled = 0;

                lastPos = pos;
                pos = openPoses[random.Next(openPoses.Count)];
                manager.positions[pos.x, pos.y].filled = 1;
                transform.position = manager.positions[pos.x, pos.y].positionTrans.position;

                float time = 0.0f;
                Vector2 prevPos;

                if (lastPos.x == -1) prevPos = manager.spawnPositions[lastPos.y].positionTrans.position;
                else prevPos = manager.positions[lastPos.x, lastPos.y].positionTrans.position;

                var nextPos = manager.positions[pos.x, pos.y].positionTrans.position;
                while (time < moveTime)
                {
                    while (PauseMenu.paused)
                    {
                        yield return new WaitForEndOfFrame();
                    }

                    time += Time.deltaTime;
                    var stepPos = Vector2.Lerp(prevPos, nextPos, time / moveTime);
                    transform.position = stepPos;
                    yield return new WaitForEndOfFrame();
                }
                transform.position = manager.positions[pos.x, pos.y].positionTrans.position;
            }
        }
    }

    static List<int2> FindOpenPoses(int2 lastPos, List<int2> options, LevelManager manager)
    {
        List<int2> openPoses = new();
        foreach (var option in options)
        {
            if (manager.positions[option.x, option.y].filled == 0)
            {
                openPoses.Add(option);
            }
        }

        if (openPoses.Count > 1)
        {
            foreach (var pos in openPoses.ToList())
            {
                if (pos.x == lastPos.x && pos.y == lastPos.y)
                {
                    openPoses.Remove(pos);
                }
            }
        }
        return openPoses;
    }

    IEnumerator Attack()
    {
        manager.positions[pos.x, pos.y].filled = 0;
        if (player != null)
        {
            float time = 0.0f;
            var prevPos = manager.positions[pos.x, pos.y].positionTrans.position;
            while (time < moveTime)
            {
                while (PauseMenu.paused)
                {
                    yield return new WaitForEndOfFrame();
                }

                time += Time.deltaTime;
                var nextPos = player.transform.position;
                var stepPos = Vector2.Lerp(prevPos, nextPos, time / moveTime);
                transform.position = stepPos;
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            manager.EnemyKilled(0);
            gameObject.SetActive(false);
        }
    }

    public void TakeDamage(float damage, DefenceData defence)
    {
        if (!disableResist && !defence.strengths.Contains(data.type))
        {
            damage *= 0.5f;
        }
        else if (disableResist)
        {
            damage *= 1.5f;
        }
        health -= damage;
        shakeSystem.Shake(damage);
        if (health <= 0)
        {
            if (!killed)
            {
                killed = true;
                manager.positions[pos.x, pos.y].filled = 0;
                if (data.boss) manager.BossKilled(score);
                else manager.EnemyKilled(score);

                gameObject.SetActive(false);
            }
        }
    }

    public void ApplyCytokine()
    {
        if (!disableResist)
        {
            disableResist = true;
            cytokineEm.Play();
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7 && player != null && !killed)
        {
            player.TakeDamage(damage);
            if (data.boss) manager.BossKilled(0);
            else manager.EnemyKilled(0);
            gameObject.SetActive(false);
        }
    }
}
