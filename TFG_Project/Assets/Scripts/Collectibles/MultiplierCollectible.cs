using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplierCollectible : Collectible
{

    public override void GetMat()
    {
    }

    public override void OnCollision(Player player)
    {
        player.GetCollectableMultiplier();
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() != null)
        {
            OnCollision(collision.gameObject.GetComponent<Player>());
        }
    }
}
