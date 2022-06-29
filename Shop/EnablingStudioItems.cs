using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnablingStudioItems : MonoBehaviour
{
    [SerializeField] private GameObject studioItem = null;
    [SerializeField] private GameObject[] disableItems = null;

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("StudioItem" + studioItem.name))
        {
            PlayerPrefs.SetInt("StudioItem" + studioItem.name, 2);
        }
        else
        {
            if (PlayerPrefs.GetInt("StudioItem" + studioItem.name) == 1)
            {
                studioItem.SetActive(true);
            }
        }

        if (PlayerPrefs.GetInt("StudioItem" + studioItem.name) == 0)
        {
            studioItem.SetActive(false);
        }
    }

    public void EnablingItem()
    {
        studioItem.SetActive(true);
        PlayerPrefs.SetInt("StudioItem" + studioItem.name, 1);

        for (int i = 0; i < disableItems.Length; i++)
        {
            disableItems[i].SetActive(false);
            PlayerPrefs.SetInt("StudioItem" + disableItems[i].name, 0);
        }
    }
}
