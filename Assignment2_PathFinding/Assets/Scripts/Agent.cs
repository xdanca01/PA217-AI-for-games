using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{
    aStar algo;
    bool draw = true;
    bool drawPath = false;
    bool followPath = false;
    float time = 0.0f;
    Vector3 roundingError = new Vector3(0.01f, 0.01f, 0.01f);
    
    List<Vector2Int> path = null;
    public Vector2Int CurrentTile { get; private set; }
    Vector2Int target;

    private Sprite _sprite;
    public Sprite Sprite
    {
        get
        {
            if(_sprite == null)
            {
                _sprite = GetComponentInChildren<SpriteRenderer>()?.sprite;
            }

            return _sprite;
        }
    }

    protected float movementSpeed; // in "tile" units
    protected Maze parentMaze;
    protected bool isInitialized = false;

    protected virtual void Start()
    {
        GameManager.Instance.DestinationChanged += OnDestinationChanged;
    }

    protected virtual void Update()
    {
        // TODO Assignment 2 ... this function might be of your interest. :-)
        // You are free to add new functions, create new classes, etc.
        // ---
        // The CurrentTile property should held the current location (tile-based) of an agent
        //
        // Have a look at Maze class, it contains several useful properties and functions.
        // For example, Maze.MazeTiles stores the information about the tiles of the maze.
        // Then, there are several functions for conversion/retrieval of tile positions, as well as for changing tile colors.
        // 
        // Finally, you can also have a look at GameManager to see what it provides.

        // NOTE
        // The code below is just a simple demonstration of some of the functionality / functions
        // You will need to replace it / change it
        if((draw == true || drawPath == true ) && tick() == false)
        {
            return;
        }
        var destWorld = parentMaze.GetWorldPositionForMazeTile(GameManager.Instance.DestinationTile);
        if (parentMaze.GetMazeTileForWorldPosition(transform.position) != GameManager.Instance.DestinationTile && draw == true)
        {
            draw = !algo.FindPath();
            drawPath = true;
            path = null;
            return;
        }
        if(drawPath == true)
        {
            if(path == null)
            {
                path = algo.GetPath();
            }
            if(path.Count <= 0)
            {
                drawPath = false;
                followPath = true;
                target = new Vector2Int(-1, -1);
            }
            else
            {
                Vector2Int node = path[0];
                path.Remove(node);
                parentMaze.SetFreeTileColor(node, Color.blue);
                return;
            }
        }
        if(followPath == true)
        {
            if (path == null || path.Count == 0)
            {
                path = algo.GetPath();
            }
            if(target.x == -1)
            {
                target = path[0];
            }
            Vector3 targetWorld = parentMaze.GetWorldPositionForMazeTile(target);
            Vector3 Direction = targetWorld - transform.position;
            Direction.Normalize();
            transform.Translate(Direction * movementSpeed * Time.deltaTime);
            //If i am in middle of target tile, then choose another target
            if ((transform.position - destWorld).magnitude <= 0.02f)
            {
                followPath = false;
                return;
            }
            if((targetWorld - transform.position).magnitude <= 0.02f)
            {
                target = path[0];
                path.Remove(target);
            }
        }

        /*var oldTile = CurrentTile;
        // Notice on the player's behavior that using this approach, a new tile is computed for a player
        // as soon as his origin crosses the tile border. Therefore, the player now often stops somehow "in the middle".
        // For this demo code, it does not really matter but just keep this in mind when dealing with movement.
        var afterTranslTile = parentMaze.GetMazeTileForWorldPosition(transform.position);

        if(oldTile != afterTranslTile)
        {
            CurrentTile = afterTranslTile;
            
            
        }

        if(CurrentTile == GameManager.Instance.DestinationTile)
        {
            //parentMaze.ResetTileColors();
            Debug.Log("YESSS");
        }*/
    }

    // This function is called every time the user sets a new destination using a left mouse button
    protected virtual void OnDestinationChanged(Vector2Int newDestinationTile)
    {
        // TODO Assignment 2 ... this function might be of your interest. :-)
        // The destination tile index is also accessible via GameManager.Instance.DestinationTile
        parentMaze.ResetTileColors();
        draw = true;
        Vector2 start = parentMaze.GetMazeTileForWorldPosition(transform.position);
        Vector2 end = GameManager.Instance.DestinationTile;
        algo = new aStar(start, end, parentMaze, GameManager.Instance.Heuristic.ToString());
        
    }

    public virtual void InitializeData(Maze parentMaze, float movementSpeed, Vector2Int spawnTilePos)
    {
        this.parentMaze = parentMaze;

        // The multiplication below ensures that movement speed is considered in tile-units so it stays
        // consistent across different scales of the maze
        this.movementSpeed = movementSpeed * parentMaze.GetElementsScale().x; 

        transform.position = parentMaze.GetWorldPositionForMazeTile(spawnTilePos.x, spawnTilePos.y);
        transform.localScale = parentMaze.GetElementsScale();

        CurrentTile = spawnTilePos;

        isInitialized = true;
    }

    bool tick()
    {
        if(time >= GameManager.Instance.tickTime)
        {
            time = 0.0f;
            return true;
        }
        time += Time.deltaTime;
        return false;
    }
}
