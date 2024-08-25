using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public enum NodeStatus
{
    RUNNING,
    FAILURE,
    SUCCESS,
    NONE
}
public abstract class Node
{
    
    public List<Node> children = new();
    public NodeStatus status = NodeStatus.NONE;
    public abstract NodeStatus Run(BlackBoard board);
    public void AddChild(Node child)
    {
        children.Add(child);
    }
}
