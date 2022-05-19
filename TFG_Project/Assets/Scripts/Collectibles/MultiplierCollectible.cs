using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplierCollectible : Collectible
{
    private Vector3 startPosition;
    private bool taken;

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
            collision.gameObject.GetComponent<Player>().GetCollectableMultiplier();
            StartCoroutine(FollowPlayer(collision.transform));
            taken = true;
           // Destroy(gameObject);
        }
    }


    IEnumerator FollowPlayer(Transform player)
    {
        GetComponent<Collider2D>().enabled = false;
        //float timer = 0;
        while (true)
        {
            if (Time.timeScale == 1)
            {
                if (Mathf.Abs((player.position - transform.position).magnitude) > 2)
                {
                    //timer = 0;
                    transform.position += (player.position - transform.position) * 0.06f;
                }
                //else if (Mathf.Abs((player.position - transform.position).magnitude) > 0.1)
                //{
                //    timer += Time.deltaTime;
                //    Vector3.Lerp(transform.position, player.position, timer);
                //}
            }
            yield return null;
        }
       
    }
}
