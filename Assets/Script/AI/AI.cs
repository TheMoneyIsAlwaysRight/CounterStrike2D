using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;

public class AI : Human
{
    [SerializeField] float movespeed;
    Vector2 moveDir;
    public static Vector2 mouse;
    float angle;

    [SerializeField] Transform target;

    float angleRange = 90f; // ��������
    float distance = 15f; // ��ä��(�þ�)�� ������ ũ��.

    void AIeye() // Ai�� �þ�
    {
        Vector3 targetVector = (target.transform.position - gameObject.transform.position); // ���� ������ ����

        if (targetVector.magnitude < distance) //���� ������ �Ÿ��� ���� ũ�� ���� �۴ٸ�
        {
            float Dot = Vector3.Dot(targetVector.normalized, transform.up); //�� ���� ���� �����
            float theta = Mathf.Acos(Dot); //��ũ �ڻ���(���ڻ���)�� ���� ��Ÿ(��) ���ϱ�.
            float degree = Mathf.Rad2Deg * theta;

            if(degree <= angleRange)
            {
                Debug.Log("���� ���� �߰��߽��ϴ�.");
            }
            else
            {
                Debug.Log("���� ���� ���ƽ��ϴ�.");
            }
        }

    }

    private void OnDrawGizmos()
    {
        Handles.color = new Color(0f, 0f, 1f, 0.2f);

        Handles.DrawSolidArc(transform.position, transform.forward, transform.up, angleRange / 2, distance); 
        
        Handles.DrawSolidArc(transform.position, transform.forward, transform.up, -angleRange / 2, distance);
    }












    void Update()
    {
        this.Hpcheck();
        AIeye();
        gameObject.transform.Translate(moveDir * movespeed * Time.deltaTime,Space.World);
    }
    void AiMove() //Ai�� ������
    {
        this.Hpcheck();
        gameObject.transform.Translate(moveDir * movespeed * Time.deltaTime, Space.World);

    }


    public void Fire(Weapon curweapon)
    {
        if (curweapon.magazine > 0)
        {

            Vector3 fireDir = transform.up;
            RaycastHit2D ray = Physics2D.Raycast(curweapon.firepoint.transform.position, fireDir);
            if (ray.collider != null)
            {
                if (ray.collider.gameObject.GetComponent<Human>())
                {
                    Human human = ray.collider.gameObject.GetComponent<Human>();
                    ray.collider.gameObject.GetComponent<IDamagable>().Damage(human, curweapon.damage);
                }
            }
            Debug.DrawRay(transform.position, fireDir * float.MaxValue, Color.red, 1f);
            curweapon.magazine--;
        }

    }

    public void Reload(Weapon curweapon)
    {
        if (curweapon.maxammo > curweapon.magazineCapacity)
        {
            curweapon.maxammo -= curweapon.magazineCapacity - curweapon.magazine;
            curweapon.magazine = curweapon.magazineCapacity;
        }
        else if (curweapon.maxammo <= curweapon.magazineCapacity && (curweapon.maxammo != 0))
        {
            curweapon.magazine = curweapon.maxammo;
            curweapon.maxammo = 0;
        }
        else
        {
            Debug.Log("ź���� �� ���������ϴ�. �������� ���⸦ ��ų�, �ٸ� ���⸦ ã���ʽÿ�.");
        }

    }


}