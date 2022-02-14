using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagement : MonoBehaviour
{
    #region Singleton
    private static GameManagement instance;
    public static GameManagement Instance { get { return instance; } }

    public DressStyles TargetStyle;

    public bool CheckDressChest;
    public bool CheckDressLeg;
    public bool CheckDressFoot;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    #endregion

    #region GameStates
    public enum GameStates
    {
        INMENU,
        START,
        WIN,
        LOSE
    }
    public GameStates GameState { get { return gameState; } }
    private GameStates gameState = GameStates.INMENU;

    [SerializeField] Transform kissTransform;

    public void StartTheGame()
    {
        gameState = GameStates.START;

        PlayerController.Instance.StartRun();
    }

    public void WinTheGame()
    {
        gameState = GameStates.WIN;

        PlayerController.Instance.WinTheGame(kissTransform);

        UIManager.Instance.WinTheGame();
    }

    public void LoseTheGame()
    {
        gameState = GameStates.LOSE;
        PlayerController.Instance.LoseTheGame();

        UIManager.Instance.LoseTheGame();
    }
    #endregion

    private int money;

    public void EarnMoney(int moneyValue)
    {
        money += moneyValue;
        Debug.Log(money);
    }


    //Player enter the gameendarea position
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            WinTheGame();

            UIManager.Instance.WinTheGame();
        }
    }
}
