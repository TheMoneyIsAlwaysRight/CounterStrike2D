using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Node
{
    //이동 가능한 정점 여부.
    public bool walkable; 
    //이 정점의 월드 공간 좌표.
    public Vector3 worldPosition;
    
    public Node parent;
    
    //격자 상의 X,Y 인덱스
    public int gridX; //그리드로 표현된 노드 상의 x인덱스.
    public int gridY;//그리드로 표현된 노드 상의 Y인덱스.

    //휴리스틱 함수의 비용
    public int gCost;  // 현재 정점에서 목표 정점까지의 거리.
    public int hCost;  // 목표 정점에서 도착 정점까지의 휴리스틱 함수를 이용한 비용 (맨해튼, 유클리드 기법에 따라 달라짐) 
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
