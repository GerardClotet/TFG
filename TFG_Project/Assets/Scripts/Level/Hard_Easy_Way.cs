using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hard_Easy_Way : MonoBehaviour
{
    public bool hasEntered { get; private set; } = false;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Player>() && !hasEntered)
        {

        }
    }
}
