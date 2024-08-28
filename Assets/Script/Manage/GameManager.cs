using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    static GameObject[] TCount;
    static GameObject[] CTCount;

    static List<GameObject> tcount;
    static List<GameObject> ctcount;
    [SerializeField] public GameObject CTWIN;
    [SerializeField] public GameObject TWIN;
    public static GameManager gameManager;


    public static GameManager Game { get { return gameManager; } }

    public void Awake()
    { 

        if (gameManager != null)
        {
            Destroy(gameObject);
        }
        else
        {
            gameManager = this;
        }
    }


    void FixedUpdate()
    {
        CountSurvivor();
    }
    public void TerrorWin()
    {
        TWIN.SetActive(true);
        GamePause();
    }
    public void CounterWIN()
    {
        CTWIN.SetActive(true);
        GamePause();
    }
    void CountSurvivor()
    {
        TCount = GameObject.FindGameObjectsWithTag("T");
        CTCount = GameObject.FindGameObjectsWithTag("CT");
        tcount = TCount.ToList<GameObject>();
        ctcount = CTCount.ToList<GameObject>();
        if(tcount.Count <= 0)
        {
            CounterWIN();
        }
        else if (ctcount.Count <= 0)
        {
            TerrorWin();
        }
    }

   public static void GamePause()
   {
        Time.timeScale = 0f;
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1f;
    }
   public static void GameResume()
   {
        Time.timeScale = 1f;
   }
}
