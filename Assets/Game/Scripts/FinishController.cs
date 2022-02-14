using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FinishController : MonoBehaviour
{
    [SerializeField]private Transform player;
    [SerializeField]private Slider levelBarSlider;

    private float distance;
    // Start is called before the first frame update
    void Start()
    {
        distance = transform.position.z - player.transform.position.z;
        levelBarSlider.maxValue = distance;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        levelBarSlider.value = player.transform.position.z;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (GameManagement.Instance.CheckDressChest &&
                GameManagement.Instance.CheckDressLeg  &&
                GameManagement.Instance.CheckDressFoot)
            {
                GameManagement.Instance.WinTheGame();
            }
            else
            {
                GameManagement.Instance.LoseTheGame();
            }
        }
    }
}
