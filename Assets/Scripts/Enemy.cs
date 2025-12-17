using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using System.Linq;

public class Enemy : MonoBehaviour
{
    public int2 pos;
    public int2 lastPos;
    public LevelManager manager;
    public float damage;
    public float health;
    public float speed;
    public float score;
    float moveDelay;
    float moveTime;
    public string moveLogicName;
    MoveLogic moveLogic;
    System.Random random;
    Player player;

    void Start()
    {
        InitValues();
        StartCoroutine(MoveCoroutine());
    }

    void InitValues()
    {
        moveDelay = 1.0f / speed;
        moveTime = moveDelay / 2.0f;
        moveLogic = MoveCont.GetMoveLogic(moveLogicName);
        random = new();
        player = FindFirstObjectByType<Player>();
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
                List<int2> openPoses = new();

                openPoses = FindOpenPoses(lastPos, options, manager);

                while (openPoses.Count == 0 || PauseMenu.paused)
                {
                    yield return new WaitForEndOfFrame();
                    openPoses = FindOpenPoses(lastPos, options, manager);
                }
                manager.positions[pos.x, pos.y].filled = 0;
                lastPos = pos;
                pos = openPoses[random.Next(openPoses.Count)];
                manager.positions[pos.x, pos.y].filled = 1;
                transform.position = manager.positions[pos.x, pos.y].positionTrans.position;

                float time = 0.0f;
                var prevPos = manager.positions[lastPos.x, lastPos.y].positionTrans.position;
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
        else gameObject.SetActive(false);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            manager.EnemyKilled(this);
            gameObject.SetActive(false);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 7 && player != null)
        {
            player.TakeDamage(damage);
            gameObject.SetActive(false);
        }
    }
}
