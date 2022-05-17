using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Collectible : MonoBehaviour
{
    public abstract void OnCollision(Player player);
    public abstract void GetMat();
}
