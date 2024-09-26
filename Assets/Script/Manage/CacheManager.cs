using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CacheManager : MonoBehaviour
{
    static CacheManager instance;
    public static CacheManager Inst => instance;

    public WaitForSeconds cacheWFS = new WaitForSeconds(0.1f);
    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }




}
