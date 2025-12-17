using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;

public class Enemy : MonoBehaviour
{
    public int2 pos;
    public LevelManager manager;
    public float damage;
    public float speed;
    float moveDelay;
    public string moveLogicName;
    MoveLogic moveLogic;
    System.Random random;

    void Start()
    {
        InitValues();
        StartCoroutine(MoveCoroutine());
    }

    void InitValues()
    {
        moveDelay = 1.0f / speed;
        moveLogic = MoveCont.GetMoveLogic(moveLogicName);
        random = new();
    }

    IEnumerator MoveCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(moveDelay);

            if (pos.x == manager.positions.GetLength(0) - 1)
            {
                Attack();
            }
            else
            {
                var options = moveLogic.FindNextPos(pos, manager.positions);
                List<int2> openPoses = new();

                foreach (var option in options)
                {
                    if (manager.positions[option.x, option.y].filled == 0)
                    {
                        openPoses.Add(option);
                    }
                }

                while (openPoses.Count == 0 || PauseMenu.paused)
                {
                    yield return new WaitForEndOfFrame();
                    foreach (var option in options)
                    {
                        if (manager.positions[option.x, option.y].filled == 0)
                        {
                            openPoses.Add(option);
                        }
                    }
                }
                manager.positions[pos.x, pos.y].filled = 0;
                pos = openPoses[random.Next(openPoses.Count)];
                manager.positions[pos.x, pos.y].filled = 1;
                transform.position = manager.positions[pos.x, pos.y].positionTrans.position;
            }
        }
    }

    void Attack()
    {
        Debug.Log("Attacked");
        manager.positions[pos.x, pos.y].filled = 0;
        Destroy(gameObject);
    }
}
