using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Figure
{
    public GameObject prefab;
    public int price;
}

public class GameManager : MonoBehaviour
{
    public GameObject currentGame;
    public Transform figureSpawnPoint;
    
    public int CurrentCoin { get; set; } = 0;

    public void StartGame(GameObject gameSet)
    {
        if (currentGame != null)
        {
            Destroy(currentGame);
        }
        currentGame = Instantiate(gameSet);
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
