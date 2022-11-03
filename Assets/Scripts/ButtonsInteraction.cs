using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR.InteractionSystem;

public class ButtonsInteraction : MonoBehaviour
{
    private GameObject _HC;

    private void Start()
    {
        _HC = GameObject.Find("HeadCollider");
    }

    private void OnHandHoverBegin()
    {
        if (this.gameObject.name.Contains("PadKey"))
        {
            //Debug.Log("PadKey interacted");
            _HC.GetComponent<Surroundings>().enabled = true;
            _HC.GetComponent<StartSceneButton>().enabled = false;
            SceneManager.LoadScene("SY");
        }
        else if (this.gameObject.name.Contains("PlusKey"))
        {
            //Debug.Log("PlusKey interacted");
            _HC.GetComponent<KeyGen_Cross>().enabled = true;
            _HC.GetComponent<StartSceneButton>().enabled = false;
            SceneManager.LoadScene("SY_Copy");
        }
        else if (this.gameObject.name.Contains("SceneStartButton"))
        {
            if (SceneManager.GetActiveScene().name.Equals("SY_Copy"))
            {
                _HC.GetComponent<KeyGen_Cross>().OnStartButtonClicked();
            }
            else if (SceneManager.GetActiveScene().name.Equals("SY"))
            {
                _HC.GetComponent<Surroundings>().OnStartButtonClicked();
            }
            SoundManager.Instance.PlaySFXSound("BoxingBell1");
            Destroy(this.gameObject);
        }
        else if (this.gameObject.name.Contains("HomeKey"))
        {
            GameManager.Instance.homeButton.SetActive(false);
            _HC.GetComponent<StartSceneButton>().enabled = true;
            _HC.GetComponent<KeyGen_Cross>().enabled = false;
            _HC.GetComponent<Surroundings>().enabled = false;
            SceneManager.LoadScene("StartScene");
        }
        else
        {
            Debug.LogError("Undefined");
        }
    }
}
