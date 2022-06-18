using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEater : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Transform destination;
    [SerializeField] private Transform origin;
    [SerializeField] private float timeToEnd = 10f;
    [SerializeField] private float stepTime = 0.05f;
    private void OnEnable()
    {
        transform.position = origin.position;
        StartCoroutine(ReachToEnd());
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if(collision.GetComponent<Player>() != null)
        //{
        //    StopAllCoroutines();
        //    transform.position = origin.position;
        //    StartCoroutine(ReachToEnd());
        //}
    }

    IEnumerator ReachToEnd()
    {
        Vector3 vec3 = transform.position;
        float t = 0;
        while (true)
        {
            if(Time.timeScale == 1)
            {
                vec3 = Vector3.Lerp(origin.position, destination.position, t);
                t += stepTime;
                transform.position = vec3;
            }
            yield return null;
        }
    }
}
