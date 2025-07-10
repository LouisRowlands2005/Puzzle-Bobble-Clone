using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ball : MonoBehaviour
{
    public shoot shoot;
    public int colour;
    public grid grid;
    Vector2 position;

    // Start is called before the first frame update
    void Start()
    {
        shoot = GameObject.FindObjectOfType<shoot>();
        grid = GameObject.FindObjectOfType<grid>();
    }

    // Update is called once per frame
    void Update()
    {
        if (shoot.shooting)
        {
            // Calculates the updated position of the ball
            position.x = shoot.direction.x * shoot.velocity * Time.deltaTime;
            position.y = shoot.direction.y * shoot.velocity * Time.deltaTime;
            // Move the ball to the position
            gameObject.transform.Translate(position);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Wall"))
        {
            shoot.direction.x = -shoot.direction.x;
        }
        else if (other.gameObject.CompareTag("Grid"))
        {
            shoot.shooting = false;

            if (grid.AddBubble(colour, gameObject.transform.position))
            {
                grid.EndTurn();
                Destroy(gameObject);
                shoot.SpawnBubble();
            }
        }
    }
}
