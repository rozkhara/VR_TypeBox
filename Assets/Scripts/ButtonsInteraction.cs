using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR.InteractionSystem;

public class ButtonsInteraction : MonoBehaviour
{
    private void OnHandHoverBegin()
    {
        if (this.gameObject.name == "PadKey(Clone)")
        {
            Debug.Log("PadKey interacted");
            //SceneManager.LoadScene("SY");
        }
        else if(this.gameObject.name == "PlusKey(Clone)")
        {
            Debug.Log("PlusKey interacted");
            //SceneManager.LoadScene("SY_Copy");
        }
        else
        {
            Debug.LogError("Undefined");
        }
    }
}
