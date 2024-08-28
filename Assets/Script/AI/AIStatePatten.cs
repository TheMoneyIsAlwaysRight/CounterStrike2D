using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIStatePatten : MonoBehaviour
{
    public enum State //Ai의 상태 패턴
    {
        Buy,
        Mission,
        Alert,
        Battle,
        Die,
        Defence
    }

    [SerializeField] GameObject FindEnemyNodeListParent; //적군 정찰 노드 리스트
    [SerializeField] PathFinding pathfinder;

    Vector2 targetVector;

    List<Node> MissionNodeTrack;
    List<Transform> ReconPositionList;
    List<Node> ForReconPositionNodeTrack;


    public GameObject player;
    public bool isCaptureEnemy;
    float AimToEnemyTime = .1f;
    float alertTime = 1f;
    int nextNode;

    Coroutine pathCoroutine;
    Coroutine aim;
    [SerializeField] State curstate;
    private void Start()
    {
        nextNode = 0;
        ReconPositionList = FindEnemyNodeListParent.GetComponentsInChildren<Transform>().ToList<Transform>();
        curstate = State.Mission;
        ChangeState(State.Mission);

    }
    public void ChangeState(State state) //상태 바꾸기
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

    //void Shopping() //상점에서 무기를 구매함.
    //{
    //    //가장 무기가 비싼 것부터 고르고, 남은 돈으로 그 다음으로 비싼 것을 고른다.
    //}
    void Battle() //교전 상태.
    {
        StopCoroutine(pathCoroutine);
        gameObject.GetComponent<AI>().movespeed = 0;
        aim = StartCoroutine(AimFire());
    }
    IEnumerator AimFire()
    {
        while (isCaptureEnemy)
        {
            Vector2 targetVector = (player.transform.position - transform.position);
            transform.up = (targetVector).normalized;
            gameObject.GetComponent<AI>().Fire();
            if (gameObject.GetComponent<AI>().weaponmanager.curweapon.magazine <= 0)
            {
                gameObject.GetComponent<AI>().Reload(gameObject.GetComponent<AI>().weaponmanager.curweapon);
            }
            yield return null;
        }
    }

    void Mission()
    {
        aim = null;
        gameObject.GetComponent<AI>().movespeed = 10f;
        gameObject.GetComponent<AI>().weaponmanager.ChangeWeapon(gameObject.GetComponent<AI>().weaponmanager.HAND[0]);
        //현재 목표 경로가 없을 경우
        if (MissionNodeTrack == null)
        {
            int RandomNode = Mathf.RoundToInt(Random.Range(0, ReconPositionList.Count - 1));

            if (Vector2.Distance(transform.position, (Vector2)ReconPositionList[RandomNode].position) < 0.5f)
            {
                return;
            }
            MissionNodeTrack = pathfinder.FindPath(transform.position, (Vector2)ReconPositionList[RandomNode].position);
            nextNode = 0;
            ChangeState(State.Mission);
        }
        else
        {
            pathCoroutine = StartCoroutine(AIPath());
        }
    }

    //Coroutine : AI가 A* 알고리즘으로 생성된 경로를 따라 움직이는 코루틴.
    IEnumerator AIPath()
    {
        while (MissionNodeTrack.Count > 0)
        {
            //생성된 경로의 맨 앞 정점을 바라보며 이동.
            Node nextNode = MissionNodeTrack[0];
            Vector2 dir = (nextNode.worldPosition - transform.position).normalized;
            transform.Translate(dir * Time.deltaTime * gameObject.GetComponent<AI>().movespeed, Space.World);
            transform.up = dir;

            //경로 상의 정점들을 도달할 때
            if (Vector2.Distance(transform.position, nextNode.worldPosition) < 2f)
            {
                //남은 경로가 없을 경우. 즉 경로의 끝에 도달한 경우.
                if (MissionNodeTrack.Count <= 1)
                {
                    //경로를 Null 및 다음 경로를 가도록하기 위해, 임무 수행 상태로 재지정.
                    MissionNodeTrack = null;
                    ChangeState(State.Mission);
                    break;
                }
                //남은 경로가 있다면, 다음 경로의 정점으로 이동하도록, 맨 앞 경로 요소를 제거.
                else
                {
                    MissionNodeTrack.RemoveAt(0);
                }
            }
            yield return null;
        }
    }
    void Alert() //경계 상태.
    {
        gameObject.GetComponent<AI>().movespeed = 10f;

        Vector2 targetVector = (player.transform.position - gameObject.transform.position);
        Coroutine alertcoroutine = StartCoroutine(AlertTime());

        if (this.alertTime <= 0f)
        {
            StopCoroutine(alertcoroutine);
            ChangeState(State.Mission);
            this.alertTime = 5f;
            nextNode = 0;

        }

    }
    void Defence() //경계 상태.
    {
        Debug.Log("방어!");
        //if (targetVector.sqrMagnitude < distance * distance)
        //{
        //    float angle = Vector2.Angle(targetVector.normalized, transform.up);

        //    if (angle < angleRange)
        //    {
        //        ChangeState(State.Battle);
        //    }
        //}
    }
    void Die() //사망 상태
    {
    }

    IEnumerator AlertTime()
    {
        this.alertTime -= Time.deltaTime;
        yield return new WaitForSeconds(0.1f);
    }
    void AISwapWeapon()
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
        List<Node> list = pathfinder.FindPath(transform.position, targetVector);

        if (list == null)
        {
            Debug.Log("경로 파악 불가");
        }
        return list;

    }

}
