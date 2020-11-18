using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    Player player;

    [Header("Physics")]
    public float damage = 70;

    private void Start()
    {
        player = FindObjectOfType<Player>();
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
       var playerCollider = player.GetComponent<BoxCollider2D>();
        if(other == playerCollider)
        {
             //player.Die();
        }
    }
}
