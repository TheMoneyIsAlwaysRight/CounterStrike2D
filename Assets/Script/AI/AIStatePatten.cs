using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

public class AIStatePatten : MonoBehaviour
{
    [SerializeField] PathFinding pathfinder;
    [SerializeField] GameObject Enemy;
    float AimToEnemyTime = .1f;
    int nextNode;
    float alertTime = 1f;
    Coroutine AimandFire;
    [SerializeField] GameObject FindEnemyNodeListParent; //적군 정찰 노드 리스트
    Vector2 targetVector;

    List<Node> MissionNodeTrack;
    List<Transform> ReconPositionList;
    List<Node> ForReconPositionNodeTrack;
    [SerializeField]bool IsRecon;
    
    IEnumerator AimToEnemy()
    {
        AimToEnemyTime -= Time.deltaTime;
        yield return new WaitForSeconds(1f);
    }
    float angleRange = 360f; // 각도범위
    float distance = 15f; // 부채꼴(시야)의 반지름 크기.
    enum State //Ai의 상태 패턴
    {
        shopping,
        mission,
        alert,
        battle,
        die,
        recon,
        defence
    }
    State curstate;
    bool IspathFind = false;

    private void Start()
    {
        //curstate = State.shopping;
        curstate = State.mission;
        nextNode = 0;

        ReconPositionList = FindEnemyNodeListParent.GetComponentsInChildren<Transform>().ToList<Transform>();
        curstate = State.mission;

        targetVector = (Enemy.transform.position - gameObject.transform.position);

    }
    private void Update()
    {
      AIWeaponPatten();
    }

    private void FixedUpdate()
    {
        switch (curstate)
        {
            case State.shopping:
                Shopping();
                break;
            case State.mission:
                Mission();
                break;
            case State.alert:
                Alert();
                break;
            case State.battle:
                Battle();
                break;
            case State.defence:
                Defence();
                break;
            case State.die:
                Die();
                break;
        }
    }
    void ChangeState(State state) //상태 바꾸기
    {
        this.curstate = state;
    }

    void Shopping() //상점에서 무기를 구매함.
    {
       //가장 무기가 비싼 것부터 고르고, 남은 돈으로 그 다음으로 비싼 것을 고른다.
    }
    void Battle() //교전 상태.
    {
        gameObject.GetComponent<AI>().movespeed = 0;

        Vector2 targetVector = (Enemy.transform.position - gameObject.transform.position);
        transform.up = (targetVector).normalized;
        if (targetVector.sqrMagnitude<distance*distance)
        {
            AimandFire = StartCoroutine(AimToEnemy());
            if (AimToEnemyTime <=0)
            {
                StopCoroutine(AimandFire);
                gameObject.GetComponent<AI>().Fire();
                if (gameObject.GetComponent<AI>().weaponmanager.curweapon.magazine <= 0)
                {
                    //Debug.Log("적이 재장전 중...");
                    gameObject.GetComponent<AI>().Reload(gameObject.GetComponent<AI>().weaponmanager.curweapon);
                }
            }
            ChangeState(State.alert);

        }
    }
    void Mission()
    {
        gameObject.GetComponent<AI>().movespeed = 7f;

        if (!IspathFind)
        {
            int RandomNode = Mathf.RoundToInt(Random.Range(0, ReconPositionList.Count - 1));
            if (Vector2.Distance(transform.position, (Vector2)ReconPositionList[RandomNode].position) < 0.5f)
            {
                return;
            }
            MissionNodeTrack = pathfinder.FindPath(transform.position, (Vector2)ReconPositionList[RandomNode].position);
            nextNode = 0;
            IspathFind = true;
        }
        else
        {
            if (MissionNodeTrack == null)
            {
                IspathFind = false;
                return;
            }
            else if(MissionNodeTrack != null)
            {
                AIPath(MissionNodeTrack);
                if (targetVector.sqrMagnitude < distance * distance)
                {
                    float angle = Vector2.Angle(targetVector.normalized, transform.up);

                    if (angle < angleRange)
                    {
                        ChangeState(State.battle);
                    }
                }
            }
            
        }
    }

