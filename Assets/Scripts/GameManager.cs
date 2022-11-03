using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public int Score { get; set; }

    public TextMeshProUGUI timerText = null;

    private float timer = 60;

    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
        if (SceneManager.GetActiveScene().name.Contains("Copy"))
        {
            GameObject.Find("HeadCollider").GetComponent<KeyGen_Cross>().OnStartButtonClicked();
        }
    }
    private void OnDestroy()
    {
        Instance = null;
    }

    // Update is called once per frame
    private void Update()
    {
        timer -= Time.deltaTime;

        timerText.text = timer.ToString("F2");

        if (timer < 0f)
        {
            timerText.text = "0.00";

            Time.timeScale = 0f;
            // 게임오버
        }
    }
}
