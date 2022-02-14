using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DressController : MonoBehaviour
{
     private List<DressData> chestDressData;
     private List<DressData> legDressData;
     private List<DressData> footDressData;
    
    [SerializeField] private List<DressData> allDressData;
    
    [SerializeField] private Toggle chestToggle;
    [SerializeField] private Toggle legToggle;
    [SerializeField] private Toggle footToggle;
    [SerializeField] private Animator anim ;

    private DressData underwearDressData;


    private DressStyles targetStyle;
    public  DressData OnChestDressData;
    public  DressData OnLegDressData;
    public  DressData OnFootDressData;
    
    [SerializeField]private DressData firstChestDressData;
    [SerializeField]private DressData firstLegDressData;
    [SerializeField]private DressData firstFootDressData;
    
    
    private void Start()
    {
        OnChestDressData.dressStyle = DressStyles.Underwear;
        OnLegDressData.dressStyle = DressStyles.Underwear;
        OnFootDressData.dressStyle = DressStyles.Underwear;
        
        targetStyle= GameManagement.Instance.TargetStyle;
        chestDressData = new List<DressData>();
        legDressData = new List<DressData>();
        footDressData = new List<DressData>();
        
        foreach (var dressData in allDressData)
        {
            switch (dressData.dressPart)
            {
                case DressParts.Chest:
                    chestDressData.Add(dressData);
                    break;
                case DressParts.Leg:
                    legDressData.Add(dressData);
                    break;
                case DressParts.Foot:
                    footDressData.Add(dressData);
                    break;
                default:
                    // code block
                    break;
            }
        }
    }

    private void ChestClear()
    {
        foreach (var part in chestDressData)
        {
            part.DressActive(false);
        }
        firstChestDressData.gameObject.SetActive(true);
    }
    private void LegClear()
    {
        foreach (var part in legDressData)
        {
            part.DressActive(false);
        }
        firstLegDressData.gameObject.SetActive(true);
    }
    private void FootClear()
    {
        foreach (var part in footDressData)
        {
            part.DressActive(false);
        }
        
    }

    public void DressUp(DressData dressData)
    {
        foreach (var staticDress in allDressData)
        {
            if (staticDress.dressStyle==dressData.dressStyle&&
                staticDress.dressPart==dressData.dressPart)
            {
                switch (dressData.dressPart)
                {
                    case DressParts.Chest:
                        ChestClear();
                        OnChestDressData=staticDress;
                        firstChestDressData.gameObject.SetActive(false);
                        break;
                    case DressParts.Leg:
                        LegClear();
                        OnLegDressData=staticDress;
                        firstLegDressData.gameObject.SetActive(false);
                        break;
                    case DressParts.Foot:
                        FootClear();
                        OnFootDressData=staticDress;
                        break;
                }
                staticDress.DressActive(true);
            }
        }
        CheckStyle();
        Destroy(dressData.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Dress"))
        {
            var triggerDress = other.GetComponent<DressData>();
            DressUp(triggerDress);
        }
        if (other.CompareTag("Obstacle"))
        {
            anim.SetTrigger("Jogging");
            var dropedDress = false;
            var randomDressPart = Random.Range(0, 3);
            while (!dropedDress)
            {
                if (OnChestDressData.dressStyle == DressStyles.Underwear&&OnLegDressData.dressStyle == DressStyles.Underwear&&OnFootDressData.dressStyle == DressStyles.Underwear)
                {
                    break;
                }
                if (randomDressPart==0&& OnChestDressData.dressStyle != DressStyles.Underwear)
                {
                    ChestClear();
                    OnChestDressData = firstChestDressData;
                    dropedDress = true;
                }

                if (randomDressPart==1&& OnLegDressData.dressStyle != DressStyles.Underwear)
                {
                    LegClear();
                    OnLegDressData = firstLegDressData;
                    dropedDress = true;
                }

                if (randomDressPart==2&& OnFootDressData.dressStyle != DressStyles.Underwear)
                {
                    FootClear();
                    OnFootDressData = firstFootDressData;
                    dropedDress = true;
                }
                randomDressPart = Random.Range(0, 3);
            }
           
            // Debug.Log("Trigger obstacle");
            // Debug.Log(randomDressPart);
            // switch (randomDressPart)
            // {
            //     case 0:
            //         ChestClear();
            //         OnChestDressData = firstChestDressData;
            //         break;
            //     case 1 :
            //         LegClear();
            //         OnLegDressData = firstLegDressData;
            //         break;
            //     case 2:
            //         OnFootDressData = firstFootDressData;
            //         FootClear();
            //         break;
            // }
            CheckStyle();
          //  Destroy(other.gameObject);
        }
    }

    private void CheckStyle()
    {
        chestToggle.isOn = OnChestDressData.dressStyle==targetStyle;
        legToggle.isOn = OnLegDressData.dressStyle==targetStyle;
        footToggle.isOn = OnFootDressData.dressStyle==targetStyle;

        GameManagement.Instance.CheckDressChest = chestToggle.isOn;
        GameManagement.Instance.CheckDressLeg = legToggle.isOn;
        GameManagement.Instance.CheckDressFoot = footToggle.isOn;
    }
}
