using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    private int m_Score;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            m_Score = 0;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void AddScore(int score)
    {
        m_Score += score;
        Debug.Log(m_Score);
    }
}
