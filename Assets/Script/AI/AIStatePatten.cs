using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIStatePatten : MonoBehaviour
{
    public enum State //Ai�� ���� ����
    {
        Buy,
        Mission,
        Alert,
        Battle,
        Die,
        Defence
    }

    [SerializeField] GameObject FindEnemyNodeListParent; //���� ���� ��� ����Ʈ
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

    //void Shopping() //�������� ���⸦ ������.
    //{
    //    //���� ���Ⱑ ��� �ͺ��� ����, ���� ������ �� �������� ��� ���� ����.
    //}
    void Battle() //���� ����.
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
        //���� ��ǥ ��ΰ� ���� ���
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


    IEnumerator AIPath()
    {
        while (MissionNodeTrack != null && MissionNodeTrack.Count > 0)
        {
            Node nextNode = MissionNodeTrack[0];
            Vector2 dir = (nextNode.worldPosition - transform.position).normalized;
             transform.Translate(dir * Time.deltaTime * gameObject.GetComponent<AI>().movespeed, Space.World);
            transform.up = dir;

            if (Vector2.Distance(transform.position, nextNode.worldPosition) < 2f)
            {
                if (MissionNodeTrack.Count <= 1)
                {
                    MissionNodeTrack = null;
                    ChangeState(State.Mission);
                    break;
                }
                else
                {
                    MissionNodeTrack.RemoveAt(0);
                }
            }
            yield return null;
        }
    }
    void Alert() //��� ����.
    {
        Debug.Log("���!");
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
    void Defence() //��� ����.
    {
        Debug.Log("���!");
        //if (targetVector.sqrMagnitude < distance * distance)
        //{
        //    float angle = Vector2.Angle(targetVector.normalized, transform.up);

        //    if (angle < angleRange)
        //    {
        //        ChangeState(State.Battle);
        //    }
        //}
    }
    void Die() //��� ����
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
            Debug.Log("��� �ľ� �Ұ�");
        }
        return list;

    }

}
