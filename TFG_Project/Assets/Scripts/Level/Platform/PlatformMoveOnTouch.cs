using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMoveOnTouch : PlatformBase
{
    private bool coroutineActive = false;

    //[Range(0.01f, 0.9f)] [SerializeField] private float maxIncreaseStep = 0.3f;
    private float origin = 0;

    public void RestartPlatform()
    {
        StopAllCoroutines();
        transform.position = startPosition;
        coroutineActive = false;
    }

    private void OnDestroy()
    {
        Player.Instance.dieAction -= RestartPlatform;
    }
    private void Start()
    {
        startPosition = transform.position;
        Player.Instance.dieAction += RestartPlatform;
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Player>())
        {
            collision.gameObject.GetComponent<Player>().transform.parent = transform;

            if (!coroutineActive && collision.GetContact(0).normal.y == -1)
                StartCoroutine(MovePlatform());
        }
    }

    protected override void OnCollisionExit2D(Collision2D collision)
    {
        if (coroutineActive)
        {
            Vector2 v = Vector2.zero;
            if (horizontal)
            {
                vel = (transform.position.x - origin) / (Time.time - startTime);
                v.x = vel;
            }
            else
            {
                vel = (transform.position.y - origin) / (Time.time - startTime);
                v.y = vel;
            }

            if (goingDown && !horizontal && startLeftOrBottom && Player.Instance.jumping)
            {
                Player.Instance.AddRBVel(-v * 1.5f);
            }
            //else if (v != Vector2.zero && !goingDown && Player.Instance.jumping)
            //{
            //    Debug.Log("Down " + Player.Instance.GetPlayerState());
            //}
        }

        if(collision.gameObject.GetComponent<Player>())
        {
            collision.gameObject.GetComponent<Player>().transform.parent = null;
        }
    }

    private  IEnumerator MovePlatform()
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
                origin = og;
                float increase = increaseStep;
                while(transform.position.x != dst)
                {
                    vec.x = Mathf.Lerp(og, dst, t += increaseStep * Time.timeScale);
                    //if (increase < maxIncreaseStep)
                    //    increase += increaseStep;

                    transform.position = vec;
                    yield return null;
                }
                t = 0;
                startTime = Time.time;
                origin = dst;
                while(transform.position.x != og)
                {
                    vec.x = Mathf.Lerp(dst, og, t += increaseStep*Time.timeScale);
                    transform.position = vec;
                    yield return null;
                }
                vel = 0f;
                origin = 0;
                break;
            case false:
                startTime = Time.time;
                origin = og;
                while (transform.position.y != dst)
                {
                    vec.y = Mathf.Lerp(og, dst, t += increaseStep * Time.timeScale);
                    transform.position = vec;
                    yield return null;
                }
                t = 0;
                startTime = Time.time;
                goingDown = true;
                origin = dst;
                while (transform.position.y != og)
                {
                    vec.y = Mathf.Lerp(dst, og, t += increaseStep * Time.timeScale);
                    transform.position = vec;
                    yield return null;
                }
                goingDown = false;
                vel = 0f;
                origin = 0;
                break;
        }
        coroutineActive = false;
    }
}
