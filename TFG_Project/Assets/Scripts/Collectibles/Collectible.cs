using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Collectible : MonoBehaviour
{
    public abstract void GetMat();
    public abstract void OnTriggerEnter2D(Collider2D collision);
}
