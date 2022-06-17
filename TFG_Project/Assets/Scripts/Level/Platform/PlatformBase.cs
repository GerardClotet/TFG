using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlatformBase : MonoBehaviour
{
    [Tooltip("If false goes vertical")]
    [SerializeField] protected bool horizontal = true;
    [Tooltip("If true goes from left to right or from bottom to top. If false the opposite")]
    [SerializeField] protected bool startLeftOrBottom = true;
    [Tooltip("If true platform goes back slower")]
    [SerializeField] protected bool lazyBack = false;
    [SerializeField] protected float distance = 5f;
    [Range(0.001f, 0.9f)] [SerializeField] protected float increaseStep = 0.05f;
    [SerializeField] protected bool acceleration = false;
    protected bool goingDown = false;
    protected float vel = 0;
    protected float startTime = 0;
    protected Vector3 startPosition;



    protected abstract void OnCollisionExit2D(Collision2D collision);
    

    //private class template<T>
    //{

    //}
}
