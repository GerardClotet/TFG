using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class LevelEater : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Transform destination;
    [SerializeField] private Transform origin;
    [SerializeField] private float stepTime = 0.05f;
    [SerializeField] float holdTime = 3f;

    private bool stopEating = false;

    private void OnEnable()
    {
        transform.position = origin.position;
        Player.Instance.dieAction += Restart;
        StartCoroutine(ReachToEnd());
    }

    private void OnDisable()
    {
        Player.Instance.dieAction -= Restart;
    }
    private void OnDestroy()
    {
        transform.position = origin.position;
        Player.Instance.dieAction -= Restart;
    }

    private void Restart()
    {
        stopEating = true;
        transform.position = origin.position;
    }
    private void Stop()
    {
        StopCoroutine(ReachToEnd());
        transform.position = origin.position;
    }

    IEnumerator ReachToEnd()
    {
        yield return new WaitForSeconds(holdTime);
        Vector3 vec3 = transform.position;
        float t = 0;
        while (!stopEating)
        {
            if(Time.timeScale == 1)
            {
                vec3 = Vector3.Lerp(origin.position, destination.position, t);
                t += stepTime;
                transform.position = vec3;
            }
            yield return null;
        }
        stopEating = false;
        transform.position = origin.position;
        StartCoroutine(ReachToEnd());
    }
}
