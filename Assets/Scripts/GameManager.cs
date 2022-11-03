using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private const float value = 10f;

    private GameObject Go;
    public GameObject homeButton = null;

    private KeyGen_Cross KG = null;
    private Surroundings SR = null;

    public bool IsGameOver { get; set; } = false;

    public int Score { get; set; }

    public TextMeshProUGUI timerText = null;

    public float Timer { get; set; } = value;

    // Start is called before the first frame update
    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
        Go = GameObject.Find("HeadCollider");

        homeButton.SetActive(false);

        if (SceneManager.GetActiveScene().name.Contains("Copy"))
        {
            KG = Go.GetComponent<KeyGen_Cross>();
        }
        else if (SceneManager.GetActiveScene().name == "SY")
        {
            SR = Go.GetComponent<Surroundings>();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (IsGameOver)
        {
            return;
        }
        if (KG != null)
        {
            if (!KG.isStarted)
            {
                return;
            }
        }
        else if (SR != null)
        {
            if (!SR.isStarted)
            {
                return;
            }
        }
        Timer -= Time.deltaTime;

        timerText.text = Timer.ToString("F2");

        if (Timer <= 0f)
        {
            timerText.text = "0.00";
            SoundManager.Instance.PlaySFXSound("BoxingBell3");

            Debug.Log("Hi!");

            IsGameOver = true;

            homeButton.SetActive(true);

            if (SR != null)
            {
                foreach (var go in SR.LeftKeyObjects.SelectMany(x => x).ToList())
                {
                    go.SetActive(false);
                }

                foreach (var go in SR.RightKeyObjects.SelectMany(x => x).ToList())
                {
                    go.SetActive(false);
                }
            }

            if (KG != null)
            {
                foreach (var go in KG.FlatItemListLeft)
                {
                    go.SetActive(false);
                }

                foreach (var go in KG.FlatItemListRight)
                {
                    go.SetActive(false);
                }
            }
        }
    }
}
