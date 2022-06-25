using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformPerpetualMove : PlatformBase
{
    // Start is called before the first frame update
    [SerializeField] float holdTime = 0f;
    private float origin = 0f;
    private float destination = 0f;

    private void Awake()
    {
        startPosition = transform.position;
        float sign = startLeftOrBottom ? 1 : -1;
        destination = horizontal ? transform.position.x + sign * distance : transform.position.y + sign * distance;
        origin = horizontal ? transform.position.x : transform.position.y;
    }

    public void StartMoving()
    {
        if (horizontal)
            StartCoroutine(MoveX());
        else
            StartCoroutine(MoveY());
    }

    protected override void OnCollisionExit2D(Collision2D collision)
    {
        if (!goingDown)
        {
            Vector2 v = Vector2.zero;
            if (horizontal)
            {
                v.x = vel;
            }
            else if(startLeftOrBottom)
            {
                vel = (transform.position.y - origin) / (Time.time - startTime);
                v.y = vel;
            }

            if (v != Vector2.zero)
                Player.Instance.AddRBVel(v);
        }
    }

    private IEnumerator MoveX()
    {
        yield return new WaitForSeconds(holdTime);

        Vector3 vec = transform.position;
        float t;
        float maybeback = increaseStep;
        if (lazyBack || acceleration)
        {
            maybeback /= 2f;
        }
        while (true)
        {
            t = 0f;
            float increase = increaseStep;
            startTime = Time.time;
            while (transform.position.x != destination)
            {
                vec.x = Mathf.Lerp(origin, destination, t += increaseStep * Time.timeScale);
                vel = (transform.position.x - origin) / (Time.time - startTime);
                if (acceleration)
                {
                    increase += increaseStep/2f;
                }
                transform.position = vec;
                yield return null;
            }
            t = 0f;
            startTime = Time.time;
            while (transform.position.x != origin)
            {
                vec.x = Mathf.Lerp(destination, origin, t += maybeback * Time.timeScale);
                vel = (transform.position.x - destination) / (Time.time - startTime);
                transform.position = vec;

                yield return null;
            }
            yield return null;
        }
    }

    private IEnumerator MoveY()
    {
        yield return new WaitForSeconds(holdTime);
        Vector3 vec = transform.position;
        float t;
        float maybeback = increaseStep;
        if (lazyBack || acceleration)
        {
            maybeback /= 2f;
        }
        while (true)
        {
            t = 0f;
            float increase = increaseStep;
            startTime = Time.time;
            while (transform.position.y != destination)
            {
                vec.y = Mathf.Lerp(origin, destination, t += increaseStep * Time.timeScale);

                vel = (transform.position.y - origin) / (Time.time - startTime);
                if (acceleration)
                {
                    increase += increaseStep / 2f;
                }
                transform.position = vec;
                yield return null;
            }
            t = 0f;
            goingDown = true;
            while (transform.position.y != origin)
            {
                vec.y = Mathf.Lerp(destination, origin, t += maybeback * Time.timeScale);
                transform.position = vec;

                yield return null;
            }
            goingDown = false;
            yield return null;
        }
    }
    //public void RestartPlatform()
    //{
    //    StopAllCoroutines();
    //    transform.position = startPosition;
    //    StartMoving();
    //}

    public void StopPlatform()
    {
        StopAllCoroutines();
        transform.position = startPosition;
    }
}
