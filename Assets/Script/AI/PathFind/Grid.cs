using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    static Grid instance;

    public static Grid Inst { get { return instance; } }

    [Header("�̵� �Ұ��� ������ ���̾� ����ũ")]
    public LayerMask unwalkableMask; //������ �� ���� ������ ��Ÿ���� ���̾��ũ
    [Header("������ ũ�� : ���� ������ ����")]
    public Vector2 gridWorldSize;//������ ��ü ������
    [Header("������ ������")]
    public float nodeRadius; //����� ������
    [HideInInspector]
    public int gridSizeX, gridSizeY; //������ �غ��� ���� ũ��

    float nodeDiameter; // ����� ����(���� ��ĭ�� ���� ���̸� �������� ũ��)

    public Node[,] grid;
    public List<Node> path;
    Vector3 worldBottomLeft;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter); //(���� ���� �غ� / ����� ����)���� ���ڿ� ��尡 ��� �� �� �ִ��� ���� ���.
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);//(���� ���� ���� / ����� ����)���� ���ڿ� ��尡 ��� �� �� �ִ��� ���� ���.
        CreateGrid();
    }

    //Method : ���� ������� Grid ���� 
    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY]; //start���� ����� ũ�⸸ŭ ��� 2���� �迭 ����
        worldBottomLeft = transform.position - Vector3.right * (gridWorldSize.x / 2) - Vector3.up * (gridWorldSize.y / 2); //���ڿ��� ���� �Ʒ� ������
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics2D.OverlapCircle(worldPoint, nodeRadius, unwalkableMask));
                grid[x, y] = new Node(walkable, worldPoint, x, y);

            }
        }
    }

    public Node NodeFromWorldPoint(Vector3 worldPosition) //���� ĳ���Ͱ� �� �ִ� ��尡 ����� ��ȯ.
    {

        float percentX = (worldPosition.x - worldBottomLeft.x) / gridWorldSize.x;
        float percentY = (worldPosition.y - worldBottomLeft.y) / gridWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];
    }

    //public List<Node> GetNeighbors(Node node)
    //{
    //    List<Node> neighbors = new List<Node>();
    //    for (int x = -1; x <= 1; x++)
    //    {
    //        for (int y = -1; y <= 1; y++)
    //        {
    //            if (x == 0 && y == 0) //�ڱ� �ڽ� ����.
    //            {
    //                continue;
    //            }

    //            int checkX = node.gridX + x;
    //            int checkY = node.gridY + y;

    //            if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
    //            {
    //                neighbors.Add(grid[checkX, checkY]);
    //            }
    //        }
    //    }
    //    return neighbors;
    //}

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 0));

        if (grid != null)
        {
            foreach (Node n in grid)
            {
                Gizmos.color = (n.walkable) ? Color.white : Color.red;
                if (path != null)
                {
                    if (path.Contains(n))
                    {
                        Gizmos.color = Color.black;
                    }
                }
                else
                {
                }
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - 0.1f));

            }
        }
    }

}