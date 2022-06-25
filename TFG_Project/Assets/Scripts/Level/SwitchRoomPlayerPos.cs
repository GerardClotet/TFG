using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class SwitchRoomPlayerPos : MonoBehaviour
{
    [SerializeField] private Transform movePosition;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var p = collision.GetComponent<Player>();
        if (p)
        {
            p.transform.position = movePosition.position;
        }
    }
}
