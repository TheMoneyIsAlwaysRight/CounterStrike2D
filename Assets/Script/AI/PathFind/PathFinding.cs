using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    List<Node> openSet = new List<Node>(); //간 적 없는 정점들
    List<Node> visited = new List<Node>(); //이미 방문한 정점들

    //Method : A* 알고리즘 이용한 경로 찾기
    public List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        if(PathManager.Inst.nodeArray == null)
        { Debug.LogError($"NodeArray is NULL");
            return null;
        }
        //현재 위치한 정점과 목표 정점까지의 격자 상 좌표 호출
        Node startNode = PathManager.Inst.NodeFromWorldPoint(startPos);
        Node targetNode = PathManager.Inst.NodeFromWorldPoint(targetPos);

        openSet.Clear(); 
        visited.Clear(); 

        //0. 시작 정점 OpenList 삽입
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            //1. List의 맨 처음 요소를 지정.
            Node node = openSet[0];
            //2.열린 목록의 모든 노드를 검사하여 fCost가 더 낮거나 같은 노드를 찾음
            for (int i = 1; i < openSet.Count; i++)
            {// fCost가 더 낮거나, fCost가 같으면서 hCost가 더 낮은 노드를 찾음
                if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost)
                {  // 더 나은 노드를 현재 노드로 설정
                    if (openSet[i].hCost < node.hCost)
                    { 
                        node = openSet[i];
                    }
                }
            }
            // 3. 현재 정점을 열린 목록에서 제거하고 닫힌 목록에 추가
            openSet.Remove(node);
            visited.Add(node);

            // 4. 현재 정점이 목표 정점과 동일하다면 경로를 찾은 것이므로 경로를 구성하여 반환
            if (node == targetNode)
            {
                List<Node> path = new List<Node>();
                Node currentNode = targetNode;

                // 목표 정점부터 시작 정점까지 부모 노드를 따라 경로를 구성
                while (currentNode != startNode)
                {
                    path.Add(currentNode);
                    currentNode = currentNode.parent;
                }
                // 경로를 뒤집어 시작 노드에서 목표 노드 순으로 정렬
                path.Reverse();
                // 경로를 PathManager에 설정
                PathManager.Inst.path = path;

                return path; // 경로 반환

            }

            //현재 정점에서 주변의 3 x 3 을 검사
            foreach (Node neighbour in GetNeighbors(node))
            {
                // 이동이 불가능 혹은 이미 방문했던 경우 제외.
                if (!neighbour.walkable || visited.Contains(neighbour))
                {
                    continue;
                }
                // 현재 정점에서 인접 정점으로 가는 거리 비용 계산 ( F = G + H )     
                int newCostToNeighbor = node.gCost + Heuristic(node,neighbour);

                // 새 비용이 인접 정점의 기존 gCost보다 작거나 인접 정점이 열린 목록에 없을 경우, 새 비용으로 업데이트.
                if (newCostToNeighbor < neighbour.gCost || !openSet.Contains(neighbour))
                { 

                    //인접 정점으로 가는 비용 업데이트
                    neighbour.gCost = newCostToNeighbor;
                    neighbour.hCost = Heuristic(neighbour, targetNode);
                    //이전 정점을 현 정점으로 설정
                    neighbour.parent = node;
                    //인접 정점을 갈 곳 리스트에 추가
                   if(!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }
        //경로 파악 불가 null 반환.
        return null;

    }
    //Method :휴리스틱 함수
    int Heuristic(Node A, Node B) 
    {

        //각 정점간의 X,Y 만큼의 거리
        int dstX = Mathf.Abs(A.gridX - B.gridX);
        int dstY = Mathf.Abs(A.gridY - B.gridY);

        //return 10 * dstX + dstY * 10; ---> 맨해튼 휴리스틱이면 이걸로.

        //대각선 거리 고려한 계산 반환.
        if (dstX > dstY)
        {
            return 14 * dstY + 10 * (dstX - dstY);
        }
        return 14 * dstX + 10 * (dstY - dstX);
    }

    public List<Node> GetNeighbors(Node node)
    {
        int gridSizeX = PathManager.Inst.gridSizeX;
        int gridSizeY = PathManager.Inst.gridSizeY;
        Node[,] grid = PathManager.Inst.nodeArray;

        List<Node> neighbors = new List<Node>();

        // 현재 정점을 기준으로 3x3 주변을 검사하여 인접 정점 목록에 추가
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                //자기 자신은 검사에서 제외.
                if (x == 0 && y == 0)
                {
                    continue;
                }

                //인접 정점의 격자 좌표 상 (X,Y) 좌표 
                int checkX = node.gridX + x; 
                int checkY = node.gridY + y;

                //격자 좌표 내에 있는 인접 정점들 추가.
                if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                {
                    neighbors.Add(grid[checkX, checkY]);
                }
            }
        }
        return neighbors;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 0));

    //    if (grid != null)
    //    {
    //        foreach (Node n in grid)
    //        {
    //            Gizmos.color = (n.walkable) ? Color.white : Color.red;
    //            if (path != null)
    //            {
    //                if (path.Contains(n))
    //                {
    //                    Gizmos.color = Color.black;
    //                }
    //            }
    //            else
    //            {
    //            }
    //            Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));

    //        }
    //    }
    //}
}
