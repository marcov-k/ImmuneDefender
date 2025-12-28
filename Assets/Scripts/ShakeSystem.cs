using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class ShakeSystem : MonoBehaviour
{
    float startAngle;
    const float moveTime = 0.05f;
    const int moveCount = 6;
    const float baseAngle = 2.0f;
    IEnumerator shakeCoroutine;
    IEnumerator lerpCoroutine;
    bool shaking = false;
    readonly List<float> queuedIntensities = new();

    void Start()
    {
        startAngle = ClampAngle(transform.eulerAngles.z);
    }

    void Update()
    {
        UpdateShaking();
    }

    void UpdateShaking()
    {
        if (!shaking && queuedIntensities.Count > 0)
        {
            if (shakeCoroutine != null) StopCoroutine(shakeCoroutine);
            if (lerpCoroutine != null) StopCoroutine(lerpCoroutine);

            shakeCoroutine = ShakeCoroutine(queuedIntensities[0]);
            queuedIntensities.RemoveAt(0);
            StartCoroutine(shakeCoroutine);
        }
    }

    public void Shake(float intensity)
    {
        queuedIntensities.Add(intensity);
    }

    IEnumerator ShakeCoroutine(float intensity)
    {
        shaking = true;
        float angleOffset = baseAngle * intensity;
        float maxDiff = angleOffset * 2.0f;
        float diff;
        float initAngle;
        float targetAngle;
        float time;
        for (int i = 1; i <= moveCount; i++)
        {
            initAngle = ClampAngle(transform.eulerAngles.z);

            if (i == moveCount) // return to initial rotation
            {
                targetAngle = startAngle;
            }
            else
            {
                if (i % 2 == 0) // turn right on even
                {
                    targetAngle = startAngle - angleOffset;
                }
                else // turn left on odd
                {
                    targetAngle = startAngle + angleOffset;
                }
            }
            targetAngle = ClampAngle(targetAngle);

            diff = Mathf.Abs(targetAngle - initAngle);
            time = moveTime * (diff / maxDiff);
            lerpCoroutine = LerpRot(targetAngle, time);
            StartCoroutine(lerpCoroutine);
            yield return new WaitForSeconds(time);
        }
        transform.rotation = Quaternion.Euler(0, 0, startAngle);
        shaking = false;
    }

    IEnumerator LerpRot(float target, float totalTime)
    {
        float time = 0.0f;
        float startRot = ClampAngle(transform.eulerAngles.z);
        float newRot;
        while (time < totalTime)
        {
            while (PauseMenu.paused) yield return new WaitForEndOfFrame();

            time += Time.deltaTime;
            newRot = Mathf.Lerp(startRot, target, time / totalTime);
            transform.rotation = Quaternion.Euler(0, 0, newRot);
            yield return new WaitForEndOfFrame();
        }
        transform.rotation = Quaternion.Euler(0, 0, target);
    }

    float ClampAngle(float angle)
    {
        while (angle > 180.0f || angle < -180.0f)
        {
            if (angle > 180.0f) angle -= 360.0f;
            if (angle < -180.0f) angle += 360.0f;
        }
        return angle;
    }
}
