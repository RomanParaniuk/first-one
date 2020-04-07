using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooting : MonoBehaviour
{
    [Header("Gun Details")]
    public Joystick fireJoystick;
    public GameObject laserPrefab;
    public float shootDelay = 0.25f;
    public float laserSpeed = 5f;
    private float shootTimer;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Update()
    {
        Shoot();
    }

    public void Shoot()
    {
        if (fireJoystick.Direction != Vector2.zero && shootTimer < Time.time)
        {
            float angle = Mathf.Atan2(fireJoystick.Vertical, fireJoystick.Horizontal) * Mathf.Rad2Deg;
            GameObject laser = Instantiate(
                   laserPrefab,
                   transform.position,
                   Quaternion.Euler(new Vector3(0, 0, angle))) as GameObject;

            laser.GetComponent<Rigidbody2D>().velocity = fireJoystick.Direction.normalized * laserSpeed;
            shootTimer = shootDelay + Time.time;
        }
    }
}   
