using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.IO.LowLevel.Unsafe;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponManager : MonoBehaviour
{
    [SerializeField] Player user;
    [SerializeField] Weapon[] HAND = new Weapon[5]; //현재 가진 무기 목록
    [SerializeField] Dictionary<int,Weapon> WeaponInfo = new Dictionary<int,Weapon>(); // 모든 무기는 무기번호로 지정됨.
    [SerializeField] GameObject droppoint;
    [SerializeField] public Weapon curweapon; //현재 무기
    bool weaponcooltime;
   

    Coroutine firecoroutine;
    Coroutine reloadcoroutine;

    void FirstSetting() //시작 시 모든 무기 비활성화+ 칼 무기로 시작
    {
        Weapon[] Weapons = gameObject.GetComponentsInChildren<Weapon>(); //자식 아래의 모든 무기 정보들을 딕셔너리에 추가.

        for (int x = 0; x < Weapons.Length; x++)
        {
            WeaponInfo.Add(Weapons[x].GetComponent<Weapon>().weaponnumber, Weapons[x]);
        }

        HAND[2] = WeaponInfo[50]; // weapon number 50 -> knife
        Weapon[] setweapon = gameObject.GetComponentsInChildren<Weapon>();

        foreach(Weapon a in setweapon) //모든 무기 비활성화
        {
            a.gameObject.SetActive(false);
        }

        curweapon = HAND[2];

        curweapon.gameObject.SetActive(true); //칼 무기만 활성화.

    }

    void Start()
    {
        FirstSetting();
    }

    public IEnumerator FireCoroutine(Weapon curweapon)
    {
        while (true)
        {
            if (user != null)
            {
                user.Fire(curweapon);
            }
            else
            {
                Debug.Log($"{gameObject.transform.parent.gameObject.name}가 웨폰컨테이너를 사용할 수 없습니다.");
            }

            yield return new WaitForSeconds(curweapon.firecooltime);

        }
    }
    public IEnumerator ReloadCoroutine(Weapon curweapon)
    {
        //animator.SetBool("Reload", true);
        yield return new WaitForSeconds(curweapon.reloadtime);
        //animator.SetBool("Reload", false);
    }

    void OnFire(InputValue value)
    {

        if (value.isPressed)
        {
            if(curweapon != null)
            {
                firecoroutine = StartCoroutine(FireCoroutine(curweapon));               
            }
        }
        else
        {
            StopCoroutine(firecoroutine);
        }

    }

    void OnReload()
    {
        if (user != null)
        {

            bool IsReload = user.Reload(curweapon);
            if (IsReload) //재장전에 성공하면
            { reloadcoroutine = StartCoroutine(ReloadCoroutine(curweapon)); }
        }
    }



    public void PickUpWeapon(GameObject pickupweapon)
    {
        int number = pickupweapon.GetComponent<item>().weaponnumber;

        switch (pickupweapon.GetComponent<item>().weaponstyle)
        {
            case 0:
                if (HAND[0] == null)
                {
                    HAND[0] = WeaponInfo[number];
                    Destroy(pickupweapon);
                }
                break;
            case 1:
                if (HAND[1] == null)
                {
                    HAND[1] = WeaponInfo[number];
                    Destroy(pickupweapon);
                }
                break;
            case 3:
                if (HAND[3] == null)
                {
                    HAND[3] = WeaponInfo[number];
                    Destroy(pickupweapon);
                }
                break;
            case 4:
                if (HAND[4] == null)
                {
                    HAND[4] = WeaponInfo[number];
                    Destroy(pickupweapon);
                }
                break;
        }
    }
    void OnDropWeapon(InputValue value)
    {
        DropWeapon();
    }
    void DropWeapon()
    {
        if (curweapon != HAND[2])
        {
            curweapon.gameObject.SetActive(false);

            GameObject dropitem = curweapon.GetComponent<Weapon>().dropPrefab;

            Instantiate(dropitem, droppoint.transform.position, transform.rotation);

            for (int x = 0; x < HAND.Length; x++) //무기를 버린 뒤 현재 무기 목록에서 지움.
            {
                if (curweapon == HAND[x])
                {
                    HAND[x] = null;
                }
            }
            curweapon = HAND[2];
            curweapon.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("이 무기는 버릴 수 없다.");
        }

    }
    void OnRifle(InputValue button)
    {

        if (HAND[0] != null)
        {
           curweapon.gameObject.SetActive(false);
           curweapon = HAND[0];
           curweapon.gameObject.SetActive(true);
        }

    }
    void OnPistol(InputValue button)
    {

        if (HAND[1] != null)
        {
            curweapon.gameObject.SetActive(false);
            curweapon = HAND[1];
            curweapon.gameObject.SetActive(true);
        }

    }
    void OnMelee(InputValue button)
    {

        if (HAND[2] != null)
        {
            curweapon.gameObject.SetActive(false);
            curweapon = HAND[2];
            curweapon.gameObject.SetActive(true);
        }

    }
    void OnGrenade(InputValue button)
    {

        if (HAND[3] != null)
        {
            curweapon.gameObject.SetActive(false);
            curweapon = HAND[3];
            curweapon.gameObject.SetActive(true);
        }

    }
    void OnBomb(InputValue button)
    {

        if (HAND[4] != null)
        {
            curweapon.gameObject.SetActive(false);
            curweapon = HAND[4];
            curweapon.gameObject.SetActive(true);
        }

    }
}
