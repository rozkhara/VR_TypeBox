using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using TMPro;

public class Surroundings : MonoBehaviour
{
    private string[] leftkeyName = new string[4] {"QWERT", "qwert", "asdfg", "zxcv"};
    private string[] rightkeyName = new string[4] { "BSCOP", "yuiop", "hjkl", "bnm"};


    private GameObject go;
    private Quaternion lookDir;
    private const float distance = 0.6f;
    private List<List<GameObject>> leftKeyObjects = null;
    private List<List<GameObject>> rightKeyObjects = null;

    [SerializeField] private GameObject keyObject;

    private void Start()
    {
        leftKeyObjects = new();
        rightKeyObjects = new();
        char temp;
        for (int i = 0; i < 4; i++)
        {
            leftKeyObjects.Add(new List<GameObject>());
            rightKeyObjects.Add(new List<GameObject>());
            for (int j = leftkeyName[i].Length - 1; j >=0; j--)
            {
                go = Instantiate(keyObject);
                temp = leftkeyName[i][j];
                go.name = temp.ToString();
                go.GetComponentInChildren<TextMeshPro>().text = AutomateKR.SOUND_TABLE[AutomateKR.HANGULE_KEY_TABLE[temp]].ToString();
                leftKeyObjects[i].Add(go);
            }
            for (int j = 0; j < rightkeyName[i].Length; j++)
            {
                go = Instantiate(keyObject);
                temp = rightkeyName[i][j];
                go.name = temp.ToString();
                switch (temp)
                {
                    case 'B':
                    case 'S':
                    case 'C':
                        go.GetComponentInChildren<TextMeshPro>().text = temp.ToString();
                        break;
                    default:
                        go.GetComponentInChildren<TextMeshPro>().text = AutomateKR.SOUND_TABLE[AutomateKR.HANGULE_KEY_TABLE[temp]].ToString();
                        break;
                }
                rightKeyObjects[i].Add(go);
            }
        }
        lookDir = this.gameObject.transform.rotation;
    }
    private void Update()
    {

        if ((SteamVR_Input.GetState("LowerButtonLeft", SteamVR_Input_Sources.LeftHand) && SteamVR_Input.GetStateDown("LowerButtonRight", SteamVR_Input_Sources.RightHand)) ||
            (SteamVR_Input.GetStateDown("LowerButtonLeft", SteamVR_Input_Sources.LeftHand) && SteamVR_Input.GetState("LowerButtonRight", SteamVR_Input_Sources.RightHand)))
        {
            DirectionReset();
            //Debug.Log("Both Buttons Pressed");
        }

        SetNewPosRot();
    }

    private void SetNewPosRot()
    {
        Vector3 position = this.gameObject.transform.position;
        Vector3 translateVector = new Vector3(0f, 0f, distance);
        Quaternion newDir = lookDir;
        foreach (List<GameObject> items in rightKeyObjects)
        {
            Quaternion basedir = newDir;
            foreach (GameObject GO in items)
            {
                newDir = Quaternion.AngleAxis(15f, Vector3.up) * newDir;
                Vector3 newPos = newDir * translateVector + position;
                GO.transform.SetPositionAndRotation(newPos, Quaternion.LookRotation(newPos - this.gameObject.transform.position, Vector3.up));
            }
            newDir = Quaternion.AngleAxis(15f, Vector3.right) * basedir;
        }
        newDir = lookDir;
        foreach (List<GameObject> items in leftKeyObjects)
        {
            Quaternion basedir = newDir;
            foreach (GameObject GO in items)
            {
                newDir = Quaternion.AngleAxis(-15f, Vector3.up) * newDir;
                Vector3 newPos = newDir * translateVector + position;
                GO.transform.SetPositionAndRotation(newPos, Quaternion.LookRotation(newPos - this.gameObject.transform.position, Vector3.up));
            }
            newDir = Quaternion.AngleAxis(15f, Vector3.right) * basedir;
        }
    }

    private void DirectionReset()
    {
        lookDir = this.gameObject.transform.rotation;
    }


}
