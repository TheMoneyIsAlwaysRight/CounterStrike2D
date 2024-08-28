using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class PathFinding : MonoBehaviour
{

    //Method : A* 알고리즘 이용한 경로 찾기
    public List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Node startNode = Grid.Inst.NodeFromWorldPoint(startPos);
        Node targetNode = Grid.Inst.NodeFromWorldPoint(targetPos);
        List<Node> openSet = new List<Node>(); //방문해야할 노드들
        List<Node> visited = new List<Node>(); //방문한 노드들
        openSet.Add(startNode);
        while (openSet.Count > 0)
        {
            Node node = openSet[0];

            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost)
                { 
                    if (openSet[i].hCost < node.hCost)
                    { 
                        node = openSet[i];
                    }
                }
            }
            openSet.Remove(node);
            visited.Add(node);

            if (node == targetNode) //경로를 다 찾았다면.
            {
                List<Node> path = new List<Node>();
                Node currentNode = targetNode;

                while (currentNode != startNode)
                {
                    path.Add(currentNode);
                    currentNode = currentNode.parent;
                }
                path.Reverse();

                Grid.Inst.path = path;

                return path;

            }
            foreach (Node neighbour in GetNeighbors(node))
            {
                if (!neighbour.walkable || visited.Contains(neighbour))
                {
              
                    continue;
                }

                int newCostToNeighbor = node.gCost + Heuristic(node,neighbour);

                if (newCostToNeighbor < neighbour.gCost || !openSet.Contains(neighbour))
                { 

                    neighbour.gCost = newCostToNeighbor;
                    neighbour.hCost = Heuristic(neighbour, targetNode);
                    neighbour.parent = node;
                   if(!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }

        return null;

    }
    //Method : A* 알고리즘 휴리스틱 함수
    int Heuristic(Node A, Node B) 
    {
        int dstX = Mathf.Abs(A.gridX - B.gridX);
        int dstY = Mathf.Abs(A.gridY - B.gridY);
        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }
        return 14 * dstX + 10 * (dstY - dstX);
    }

    public List<Node> GetNeighbors(Node node)
    {
        int gridSizeX = Grid.Inst.gridSizeX;
        int gridSizeY = Grid.Inst.gridSizeY;
        Node[,] grid = Grid.Inst.grid;

        List<Node> neighbors = new List<Node>();
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0) //자기 자신 제외.
                {
                    continue;
                }

                int checkX = node.gridX + x;
                int checkY = node.gridY + y;

                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbors.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbors;
    }
}
