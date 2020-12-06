using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shredder : MonoBehaviour
{
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.gameObject.name == "Player")
        {
            return;
        }
        Destroy(collision.gameObject);

    }
}
