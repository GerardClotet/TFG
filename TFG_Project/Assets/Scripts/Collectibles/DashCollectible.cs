using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashCollectible : Collectible
{
    private static float resetFunctionTimer = 1.5f;

    public override void GetMat()
    {
        throw new System.NotImplementedException();
    }

    public override void OnCollision(Player player)
    {
        player.ResetDashCollectable();
        StartCoroutine(DashCollectibleTimer());
        //Make animation & when done
        //Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() != null)
        {
            OnCollision(collision.gameObject.GetComponent<Player>());
        }
    }

    IEnumerator DashCollectibleTimer()
    {
        GetComponent<Collider2D>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;

        float tmp_timer = 0;
        while(tmp_timer < resetFunctionTimer)
        {
            tmp_timer += Time.deltaTime;
            yield return null;
        }
        GetComponent<MeshRenderer>().enabled = true;
        GetComponent<Collider2D>().enabled = true;
    }
}
