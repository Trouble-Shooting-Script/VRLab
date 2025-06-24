using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class Figure
{
    public GameObject prefab;
    public int price;
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Transform xrOrigin;
    public GameObject gameSelectionPanel;
    public GameObject currentGame;
    public TMP_Text coinText;
    public Transform figureSpawnPoint;

    private int m_Coin = 0;
    public int CurrentCoin
    {
        get => m_Coin;
        set
        {
            coinText.text = value.ToString();
            m_Coin = value;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void StartGame(GameObject gameSet)
    {
        StartGame(gameSet, xrOrigin.position, xrOrigin.rotation);
    }

    private void StartGame(GameObject gameSet, Vector3 position, Quaternion rotation)
    {
        if (currentGame != null)
        {
            Debug.Log("Destroying current game: " + currentGame.name);
            Destroy(currentGame);
        }
        currentGame = Instantiate(gameSet, position, rotation);
        gameSelectionPanel.SetActive(false);
    }

    public void Buy(Figure figure)
    {
        if (figure.price > CurrentCoin)
        {
            return;
        }
        CurrentCoin -= figure.price;
        Instantiate(figure.prefab, figureSpawnPoint.position, figureSpawnPoint.rotation);
    }
}
