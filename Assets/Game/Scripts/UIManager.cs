using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    #region Singleton
    public static UIManager Instance { get { return instance; } }
    private static UIManager instance;

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

    [SerializeField] GameObject TapToStartPanel;
    [SerializeField] GameObject GameInPanel;
    [SerializeField] GameObject WinPanel;
    [SerializeField] GameObject LosePanel;
    [SerializeField] TextMeshProUGUI dressStyleGUI;
    
    public void StartTheGame()
    {
        GameManagement.Instance.StartTheGame();

        TapToStartPanel.SetActive(false);

        GameInPanel.SetActive(true);

        //dressStyleGUI.text = GameManagement.Instance.TargetStyle.ToString();
    }

    public void WinTheGame()
    {
     //   GameManagement.Instance.WinTheGame();

        WinPanel.SetActive(true);

        GameInPanel.SetActive(false);
    }

    public void LoseTheGame()
    {
     //   GameManagement.Instance.LoseTheGame();

        LosePanel.SetActive(true);

        GameInPanel.SetActive(false);
    }

    public void NextLevel()
    {
        SceneManager.LoadScene("Level2");
    }

    public void TryAgain()
    {
        SceneManager.LoadScene(sceneBuildIndex: 0);
    }
}
