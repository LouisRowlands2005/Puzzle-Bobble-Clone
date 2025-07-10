using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shoot : MonoBehaviour
{
    bool ready;
    public bool shooting;
    public Vector2 direction;
    public GameObject blue;
    public GameObject green;
    public GameObject red;
    public GameObject yellow;
    public GameObject currentBall;
    public float velocity;
    public point point;
    int colour;
    ball ball;

    // Start is called before the first frame update
    void Start()
    {
        SpawnBubble();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && ready)
        {
            ShootBubble();
        }
    }

    public void SpawnBubble()
    {
        // Randomizes the colour of the ball
        colour = Random.Range(1, 5); // Upper limit is exclusive
        // Instantiates the ball at the spawn point with the randomized colour
        if (colour == 1)
        {
            currentBall = Instantiate(blue, transform.position, new Quaternion(0f, 0f, 0f, 1f));
        }
        else if (colour == 2)
        {
            currentBall = Instantiate(green, transform.position, new Quaternion(0f, 0f, 0f, 1f));
        }
        else if (colour == 3)
        {
            currentBall = Instantiate(red, transform.position, new Quaternion(0f, 0f, 0f, 1f));
        }
        else
        {
            currentBall = Instantiate(yellow, transform.position, new Quaternion(0f, 0f, 0f, 1f));
        }

        ball = currentBall.GetComponent<ball>();
        ball.colour = colour;
        ready = true;
    }

    void ShootBubble()
    {
        ready = false;
        shooting = true;
        direction = point.direction;
    }
}
