using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR.InteractionSystem;

public class ButtonsInteraction : MonoBehaviour
{
    private void OnHandHoverBegin()
    {
        if (this.gameObject.name.Contains("PadKey"))
        {
            Debug.Log("PadKey interacted");
            //SceneManager.LoadScene("SY");
        }
        else if (this.gameObject.name.Contains("PlusKey"))
        {
            Debug.Log("PlusKey interacted");
            //SceneManager.LoadScene("SY_Copy");
        }
        else if (this.gameObject.name.Contains("SceneStartButton"))
        {
            if (SceneManager.GetActiveScene().name.Contains("Copy"))
            {
                GameObject.Find("HeadCollider").GetComponent<KeyGen_Cross>().OnStartButtonClicked();
                Managers.SoundManager.Instance.PlaySFXSound("BoxingBell1");
                Destroy(this.gameObject);
            }
        }
        else
        {
            Debug.LogError("Undefined");
        }
    }
}
