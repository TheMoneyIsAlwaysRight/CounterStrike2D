using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : MonoBehaviour, IDamagable
{
    enum BelongTeam
    {
        T,
        CT
    }
    [SerializeField] bool T;
    [SerializeField] string Team;
    [SerializeField] public int hp; //체력
    [SerializeField] public int ap; //방탄복 수치
    [SerializeField] protected int money;  //갖고 있는 돈 
    [SerializeField] FIRETRACKING firetrack; //총알의 궤적
    [SerializeField] Animator animator;
    [SerializeField] GameObject FireFlash;
    [SerializeField] GameObject FireFlash2;
    [SerializeField] public WeaponManager weaponmanager;
    [SerializeField] AudioClip reloadsound;
    [SerializeField] AudioClip reloadsound2;
    [SerializeField] public AudioSource audiosource;
    public bool IsReloading { get; private set; }
    public bool IsRecoil { get; private set; }

    Coroutine reloadcoroutine;
    Coroutine recoilcoroutine;
    float reloadcooltime;
    bool IsReload;
    /*
     *[0] : Main Weapon 
     *[1] : Sub Weapon
     *[2] : Knife
     *[3]~[4] : Grenades
     *[5] : C4 bomb
     **/
    private void Awake()
    {
        if(T)
        {
            gameObject.tag = "T";
            Team = "Terrorist";
        }
        else
        {
            gameObject.tag = "CT";
            Team = "Counter_Terrorist";
        }
    }

    private void FixedUpdate()
    {
        Hpcheck();
    }
    protected void Hpcheck()
    {
        if(hp<=0)
        {
            Die();
        }
    }

    protected void Die()
    {
        Destroy(gameObject);
    }

    public IEnumerator RecoilCoroutine()
    {
        IsRecoil = true;
        yield return new WaitForSeconds(weaponmanager.curweapon.firecooltime);
        IsRecoil = false;
        //audiosource.Play(reloadsound);
    }

    public void Fire()
    {
        Weapon curweapon = transform.gameObject.GetComponentInChildren<WeaponManager>().curweapon;
        if (IsRecoil)
        { //발사 쿨타임 동안 제외
            return;
        }
        recoilcoroutine = StartCoroutine(RecoilCoroutine());
        if(curweapon == transform.gameObject.GetComponentInChildren<WeaponManager>().HAND[4])
        { //폭탄 제외
            return;
        }
        if (curweapon == transform.gameObject.GetComponentInChildren<WeaponManager>().HAND[2])
        {
            animator.SetBool("Knife", true);
            Vector3 fireDir = transform.up;
            firetrack.gameObject.SetActive(true);
            curweapon.magazine--;
            animator.SetBool("Knife", false);
        }
        else if (curweapon.magazine > 0)
        {         
            float bulletfireRandom;
            animator.SetBool("Fire", true);
            Vector3 fireDir = transform.up;
            firetrack.gameObject.SetActive(true);
            bulletfireRandom = Random.value;
            if (bulletfireRandom >= 0.5)
            {
                FireFlash.SetActive(true);
            }
            else
            {
                FireFlash2.SetActive(true);
            }
            curweapon.magazine--;
            animator.SetBool("Fire", false);
            audiosource.Play();
        }

    }
    public void Reload(Weapon curWeapon)
    {
        if (IsReloading || curWeapon == null)
        {
            return; // 이미 재장전 중이거나 무기가 없는 경우 재장전을 시작하지 않음
        }
        IsReloading = true;
        animator.SetBool("Reload", true);
        StartCoroutine(ReloadCoroutine(curWeapon));
    }

    private IEnumerator ReloadCoroutine(Weapon curWeapon)
    {
        yield return new WaitForSeconds(curWeapon.reloadtime);

        int bulletsNeeded = curWeapon.magazineCapacity - curWeapon.magazine;

        if (curWeapon.maxammo >= bulletsNeeded)
        {
            curWeapon.maxammo -= bulletsNeeded;
            curWeapon.magazine += bulletsNeeded;
        }
        else
        {
            curWeapon.magazine += curWeapon.maxammo;
            curWeapon.maxammo = 0;
           
        }
        animator.SetBool("Reload", false);
        IsReloading = false;
    }
}
