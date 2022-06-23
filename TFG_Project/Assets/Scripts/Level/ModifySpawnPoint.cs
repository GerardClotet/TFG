using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifySpawnPoint : MonoBehaviour
{
    [SerializeField] Transform swapSpawnPoisition;
    [HideInInspector] public bool hasEntered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Player>() && !hasEntered)
        {
            ModifySpawnPoint[] swapers = gameObject.transform.parent.GetComponentsInChildren<ModifySpawnPoint>();
            for(int i =0; i < swapers.Length; i++)
            {
                if(swapers[i].hasEntered)
                {
                    swapers[i].hasEntered = false;
                }
            }
            hasEntered = true;
            GetComponentInParent<RoomChange>().ChangeSpawnPoint(swapSpawnPoisition);
        }
    }
}
