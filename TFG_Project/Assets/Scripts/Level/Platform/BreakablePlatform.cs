using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakablePlatform : MonoBehaviour
{
    [SerializeField] float holdTime = 2f;
    [SerializeField] float recoverTime = 0.7f;
    private bool coroutineActive = false;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<Player>() != null && !coroutineActive && collision.GetContact(0).normal.y <= -0.9f)
        {
            coroutineActive = true;
            StartCoroutine(DoYourThings());
        }
    }

    IEnumerator DoYourThings()
    {
        GetComponent<ParticleSystem>().Play();
        GetComponentInChildren<Animator>().SetTrigger("PlayerOnTop");
        float timer = 0;
        while(timer < holdTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        GetComponent<ParticleSystem>().Stop();
        GetComponentInChildren<Animator>().SetTrigger("BackToNormal");
        GetComponent<Collider2D>().enabled = false;

        Color c = GetComponentInChildren<SpriteRenderer>().material.color;
        c.a = 0;
        GetComponentInChildren<SpriteRenderer>().material.SetColor("_Color", c);
        yield return new WaitForSeconds(recoverTime);

        GetComponent<Collider2D>().enabled = true;
        c.a = 1f;
        GetComponentInChildren<SpriteRenderer>().material.SetColor("_Color", c);
        coroutineActive = false;
    }
}
