using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class RoomChange : MonoBehaviour
{

    [SerializeField] private CinemachineVirtualCamera virtualCam;
    [SerializeField] private Transform spawnPoint;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") && !collision.isTrigger) //maybe do it through GetComponent<Player>()
        {
            collision.gameObject.GetComponent<Player>().SetSpawnPoint(spawnPoint.position);
            virtualCam.gameObject.SetActive(true);

            MultiplierCollectible collectible = GetComponentInChildren<MultiplierCollectible>();
            if (collectible != null)
            {
                collectible.StartCollectible();
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !collision.isTrigger) //maybe do it through GetComponent<Player>()
        {
            virtualCam.gameObject.SetActive(false);

            MultiplierCollectible collectible = GetComponentInChildren<MultiplierCollectible>();
            if (collectible != null)
            {
                collectible.EndRoom();
            }
        }
    }
}
