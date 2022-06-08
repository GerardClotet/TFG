using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplierCollectible : Collectible
{
    [HideInInspector] public bool taken { get; private set;}
    private Vector3 startPosition;
    public Action countMe;

    public void StartCollectible()
    {
        startPosition = transform.position;
        Player.Instance.dieAction += ResetCollectible;
    }

    private void ResetCollectible()
    {
        StopAllCoroutines();
        GetComponent<Collider2D>().enabled = true;
        transform.position = startPosition;
        taken = false;
    }

    public void EndRoom()
    {
        Player.Instance.dieAction -= ResetCollectible;
        StopAllCoroutines();

        if (taken)
        {
            ReportGatherer.Instance.CollectibleMultiplierCounter();
            Destroy(gameObject);
        }
    }

    public override void GetMat()
    {
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() != null)
        {
            StartCoroutine(FollowPlayer(collision.transform));
            taken = true;
            GetComponent<Collider2D>().enabled = false;
        }
    }


    IEnumerator FollowPlayer(Transform player)
    {
        GetComponent<Collider2D>().enabled = false;
        float startTime = Time.time;
        float elapsedTime;
        float stepAmount = 0f;
        bool reset = false;
        while (true)
        {
            if (!reset)
            {
                elapsedTime = Time.time - startTime;
                stepAmount = Mathf.Pow(elapsedTime, 2);
                transform.position = Vector3.Lerp(transform.position, player.position, Mathf.MoveTowards(0, 1, stepAmount));
            }

            if(stepAmount > 1f && reset == false)
            {
                startTime = Time.time;
                reset = true;
            }
            else if(reset == true && Mathf.Abs((player.position - transform.position).magnitude)> 2f)
            {
                reset = false;
                startTime = Time.time;
            }
            yield return null;
        }
    }
}
