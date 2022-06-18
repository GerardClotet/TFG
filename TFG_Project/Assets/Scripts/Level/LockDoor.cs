using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class LockDoor : MonoBehaviour
{
    public void OpenDoor()
    {
        StartCoroutine(SlideDown(GetComponentInChildren<MeshRenderer>().gameObject));
    }

    IEnumerator SlideDown(GameObject go)
    {
        go.transform.parent = null;
        var sprite = GetComponent<SpriteRenderer>();
        float hOg = 1;
        float wOg = 1;
        
        float spriteTime = 0;
        Vector3 v = Vector3.one;
        while (transform.localScale.x != 0)
        {
            v.x = Mathf.Lerp(wOg, 0, spriteTime);
            v.y = Mathf.Lerp(hOg, 0, spriteTime);
            transform.localScale = v;
            spriteTime += 0.2f;
            yield return null;
        }

        float dst = go.transform.position.y - 3f;
        float og = go.transform.position.y;
        float t = 0;
        v = go.transform.position;
        while (go.transform.position.y != dst)
        {
            v.y = Mathf.Lerp(og, dst, t);
            go.transform.position = v;
            t += 0.1f;
            yield return null;
        }
        Destroy(gameObject);
        Destroy(go);
    }
}
