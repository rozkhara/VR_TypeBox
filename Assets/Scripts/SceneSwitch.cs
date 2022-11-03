using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitch : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject.Find("HeadCollider").GetComponent<StartSceneButton>().enabled = true;
        SceneManager.LoadScene("StartScene");
    }
}
