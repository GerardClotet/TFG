using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMove : MonoBehaviour
{
    private bool coroutineActive = false;

    [Tooltip("If false goes vertical")]
    [SerializeField] private bool horizontal = true;
    [Tooltip("If true goes from left to right or from bottom to top. If false the opposite")]
    [SerializeField] private bool startLeftOrBottom = true;

    [SerializeField] private float distance = 5f;
    [Range(0.01f, 0.9f)] [SerializeField] private float increaseStep = 0.05f;

    private float startTime = 0;
    private float vel = 0;
    private bool goingDown = false;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!coroutineActive && collision.gameObject.GetComponent<Player>())
        {
            if(collision.GetContact(0).normal.y == -1)
                StartCoroutine(MovePlatform());
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //float vel = Mathf.Abs(transform.position.x - og) / Time.time -startTime;
        if (vel != 0 && !goingDown)
        {
            Vector2 v = Vector2.zero;
            if (horizontal)
                v.x = vel;
            else
                v.y = vel;

            Player.Instance.AddRBVel(v * 1.5f);
        }
        Debug.Log("EXit");
    }

    IEnumerator MovePlatform()
    {
        coroutineActive = true;
        float sign = startLeftOrBottom ? 1 : -1;
        float dst = horizontal ? transform.position.x + sign * distance : transform.position.y + sign * distance;
        float og = horizontal ? transform.position.x : transform.position.y;
        Vector3 vec = transform.position;
        float t = 0;
        switch (horizontal)
        {
            case true:
                startTime = Time.time;
                while(transform.position.x != dst)
                {
                    vec.x = Mathf.Lerp(og, dst, t += increaseStep);
                    transform.position = vec;
                    vel = (transform.position.x - og) / (Time.time - startTime);
                    yield return null;
                }
                t = 0;
                startTime = Time.time;
                while(transform.position.x != og)
                {
                    vec.x = Mathf.Lerp(dst, og, t += increaseStep);
                    transform.position = vec;
                    vel = (transform.position.x - dst) / (Time.time - startTime);
                    yield return null;
                }
                vel = 0f;
                break;
            case false:
                startTime = Time.time;
                while (transform.position.y != dst)
                {
                    vec.y = Mathf.Lerp(og, dst, t += increaseStep);
                    transform.position = vec;
                    vel = (transform.position.y - og) / (Time.time - startTime);
                    yield return null;
                }
                t = 0;
                startTime = Time.time;
                goingDown = true;
                while (transform.position.y != og)
                {
                    vec.y = Mathf.Lerp(dst, og, t += increaseStep);
                    transform.position = vec;
                    vel = (transform.position.y - dst) / (Time.time - startTime);
                    yield return null;
                }
                goingDown = false;
                vel = 0f;
                break;
        }

        coroutineActive = false;
    }
}
