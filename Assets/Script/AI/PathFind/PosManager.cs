using System.Collections.Generic;
using UnityEngine;

public class PosManager : MonoBehaviour
{
    static PosManager instance;
    public static PosManager Inst { get { return instance; } }

    [Header("�̵� �Ұ��� ������ ���̾� ����ũ")]
    public LayerMask unwalkableMask; //������ �� ���� ������ ��Ÿ���� ���̾��ũ
    [Header("������ ũ�� : ���� ������ ����")]
    public Vector2 gridWorldSize;//������ ��ü ������
    [Header("������ ������")]
    public float nodeRadius; //����� ������
    [HideInInspector]
    public int gridSizeX, gridSizeY; //������ �غ��� ���� ũ��

    float nodeDiameter; // ����� ����(���� ��ĭ�� ���� ���̸� �������� ũ��)

    public Node[,] nodeArray;
    Vector3 worldBottomLeft;

    private void Awake()
    {
        instance = this;
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter); //(���� ���� �غ� / ����� ����)���� ���ڿ� ��尡 ��� �� �� �ִ��� ���� ���.
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);//(���� ���� ���� / ����� ����)���� ���ڿ� ��尡 ��� �� �� �ִ��� ���� ���.
        CreateGrid();
    }

    //Method : �ʿ� ���� �� ��ǥ�� �����ϴ� �Լ�
    void CreateGrid()
    {
        nodeArray = new Node[gridSizeX, gridSizeY]; //start���� ����� ũ�⸸ŭ ��� 2���� �迭 ����
        worldBottomLeft = transform.position - Vector3.right * (gridWorldSize.x / 2) - Vector3.up * (gridWorldSize.y / 2); //���ڿ��� ���� �Ʒ� ������
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.up * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics2D.OverlapCircle(worldPoint, nodeRadius, unwalkableMask));
                nodeArray[x, y] = new Node(walkable, worldPoint, x, y);

            }
        }
    }

    //Method : ���� ���� ��ǥ�� ���� �� ��ǥ�� ��ȯ �Լ�
    public Node NodeFromWorldPoint(Vector3 worldPosition)
    {

        float percentX = (worldPosition.x - worldBottomLeft.x) / gridWorldSize.x;
        float percentY = (worldPosition.y - worldBottomLeft.y) / gridWorldSize.y;

        percentX = Mathf.Clamp01(percentX);
        percentY = Mathf.Clamp01(percentY);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        for (int a = 0; a < nodeArray.GetLength(0); a++)
        {
            for (int b = 0; b < nodeArray.GetLength(1); b++)
            {
                if (a == x && y == b)
                {
                    return nodeArray[x, y];
                }
            }
        }
        return null;
    }

    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.blue;
    //    Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, gridWorldSize.y, 0));

    //    if (nodeArray != null)
    //    {
    //        foreach (Node n in nodeArray)
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