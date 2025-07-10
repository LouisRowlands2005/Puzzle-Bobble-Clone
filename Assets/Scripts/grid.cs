using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grid : MonoBehaviour
{
    public int rows = 7;
    public int columns = 9;
    int[,] gridMatrix;

    public GameObject blue;
    public GameObject green;
    public GameObject red;
    public GameObject yellow;
    GameObject ball;
    GameObject[,] gridBalls;

    gridBall gridBall;
    Vector2 position;
    Vector2Int gridPosition;

    public float radius = 0.5f;
    int turnCounter = 0;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
        CreateGrid();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            EndTurn();
        }
    }

    void Initialize()
    {
        // Creates the matrix to store the value of each ball in the grid
        gridMatrix = new int[rows, columns];
        // Creates the matrix to store the ball object for each correspending entry in the grid
        gridBalls = new GameObject[rows, columns];

        // Sets all entries in the first three rows to random values between 0 and 4
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                gridMatrix[i, j] = Random.Range(0, 5);
            }
        }

        // Sets all entries in the rest of the rows to 0
        for (int i = 3; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                gridMatrix[i, j] = 0;
            }
        }
    }

    void CreateGrid()
    {
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                // Calculates the position of each ball to fit into a hexagonal grid
                position = new Vector2(
                    gameObject.transform.position.x + (j * radius * 2) + ((i % 2 == 0) ? radius : 0),
                    gameObject.transform.position.y - (i * radius * Mathf.Sqrt(3))
                    );

                // Creates the ball object of the corresponding colour
                if (gridMatrix[i, j] == 1)
                {
                    ball = Instantiate(blue, position, new Quaternion(0f, 0f, 0f, 1f));
                }
                else if (gridMatrix[i, j] == 2)
                {
                    ball = Instantiate(green, position, new Quaternion(0f, 0f, 0f, 1f));
                }
                else if (gridMatrix[i, j] == 3)
                {
                    ball = Instantiate(red, position, new Quaternion(0f, 0f, 0f, 1f));
                }
                else if (gridMatrix[i, j] == 4)
                {
                    ball = Instantiate(yellow, position, new Quaternion(0f, 0f, 0f, 1f));
                }

                if (gridMatrix[i, j] != 0) // If the ball isn't set to empty
                {
                    // Sets the parent of the ball to the grid object
                    ball.transform.parent = gameObject.transform;
                    // Assigns the ball to the corresponding entry in the gridBalls matrix
                    gridBalls[i, j] = ball;
                    // Store's the balls data in the object using the GridBall script
                    gridBall = ball.GetComponent<gridBall>();
                    gridBall.row = i;
                    gridBall.column = j;
                    gridBall.colour = gridMatrix[i, j];
                }
            }
        }
    }

    public void EndTurn()
    {
        // Checks the bottom row of the grid for any balls
        for (int i = 0; i < columns; i++)
        {
            if (gridMatrix[(rows - 1), i] != 0)
            {
                Debug.Log("Game Over");
                turnCounter = 0;
                return;
            }
        }

        turnCounter++;
        if (turnCounter >= 3)
        {
            AddRow();
            turnCounter = 0;
        }
    }

    void AddRow()
    {
        // Shifts the matrix down by one row
        for (int i = rows - 1; i > 0; i--)
        {
            for (int j = 0; j < columns; j++)
            {
                gridMatrix[i, j] = gridMatrix[i - 1, j];
            }
        }

        // Randomizes the values of the top row
        for (int j = 0; j < columns; j++)
        {
            gridMatrix[0, j] = Random.Range(0, 5);
        }

        // Destroys all the balls in the grid
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                if (gridBalls[i, j] != null)
                {
                    Destroy(gridBalls[i, j]);
                }
            }
        }
        CreateGrid();
    }

    public bool AddBubble(int colour, Vector3 worldPosition)
    {
        // Converts the world coordinates to the local coordinates of the grid
        Vector3 localPosition = worldPosition - gameObject.transform.position;

        // Finds the nearest row to the ball
        float posY = -localPosition.y / (radius * Mathf.Sqrt(3));
        int row = Mathf.RoundToInt(posY);

        // Finds the nearest column to the ball
        float posX = (localPosition.x - ((row % 2 == 0) ? radius : 0)) / (radius * 2);
        int col = Mathf.RoundToInt(posX);

        gridPosition = new Vector2Int(row, col);

        if (row >= 0 && row < rows && col >= 0 && col < columns && gridMatrix[row, col] == 0)
        {
            gridMatrix[row, col] = colour;

            // Calculates the adjusted position of the ball to fit into the hexagonal grid
            worldPosition = new Vector2(
                gameObject.transform.position.x + (col * radius * 2) + ((row % 2 == 0) ? radius : 0),
                gameObject.transform.position.y - (row * radius * Mathf.Sqrt(3))
                );

            if (colour == 1)
                ball = Instantiate(blue, worldPosition, Quaternion.identity);
            else if (colour == 2)
                   ball = Instantiate(green, worldPosition, Quaternion.identity);
            else if (colour == 3)
                ball = Instantiate(red, worldPosition, Quaternion.identity);
            else
                ball = Instantiate(yellow, worldPosition, Quaternion.identity);

            ball.transform.parent = gameObject.transform;
            gridBalls[row, col] = ball;

            gridBall = ball.GetComponent<gridBall>();
            gridBall.row = row;
            gridBall.column = col;
            gridBall.colour = colour;

            List<Vector2Int> cluster = new List<Vector2Int>();
            HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

            ClusterDetection(row, col, colour, cluster, visited);

            if (cluster.Count >= 3)
            {
                foreach (Vector2Int pos in cluster)
                {
                    Destroy(gridBalls[pos.x, pos.y]);
                    gridBalls[pos.x, pos.y] = null;
                    gridMatrix[pos.x, pos.y] = 0;
                }
            }

            return true;
        }
        return false;
    }

    void ClusterDetection(int row, int col, int colour, List<Vector2Int> cluster, HashSet<Vector2Int> visited)
    {
        // Stores the position of the ball
        Vector2Int pos = new Vector2Int(row, col);
        // Checks to see if the ball either doesn't match the colour or has been checked already
        if (visited.Contains(pos) || gridMatrix[row, col] != colour)
            return;

        // Adds the position of the ball to the list and hash set
        visited.Add(pos);
        cluster.Add(pos);

        // Creates a list of every adjacent ball to the current ball
        List<Vector2Int> touching = FindTouching(row, col);

        foreach (Vector2Int touch in touching)
        {
            ClusterDetection(touch.x, touch.y, colour, cluster, visited);
        }
    }

    List<Vector2Int> FindTouching(int row, int col)
    {
        List<Vector2Int> touching = new List<Vector2Int>();

        // Adds the common positions to the list
        touching.Add(new Vector2Int(row - 1, col));
        touching.Add(new Vector2Int(row + 1, col));
        touching.Add(new Vector2Int(row, col - 1));
        touching.Add(new Vector2Int(row, col + 1));
        // Adds the remaining positions in accordance to the offset
        if (row % 2 == 0)
        {
            touching.Add(new Vector2Int(row - 1, col + 1));
            touching.Add(new Vector2Int(row + 1, col + 1));
        }
        else 
        {
            touching.Add(new Vector2Int(row - 1, col - 1));
            touching.Add(new Vector2Int(row + 1, col - 1));
        }

        // Removes any positions that are out of bounds or empty
        touching.RemoveAll(pos => pos.x < 0 || pos.x >= rows || pos.y < 0 || pos.y >= columns || gridMatrix[pos.x, pos.y] == 0);
        return touching;
    }
}
