using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashCollectible : Collectible
{
    public override void GetMat()
    {
        throw new System.NotImplementedException();
    }

    public override void OnCollision(Player player)
    {
        player.ResetDashCollectable();
        //Make animation & when done
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() != null)
        {
            OnCollision(collision.gameObject.GetComponent<Player>());
        }
    }
}
