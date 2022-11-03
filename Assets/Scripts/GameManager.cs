using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private GameObject Go;
    public GameObject homeButton = null;

    private KeyGen_Cross KG = null;
    private Surroundings SR = null;

    private bool isGameOver = false;

    public int Score { get; set; }

    public TextMeshProUGUI timerText = null;

    private float timer = 10;

    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
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
    private void OnDestroy()
    {
        Instance = null;
    }

    // Update is called once per frame
    private void Update()
    {
        if (isGameOver)
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
        timer -= Time.deltaTime;

        timerText.text = timer.ToString("F2");

        if (timer < 0f)
        {
            timerText.text = "0.00";
            Managers.SoundManager.Instance.PlaySFXSound("BoxingBell3");

            Time.timeScale = 0f;
            isGameOver = true;

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
