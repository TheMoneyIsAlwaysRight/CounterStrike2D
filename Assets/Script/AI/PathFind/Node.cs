using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Node
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gCost;
    public int hCost;
    public Node parent;
    public int gridX; //�׸���� ǥ���� ��� ���� x�ε���.
    public int gridY;//�׸���� ǥ���� ��� ���� Y�ε���.

    public Node(bool walkable, Vector3 worldPosition,int gridX,int gridY)
    {
        this.walkable = walkable;
        this.worldPosition = worldPosition;
        this.gridX = gridX;
        this.gridY = gridY;
    }
    public int fCost
    {
        get { return gCost + hCost; }
    }
}
