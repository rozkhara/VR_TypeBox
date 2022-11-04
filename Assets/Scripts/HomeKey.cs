using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HomeKey : MonoBehaviour
{
    public static HomeKey Instance { get; private set; }

    // Start is called before the first frame update
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            GameManager.Instance.timerText = GameObject.Find("Timer").GetComponent<TextMeshProUGUI>();
            Destroy(this.gameObject);
        }
    }
}
