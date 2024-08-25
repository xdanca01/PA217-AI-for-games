using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackBoard : MonoBehaviour
{
    public Agent agent;
    public Maze maze;
    public AgentAction? action;
    public AgentBrain1 brain;
    
    public BlackBoard(Agent a, Maze m, AgentBrain1 b)
    {
        agent = a;
        maze = m;
        action = null;
        brain = b;
    }

    public void setAction(AgentAction a)
    {
        action = a;
    }

    //Returns tile that is not in same ROW and COL as agent, that is also free and I can move to it
    //It finds the free tile by going around the agent's point in spiral 
    ////0 right, 1 up, 2 left, 3 down, A agent
    ///
    //                    22221
    //                    32211
    //                    33A01
    //                    33000
    //                    30000
    //
    public Vector2Int TileNotInSameRowCol()
    {
        Vector2Int tile = agent.CurrentTile;
        int width = maze.GetMazeTiles().Count;
        int direction = 0;
        Vector2Int left = new Vector2Int(-1, 0);
        Vector2Int right = new Vector2Int(1, 0);
        Vector2Int down = new Vector2Int(0, 1);
        Vector2Int up = new Vector2Int(0, -1);
        Bomb myBomb = brain.ActiveBombs.Find(b => b.TileLocation == agent.CurrentTile);
        int xDif = 0, yDif = 0;
        //Move in spiral around the agent
        for (int steps = 1; steps <= width;)
        {
            //Move in steps to the right, up, left, down
            for(int step = 0; step < steps; ++step)
            {
                if (direction == 0)
                {
                    tile = tile + right;
                }
                else if (direction == 1)
                {
                    tile = tile + up;
                }
                else if (direction == 2)
                {
                    tile = tile + left;
                }
                else
                {
                    tile = tile + down;
                }
                //Check tile validation (is it valid and is it free)
                if (!maze.IsInBoundsTile(tile) || maze.GetTileType(tile) != MazeTileType.Free)
                {
                    continue;
                }
                xDif = Mathf.Abs(myBomb.TileLocation.x - tile.x);
                yDif = Mathf.Abs(myBomb.TileLocation.y - tile.y);
                //Check that tile is not in the bomb's field
                if((myBomb.TileLocation.x != tile.x && myBomb.TileLocation.y != tile.y) || xDif > myBomb.Strength || yDif > myBomb.Strength)
                {
                    //Route to the tile exists, then return the tile
                    if(brain.PathToTileExists(tile))
                    {
                        return tile;
                    }
                }
            }
            //After 2 cycles add distance, to go in spiral around the agents position
            if(direction % 2 == 1)
            {
                ++steps;
            }
            direction = (direction + 1) % 4;
        }
        return agent.CurrentTile;
    }
}
