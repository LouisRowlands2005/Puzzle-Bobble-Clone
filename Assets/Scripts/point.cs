using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class point : MonoBehaviour
{
    Vector2 mouse;
    Vector2 spawn;
    Vector2 start;
    public Vector2 direction;
    float length;
    float angle;
    public float offset;

    // Start is called before the first frame update
    void Start()
    {
        spawn.x = GameObject.Find("Ball Spawner").transform.position.x;
        spawn.y = GameObject.Find("Ball Spawner").transform.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        mouse.x = Camera.main.ScreenToWorldPoint(Input.mousePosition).x;
        mouse.y = Camera.main.ScreenToWorldPoint(Input.mousePosition).y;

        // Calculates the direction of mouse from the spawnpoint
        direction.x = mouse.x - spawn.x;
        direction.y = mouse.y - spawn.y;

        // Calculates the length of the direction vector
        length = Mathf.Sqrt((direction.x * direction.x) + (direction.y * direction.y));

        // Normalizes the direction vector
        direction.x = direction.x / length;
        direction.y = direction.y / length;

        // Calculates the half angle from the direction vector
        angle = Mathf.Atan2(direction.y, direction.x) / 2; 

        // Sets the rotation of the pointer using a quaternion
        gameObject.transform.rotation = new Quaternion(0, 0, Mathf.Sin(angle), Mathf.Cos(angle));

        // Calculates the distance of the pointer using the offset
        start.x = direction.x * offset;
        start.y = direction.y * offset;
        // Sets the position of the pointer
        gameObject.transform.position = new Vector2(spawn.x + start.x, spawn.y + start.y);
    }
}
