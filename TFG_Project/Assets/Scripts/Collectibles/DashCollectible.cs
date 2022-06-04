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

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() != null)
        {
            collision.gameObject.GetComponent<Player>().ResetDashCollectable();
            StartCoroutine(DashCollectibleTimer());
        }
    }

    IEnumerator DashCollectibleTimer()
    {
        GetComponent<Collider2D>().enabled = false;
        GetComponent<MeshRenderer>().material.SetColor("_Color", Color.green);
        float tmp_timer = 0;
        while(tmp_timer < resetFunctionTimer)
        {
            tmp_timer += Time.deltaTime;
            yield return null;
        }
        GetComponent<MeshRenderer>().material.SetColor("_Color", Color.blue);
        GetComponent<Collider2D>().enabled = true;
    }
}
