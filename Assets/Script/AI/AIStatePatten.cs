using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIStatePatten : MonoBehaviour
{
    enum State //Ai의 상태 패턴
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

    Coroutine AimandFire;
    Vector2 targetVector;

    List<Node> MissionNodeTrack;
    List<Transform> ReconPositionList;
    List<Node> ForReconPositionNodeTrack;


    public GameObject player;
    float angleRange = 360f; // 각도범위
    float distance = 15f; // 부채꼴(시야)의 반지름 크기.
    float AimToEnemyTime = .1f;
    float alertTime = 1f;
    int nextNode;
    IEnumerator AimToEnemy()
    {
        AimToEnemyTime -= Time.deltaTime;
        yield return new WaitForSeconds(1f);
    }

    [SerializeField] State curstate;
    private void Start()
    {
        nextNode = 0;
        ReconPositionList = FindEnemyNodeListParent.GetComponentsInChildren<Transform>().ToList<Transform>();
        curstate = State.Mission;
        targetVector = (player.transform.position - gameObject.transform.position);
        ChangeState(State.Mission);

    }
    void ChangeState(State state) //상태 바꾸기
    {
        Debug.Log("AI 상태 변경");
        this.curstate = state;
        switch (curstate)
        {
            case State.Buy:
                Shopping();
                break;
            case State.Mission:
                Mission();
                break;
            case State.Alert:
                Alert();
                break;
            case State.Battle:
                Battle();
                break;
            case State.Defence:
                Defence();
                break;
            case State.Die:
                Die();
                break;
        }
    }

    void Shopping() //상점에서 무기를 구매함.
    {
        //가장 무기가 비싼 것부터 고르고, 남은 돈으로 그 다음으로 비싼 것을 고른다.
    }
    void Battle() //교전 상태.
    {
        gameObject.GetComponent<AI>().movespeed = 0;

        Vector2 targetVector = (player.transform.position - gameObject.transform.position);
        transform.up = (targetVector).normalized;
        if (targetVector.sqrMagnitude < distance * distance)
        {
            AimandFire = StartCoroutine(AimToEnemy());
            if (AimToEnemyTime <= 0)
            {
                StopCoroutine(AimandFire);
                gameObject.GetComponent<AI>().Fire();
                if (gameObject.GetComponent<AI>().weaponmanager.curweapon.magazine <= 0)
                {
                    gameObject.GetComponent<AI>().Reload(gameObject.GetComponent<AI>().weaponmanager.curweapon);
                }
            }
            ChangeState(State.Alert);

        }
    }
    void Mission()
    {
        gameObject.GetComponent<AI>().movespeed = 7f;

        //현재 목표 경로가 없을 경우
        if (MissionNodeTrack == null)
        {
            Debug.Log("길 없");
            int RandomNode = Mathf.RoundToInt(Random.Range(0, ReconPositionList.Count - 1));
            Debug.Log($"RandomNode : {RandomNode}");
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
            Debug.Log("길 있");
            AIPath(MissionNodeTrack);
            if (targetVector.sqrMagnitude < distance * distance)
            {
                float angle = Vector2.Angle(targetVector.normalized, transform.up);
                if (angle < angleRange)
                {
                    ChangeState(State.Battle);
                }
            }
        }
    }

    void AIPath(List<Node> nodeList)
    {
        while (nodeList != null && nodeList.Count > 0)
        {
            Node next = nodeList[0];
            Vector2 dir = (next.worldPosition - transform.position).normalized;
            transform.Translate(dir * Time.deltaTime * gameObject.GetComponent<AI>().movespeed, Space.World);
            transform.up = dir;

            if (Vector2.Distance(gameObject.transform.position, next.worldPosition) < 2f)
            {
                if (next == nodeList[nodeList.Count - 1]) //nodeList의 목표 지점 도착
                {
                    ChangeState(State.Alert);
                    nodeList.RemoveAt(0);
                    return;
                }
                nodeList.RemoveAt(0);
            }
        }

    }
    void Alert() //경계 상태.
    {
        gameObject.GetComponent<AI>().movespeed = 2f;

        Vector2 targetVector = (player.transform.position - gameObject.transform.position);
        Coroutine alertcoroutine = StartCoroutine(AlertTime());
        if (targetVector.sqrMagnitude < distance * distance) //적이 다시 시야에 들어왔을 때
        {
            StopCoroutine(alertcoroutine);
            ChangeState(State.Battle);
        }
        else if (this.alertTime <= 0f)
        {
            StopCoroutine(alertcoroutine);
            ChangeState(State.Mission);
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
                ChangeState(State.Battle);
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
