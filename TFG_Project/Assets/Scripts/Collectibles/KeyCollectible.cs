using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyCollectible : Collectible
{
    private Vector3 startPosition;
    [SerializeField] private LockDoor door;
    [HideInInspector] public bool taken { get; private set; }

    public bool isOpening { get; private set; }
    public void StartCollectible()
    {
        isOpening = false;
        startPosition = transform.position;
        Player.Instance.dieAction += ResetCollectible;
    }
    public override void GetMat()
    {

    }
    public override void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.GetComponent<Player>())
        {
            Debug.Log("Key Enter");
            StartCoroutine(FollowPlayer(collision.GetComponent<Player>().transform));
        }
    }

    private void ResetCollectible()
    {
        taken = false;
        StopAllCoroutines();
        GetComponent<Collider2D>().enabled = true;
        transform.position = startPosition;

    }

    public IEnumerator OpenDoor()
    {
        Player.Instance.dieAction -= ResetCollectible; //?
        isOpening = true;
        StopCoroutine(FollowPlayer(Player.Instance.transform));
        yield return new WaitForSeconds(0.3f);
        float startTime = Time.time;
        float elapsedTime;
        float stepAmount;
        Vector3 startPos = transform.position;
        while (transform.position != door.transform.position)
        {
            elapsedTime = Time.time - startTime;
            stepAmount = Mathf.Pow(elapsedTime, 2);
            transform.position = Vector3.Lerp(startPos, door.transform.position, stepAmount);
            if(stepAmount > 1)
            {
                break;
            }
            yield return null;
        }
        door.OpenDoor();

        Destroy(gameObject);
    }

    IEnumerator FollowPlayer(Transform player)
    {
        taken = true;
        GetComponent<Collider2D>().enabled = false;
        float startTime = Time.time;
        float elapsedTime;
        float stepAmount;
        while (true)
        {
            elapsedTime = Time.time - startTime;
            stepAmount = Mathf.Pow(elapsedTime, 2);
            transform.position = Vector3.Lerp(transform.position, player.position - Vector3.left, Mathf.MoveTowards(0, 1, stepAmount));
            yield return null;
        }
    }
}
