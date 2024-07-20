using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build.Content;
using UnityEngine;

public class Manager : MonoBehaviour//½Ì±ÛÅæ ¸Å´ÏÀú
{
    static Manager instance;
    static GameManager gamemanager;
    static DataManager datamanager;
    //static SoundManager soundmanager;
    static UIManager uimanager;
    
    // property
    public GameManager Game { get { return gamemanager; } }
    public DataManager Data { get { return datamanager; } }
    //public SoundManager Sound { get { return soundmanager; } }
    public UIManager Ui { get { return uimanager; } }

    public static Manager GetInstance(){return instance;}


    private void Awake()
    {
        if(instance != null)
        {

            Destroy(this);
            Destroy(gameObject);

            return;
        }
        else
        {
            instance = this;
        }
          
    }
}
