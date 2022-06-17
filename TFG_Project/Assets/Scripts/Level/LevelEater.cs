using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEater : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private Transform destination;
    [SerializeField] private Transform origin;
    [SerializeField] private float timeToEnd = 10f;

    private void OnEnable()
    {
        transform.position = origin.position;
        StartCoroutine(ReachToEnd());
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Player>() != null)
        {
            StopAllCoroutines();
            transform.position = origin.position;
            StartCoroutine(ReachToEnd());
        }
    }

    IEnumerator ReachToEnd()
    {
        float stepX = (destination.position.x - origin.position.x)/timeToEnd;
        float stepY = (destination.position.x - origin.position.x) / timeToEnd;
        // Vector3 vec3 = origin;
        float step = 0;
        while (true)
        {
        
            if(Time.timeScale == 1)
            {
                transform.position = Vector3.Lerp(origin.position, destination.position, Mathf.MoveTowards(0, 1, Time.deltaTime));
            }
            yield return null;
        }
    }
}
