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
            GetComponent<Collider2D>().enabled = false;
        }
    }


    IEnumerator FollowPlayer(Transform player)
    {
        GetComponent<Collider2D>().enabled = false;
        float step = 0;
        Vector3 vec = Vector3.zero;

        while (true)
        {
            if (Time.timeScale == 1)
            {
                if (Mathf.Abs((player.position - transform.position).magnitude) > 3)
                {
                    step = 0;
                    transform.position += (player.position - transform.position) * 0.06f;
                }
                //else if (Mathf.Abs((player.position - transform.position).magnitude) > 2f )
                //{
                //    step += 0.001f;
                //    vec.x = LeanTween.easeOutBack(transform.position.x,player.position.x - (player.position.x - transform.position.x) / 2f, step);
                //    vec.y = LeanTween.easeOutBack(transform.position.y, player.position.y - (player.position.y - transform.position.y) / 2f, step);
                //    transform.position = vec;
                //}
            }
            yield return null;
        }
    }
}
