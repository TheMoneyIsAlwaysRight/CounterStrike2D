using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Node
{
    //�̵� ������ ���� ����.
    public bool walkable; 
    //�� ������ ���� ���� ��ǥ.
    public Vector3 worldPosition;
    
    public Node parent;
    
    //���� ���� X,Y �ε���
    public int gridX; //�׸���� ǥ���� ��� ���� x�ε���.
    public int gridY;//�׸���� ǥ���� ��� ���� Y�ε���.

    //�޸���ƽ �Լ��� ���
    public int gCost;  // ���� �������� ��ǥ ���������� �Ÿ�.
    public int hCost;  // ��ǥ �������� ���� ���������� �޸���ƽ �Լ��� �̿��� ��� (����ư, ��Ŭ���� ����� ���� �޶���) 
    //fCost = gCost + hCost 
    
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
