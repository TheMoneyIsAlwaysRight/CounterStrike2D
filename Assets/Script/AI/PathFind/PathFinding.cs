using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class PathFinding : MonoBehaviour
{
    List<Node> openSet = new List<Node>(); //�� �� ���� ������
    List<Node> visited = new List<Node>(); //�̹� �湮�� ������

    //Method : A* �˰��� �̿��� ��� ã��
    public List<Node> FindPath(Vector3 startPos, Vector3 targetPos)
    {
        if(PathManager.Inst.nodeArray == null)
        { Debug.LogError($"NodeArray is NULL");
            return null;
        }
        //���� ��ġ�� ������ ��ǥ ���������� ���� �� ��ǥ ȣ��
        Node startNode = PathManager.Inst.NodeFromWorldPoint(startPos);
        Node targetNode = PathManager.Inst.NodeFromWorldPoint(targetPos);

        openSet.Clear(); 
        visited.Clear(); 

        //0. ���� ���� OpenList ����
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            //1. List�� �� ó�� ��Ҹ� ����.
            Node node = openSet[0];
            //2.���� ����� ��� ��带 �˻��Ͽ� fCost�� �� ���ų� ���� ��带 ã��
            for (int i = 1; i < openSet.Count; i++)
            {// fCost�� �� ���ų�, fCost�� �����鼭 hCost�� �� ���� ��带 ã��
                if (openSet[i].fCost < node.fCost || openSet[i].fCost == node.fCost)
                {  // �� ���� ��带 ���� ���� ����
                    if (openSet[i].hCost < node.hCost)
                    { 
                        node = openSet[i];
                    }
                }
            }
            // 3. ���� ������ ���� ��Ͽ��� �����ϰ� ���� ��Ͽ� �߰�
            openSet.Remove(node);
            visited.Add(node);

            // 4. ���� ������ ��ǥ ������ �����ϴٸ� ��θ� ã�� ���̹Ƿ� ��θ� �����Ͽ� ��ȯ
            if (node == targetNode)
            {
                List<Node> path = new List<Node>();
                Node currentNode = targetNode;

                // ��ǥ �������� ���� �������� �θ� ��带 ���� ��θ� ����
                while (currentNode != startNode)
                {
                    path.Add(currentNode);
                    currentNode = currentNode.parent;
                }
                // ��θ� ������ ���� ��忡�� ��ǥ ��� ������ ����
                path.Reverse();
                // ��θ� PathManager�� ����
                PathManager.Inst.path = path;

                return path; // ��� ��ȯ

            }

            //���� �������� �ֺ��� 3 x 3 �� �˻�
            foreach (Node neighbour in GetNeighbors(node))
            {
                // �̵��� �Ұ��� Ȥ�� �̹� �湮�ߴ� ��� ����.
                if (!neighbour.walkable || visited.Contains(neighbour))
                {
                    continue;
                }
                // ���� �������� ���� �������� ���� �Ÿ� ��� ��� ( F = G + H )     
                int newCostToNeighbor = node.gCost + Heuristic(node,neighbour);

                // �� ����� ���� ������ ���� gCost���� �۰ų� ���� ������ ���� ��Ͽ� ���� ���, �� ������� ������Ʈ.
                if (newCostToNeighbor < neighbour.gCost || !openSet.Contains(neighbour))
                { 

                    //���� �������� ���� ��� ������Ʈ
                    neighbour.gCost = newCostToNeighbor;
                    neighbour.hCost = Heuristic(neighbour, targetNode);
                    //���� ������ �� �������� ����
                    neighbour.parent = node;
                    //���� ������ �� �� ����Ʈ�� �߰�
                   if(!openSet.Contains(neighbour))
                    {
                        openSet.Add(neighbour);
                    }
                }
            }
        }
        //��� �ľ� �Ұ� null ��ȯ.
        return null;

    }
    //Method :�޸���ƽ �Լ�
    int Heuristic(Node A, Node B) 
    {

        //�� �������� X,Y ��ŭ�� �Ÿ�
        int dstX = Mathf.Abs(A.gridX - B.gridX);
        int dstY = Mathf.Abs(A.gridY - B.gridY);

        //return 10 * dstX + dstY * 10; ---> ����ư �޸���ƽ�̸� �̰ɷ�.

        //�밢�� �Ÿ� ����� ��� ��ȯ.
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

        // ���� ������ �������� 3x3 �ֺ��� �˻��Ͽ� ���� ���� ��Ͽ� �߰�
        for (int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                //�ڱ� �ڽ��� �˻翡�� ����.
                if (x == 0 && y == 0)
                {
                    continue;
                }

                //���� ������ ���� ��ǥ �� (X,Y) ��ǥ 
                int checkX = node.gridX + x; 
                int checkY = node.gridY + y;

                //���� ��ǥ ���� �ִ� ���� ������ �߰�.
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