    void AIPath(List<Node> nodeList)
    {
        if (nodeList != null && nodeList.Count > 0)
        {
            Node next = nodeList[0];

            Vector2 dir = (next.worldPosition - transform.position).normalized;
            transform.Translate(dir * Time.deltaTime * gameObject.GetComponent<AI>().movespeed, Space.World);
            if (Vector2.Distance(gameObject.transform.position, next.worldPosition) < 2f)
            {
                if (next == nodeList[nodeList.Count - 1]) //nodeList의 목표 지점 도착
                {
                    ChangeState(State.alert);
                    nodeList.RemoveAt(0);
                    return;
                }
                nodeList.RemoveAt(0);
            }
        }

    }
    //void Recon()
    //{
    //    gameObject.GetComponent<AI>().movespeed = 7f;

    //    if (!IspathFind)
    //    {
    //        int RandomNode = Mathf.RoundToInt(Random.Range(0, ReconPositionList.Count - 1));
    //        if (Vector2.Distance(transform.position,(Vector2)ReconPositionList[RandomNode].position) < 0.5f)
    //        {
    //            return;
    //        }
    //        ForReconPositionNodeTrack = pathfinder.FindPath(transform.position, (Vector2)ReconPositionList[RandomNode].position);
    //        IspathFind = true;
    //    }
    //    else
    //    {
    //        AIPath(ForReconPositionNodeTrack);

    //        if (targetVector.sqrMagnitude < distance * distance)
    //        {
    //            float angle = Vector2.Angle(targetVector.normalized, transform.up);

    //            if (angle < angleRange)
    //            {
    //                ChangeState(State.battle);
    //                IspathFind = false;

    //            }
    //        }
    //    }

    //}
    void Alert() //경계 상태.
    {
        gameObject.GetComponent<AI>().movespeed = 2f;

        Vector2 targetVector = (Enemy.transform.position - gameObject.transform.position);
        Coroutine alertcoroutine = StartCoroutine(AlertTime());
        if (targetVector.sqrMagnitude < distance * distance) //적이 다시 시야에 들어왔을 때
        {
            StopCoroutine(alertcoroutine);
            ChangeState(State.battle);
        }
        else if(this.alertTime <=0f)
        {
            StopCoroutine(alertcoroutine);
            ChangeState(State.mission);
            IspathFind = false;
            this.alertTime = 5f;
            nextNode = 0;

        }

    }
    void Defence() //경계 상태.
    {       
        if (targetVector.sqrMagnitude < distance * distance)
        {
            float angle = Vector2.Angle(targetVector.normalized, transform.up);

            if (angle < angleRange)
            {
                ChangeState(State.battle);
            }
        }
    }
    void Die() //사망 상태
    {
    }

    IEnumerator AlertTime()
    {
        this.alertTime -= Time.deltaTime;
        yield return new WaitForSeconds(0.1f);
    }
    void AIWeaponPatten()
    {
        if (gameObject.GetComponent<AI>().weaponmanager.HAND[0] == null)
        {
            gameObject.GetComponent<AI>().weaponmanager.BuyWeapon(gameObject.GetComponent<AI>().weaponmanager.WeaponInfo[30]);
            return;
        }
        if (gameObject.GetComponent<AI>().weaponmanager.curweapon != gameObject.GetComponent<AI>().weaponmanager.HAND[0])
        {
            gameObject.GetComponent<AI>().weaponmanager.ChangeWeapon(gameObject.GetComponent<AI>().weaponmanager.HAND[0]);
        }
    }
    List<Node> CalculatePath()
    {
        List<Node> list = pathfinder.FindPath(transform.position,targetVector);

        IspathFind = true;
        if(list == null)
        {
            Debug.Log("경로 파악 불가");
        }
        return list;

    }

}
