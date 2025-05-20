using FMOD;
using System.Collections;
using UnityEngine;

public class ScrollHUD : MonoBehaviour
{
    // Icons
    [SerializeField] private GameObject bookLeft; // State 1
    [SerializeField] private GameObject bookRight; // State 0
    [SerializeField] private GameObject lighterLeft; // State 0
    [SerializeField] private GameObject lighterRight; // State 2
    [SerializeField] private GameObject handLeft; // State 2
    [SerializeField] private GameObject handRight; // State 1

    [SerializeField] private GameObject tabKeysIcon;
    [SerializeField] private GameObject tabHandsIcon;

    [SerializeField] private GameObject key1;
    [SerializeField] private GameObject key1indicator;
    [SerializeField] private GameObject key2;

    private int normalState = 0;
    private int keysState = 0;
    private bool inKeyMenu = false; // false = normal, true = keys
    private bool tabEnabled = false; // false = 0 keys, true = > 0 keys
    [HideInInspector] public int keyCount = 0;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Equals)) UpdateKeys(1);
        if (Input.GetKeyDown(KeyCode.Minus)) UpdateKeys(-1);
        if (keyCount > 0)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (inKeyMenu)
                {
                    inKeyMenu = false;
                    DisableAllKeys();
                    EnableCorrectIcons();
                    tabHandsIcon.SetActive(false);
                    tabKeysIcon.SetActive(true);
                }
                else if (!inKeyMenu)
                {
                    inKeyMenu = true;
                    DisableAllIcons();
                    EnableCorrectKeys();
                    tabHandsIcon.SetActive(true);
                    tabKeysIcon.SetActive(false);
                }
            }
        }

        if (!inKeyMenu)
        {
            int oldState = normalState;
            if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                normalState += 1;
                if (normalState >= 3) normalState = 0;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                normalState -= 1;
                if (normalState <= -1) normalState = 2;
            }
            if (normalState != oldState)
            {
                EnableCorrectIcons();
            }
        }
        else
        {
            int oldState = keysState;
            if (keyCount >= 2)
            {
                if (Input.GetAxis("Mouse ScrollWheel") > 0)
                {
                    keysState += 1;
                    if (keysState >= 2) keysState = 0;
                }
                else if (Input.GetAxis("Mouse ScrollWheel") < 0)
                {
                    keysState -= 1;
                    if (keysState <= -1) keysState = 1;
                }
                if (keysState != oldState)
                {
                    EnableCorrectKeys();
                }
            }
        }
    }
    private void DisableAllIcons()
    {
        bookLeft.SetActive(false);
        bookRight.SetActive(false);
        lighterLeft.SetActive(false);
        lighterRight.SetActive(false);
        handLeft.SetActive(false);
        handRight.SetActive(false);
    }
    private void EnableCorrectIcons()
    {
        switch (normalState)
        {
            case 0:
                DisableAllIcons();
                bookRight.SetActive(true);
                lighterLeft.SetActive(true);
                break;
            case 1:
                DisableAllIcons();
                bookLeft.SetActive(true);
                handRight.SetActive(true);
                break;
            case 2:
                DisableAllIcons();
                handLeft.SetActive(true);
                lighterRight.SetActive(true);
                break;
        }
    }

    private void DisableAllKeys()
    {
        key1.SetActive(false);
        key1indicator.SetActive(false);
        key2.SetActive(false);
    }
    private void EnableCorrectKeys()
    {
        UnityEngine.Debug.Log("Keys: " + keysState);
        if (keyCount < 2) key1indicator.SetActive(false);
        if (keyCount == 2) key1indicator.SetActive(true);
        switch (keysState)
        {
            case 0:
                key1.SetActive(true);
                key2.SetActive(false);
                break;
            case 1:
                key1.SetActive(false);
                key2.SetActive(true);
                break;
        }
    }

    private void UpdateKeys(int pValue)
    {
        keyCount += pValue;
        if (keyCount < 0) keyCount = 0;
        if (keyCount > 2) keyCount = 2;
        if (!tabEnabled && keyCount >= 1) tabEnabled = true;
        if (keyCount == 0) tabKeysIcon.SetActive(false);
        if (keyCount == 1) tabKeysIcon.SetActive(true);
        if(inKeyMenu && keyCount >= 2) key1indicator.SetActive(true);
    }

}
