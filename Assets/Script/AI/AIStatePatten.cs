using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIStatePatten : MonoBehaviour
{
    public enum State //Ai�� ���� ����
    {
        Mission,
        Battle,
        Die,
    }

    [SerializeField] GameObject PatrolSpot_Array; //���� ���� ��� ����Ʈ
    [SerializeField] PathFinding pathfinder;

    Vector2 targetVector;

    List<Node> curPath;
    List<Transform> patrolSpot;
    List<Node> ForReconPositionNodeTrack;


    public GameObject player;
    public bool isCaptureEnemy;
    float AimToEnemyTime = .1f;
    float alertTime = 1f;
    int nextNode;

    Coroutine pathCoroutine;
    Coroutine aimCoroutine;
    [SerializeField] State curstate;

    private void Awake()
    {
        patrolSpot = PatrolSpot_Array.GetComponentsInChildren<Transform>().ToList<Transform>();
    }
    private void Start()
    {
        nextNode = 0;
        ChangeState(State.Mission);
    }
    public void ChangeState(State state) //���� �ٲٱ�
    {
        this.curstate = state;
        switch (curstate)
        {
            case State.Mission:
                Mission();
                break;
            case State.Battle:
                Battle();
                break;
            case State.Die:
                Die();
                break;
        }
    }

    IEnumerator AimFire()
    {
        while (isCaptureEnemy)
        {
            //��ǥ�� �ȹٷ� �ٶ󺸱�.
            Vector2 targetVector = (player.transform.position - transform.position);
            transform.up = (targetVector).normalized;
            
            // ����ϵ�,ź���� �������� ������ �Լ� ȣ��.
            gameObject.GetComponent<AI>().Fire();
            if (gameObject.GetComponent<AI>(). weaponmanager.curweapon.magazine <= 0)
            {
                gameObject.GetComponent<AI>().Reload(gameObject.GetComponent<AI>().weaponmanager.curweapon);
            }
            yield return null;
        }
    }
    void Mission()
    {
        aimCoroutine = null;
        gameObject.GetComponent<AI>().movespeed = 10f;
       // gameObject.GetComponent<AI>().weaponmanager.ChangeWeapon(gameObject.GetComponent<AI>().weaponmanager.HAND[0]);
        if(pathCoroutine != null) { StopCoroutine(pathCoroutine); }

        //���� ��ǥ ��ΰ� ���� ���, ��� ����.
        if (curPath == null)
        {

            //���� ������ �� �������� �� ������ ����
            int RandomNode = Random.Range(0,patrolSpot.Count-1);

            curPath = 
                pathfinder.FindPath(transform.position,
                (Vector2)patrolSpot[RandomNode].position);
            //�ӹ� ���� ���·� ������
            nextNode = 0;
            ChangeState(State.Mission);
        }
        else //��ΰ� �ִٸ�, ��� ���󰡱� �ڷ�ƾ ����.
        {
           pathCoroutine = StartCoroutine(AIPath());
        }
    }
    void Battle() //���� ����.
    {
        if (pathCoroutine != null)
        {
            StopCoroutine(pathCoroutine);
        }
        gameObject.GetComponent<AI>().movespeed = 0;
        aimCoroutine = StartCoroutine(AimFire());
    }
    void Die() //��� ����
    {
    }


    //Coroutine : AI�� A* �˰������� ������ ��θ� ���� �����̴� �ڷ�ƾ.
    IEnumerator AIPath()
    {
        while (curPath.Count > 0)
        {
            //������ ����� �� �� ������ �ٶ󺸸� �̵�.
            Node nextNode = curPath[0];
            Vector2 dir = (nextNode.worldPosition - transform.position).normalized;
            transform.Translate(dir * Time.deltaTime * gameObject.GetComponent<AI>().movespeed, Space.World);
            transform.up = dir;

            //��� ���� �������� ������ ��
            if (Vector2.Distance(transform.position, nextNode.worldPosition) < 2f)
            {
                //���� ��ΰ� ���� ���. �� ����� ���� ������ ���.
                if (curPath.Count <= 1)
                {
                    //��θ� Null �� ���� ��θ� �������ϱ� ����, �ӹ� ���� ���·� ������.
                    curPath = null;
                    ChangeState(State.Mission);
                    break;
                }
                //���� ��ΰ� �ִٸ�, ���� ����� �������� �̵��ϵ���, �� �� ��� ��Ҹ� ����.
                else
                {
                    curPath.RemoveAt(0);
                }
            }
            yield return null;
        }
    }
    //void Alert() //��� ����.
    //{
    //    gameObject.GetComponent<AI>().movespeed = 10f;

    //    Vector2 targetVector = (player.transform.position - gameObject.transform.position);
    //    Coroutine alertcoroutine = StartCoroutine(AlertTime());

    //    if (this.alertTime <= 0f)
    //    {
    //        StopCoroutine(alertcoroutine);
    //        ChangeState(State.Mission);
    //        this.alertTime = 5f;
    //        nextNode = 0;

    //    }

    //}
    //void Defence() //��� ����.
    //{
    //    Debug.Log("���!");
    //    //if (targetVector.sqrMagnitude < distance * distance)
    //    //{
    //    //    float angle = Vector2.Angle(targetVector.normalized, transform.up);

    //    //    if (angle < angleRange)
    //    //    {
    //    //        ChangeState(State.Battle);
    //    //    }
    //    //}
    //}
    //IEnumerator AlertTime()
    //{
    //    this.alertTime -= Time.deltaTime;
    //    yield return new WaitForSeconds(0.1f);
    //}
    //void AISwapWeapon()
    //{
    //    if (gameObject.GetComponent<AI>().weaponmanager.HAND[0] == null)
    //    {
    //        gameObject.GetComponent<AI>().weaponmanager.BuyWeapon(gameObject.GetComponent<AI>().weaponmanager.WeaponInfo[30]);
    //        return;
    //    }
    //    if (gameObject.GetComponent<AI>().weaponmanager.curweapon != gameObject.GetComponent<AI>().weaponmanager.HAND[0])
    //    {
    //        gameObject.GetComponent<AI>().weaponmanager.ChangeWeapon(gameObject.GetComponent<AI>().weaponmanager.HAND[0]);
    //    }
    //}
}
