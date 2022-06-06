using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashCollectible : Collectible
{
    private static float resetFunctionTimer = 1.5f;
    private ParticleSystem[] p;
    private void Awake()
    {
        p = GetComponentsInChildren<ParticleSystem>();
    }
    public override void GetMat()
    {
        throw new System.NotImplementedException();
    }

    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() != null)
        {
            collision.gameObject.GetComponent<Player>().ResetDashCollectable();
            p[0].Play();
            StartCoroutine(DashCollectibleTimer());
        }
    }

    IEnumerator DashCollectibleTimer()
    {
        p[1].Play();
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
        p[1].Stop();
    }
}
