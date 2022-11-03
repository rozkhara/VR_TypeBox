using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using TMPro;

public class Surroundings : MonoBehaviour
{
    private readonly string[] leftkeyName = new string[4] { "QWERT", "qwert", "asdfg", "zxcv" };
    private readonly string[] rightkeyName = new string[4] { "OP", "yuiop", "hjkl", "bnm" };
    private const float distance = 0.7f;
    private const float angleVertical = 13f;
    private const float angleHorizontal = 13f;
    private InputManager IM = null;

    public bool isStarted = false;

    private GameObject go;
    private Quaternion lookDir;
    private List<List<GameObject>> leftKeyObjects = null;
    private List<List<GameObject>> rightKeyObjects = null;

    [SerializeField] private GameObject keyObject;

    private void Awake()
    {
        if (IM == null)
        {
            IM = GameObject.Find("InputManager").GetComponent<InputManager>();
        }
    }

    public void OnStartButtonClicked()
    {
        Initialize();
        isStarted = true;
    }

    private void Initialize()
    {
        leftKeyObjects = new();
        rightKeyObjects = new();
        char temp;
        for (int i = 0; i < 4; i++)
        {
            leftKeyObjects.Add(new List<GameObject>());
            rightKeyObjects.Add(new List<GameObject>());
            for (int j = leftkeyName[i].Length - 1; j >= 0; j--)
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
        SetNewPosRot();
    }

    private void Update()
    {
        if (!isStarted)
        {
            return;
        }
        if ((SteamVR_Input.GetStateDown("StickClick", SteamVR_Input_Sources.LeftHand) && SteamVR_Input.GetState("StickClick", SteamVR_Input_Sources.RightHand)) ||
            (SteamVR_Input.GetState("StickClick", SteamVR_Input_Sources.LeftHand) && SteamVR_Input.GetStateDown("StickClick", SteamVR_Input_Sources.RightHand)))
        {
            //Debug.Log("Both Buttons Pressed");
            SetNewPosRot();
        }
        if (SteamVR_Input.GetStateDown("LowerButtonLeft", SteamVR_Input_Sources.LeftHand))
        {
            IM.KeyDownHangul('B');
        }
        if (SteamVR_Input.GetStateDown("LowerButtonRight", SteamVR_Input_Sources.RightHand))
        {
            IM.KeyDownHangul('C');

        }

    }

    public void SetNewPosRot()
    {
        DirectionReset();
        Vector3 position = this.gameObject.transform.position;
        Vector3 translateVector = new(0f, 0f, distance);
        Quaternion newDir = lookDir;
        Vector3 newRight = Vector3.Cross(-this.gameObject.transform.forward, Vector3.up);
        foreach (List<GameObject> items in rightKeyObjects)
        {
            Quaternion basedir = newDir;
            newDir = Quaternion.AngleAxis(-angleVertical, newRight) * basedir;

            newDir = Quaternion.AngleAxis(angleHorizontal / 2, Vector3.up) * newDir;
            foreach (GameObject GO in items)
            {
                Vector3 newPos = newDir * translateVector + position;
                GO.transform.SetPositionAndRotation(newPos, Quaternion.LookRotation(newPos - this.gameObject.transform.position, Vector3.up));
                newDir = Quaternion.AngleAxis(angleHorizontal, Vector3.up) * newDir;
            }
            newDir = Quaternion.AngleAxis(angleVertical, newRight) * basedir;
        }
        newDir = lookDir;
        foreach (List<GameObject> items in leftKeyObjects)
        {
            Quaternion basedir = newDir;
            newDir = Quaternion.AngleAxis(-angleVertical, newRight) * basedir;

            newDir = Quaternion.AngleAxis(-angleHorizontal / 2, Vector3.up) * newDir;
            foreach (GameObject GO in items)
            {
                Vector3 newPos = newDir * translateVector + position;
                GO.transform.SetPositionAndRotation(newPos, Quaternion.LookRotation(newPos - this.gameObject.transform.position, Vector3.up));
                newDir = Quaternion.AngleAxis(-angleHorizontal, Vector3.up) * newDir;
            }
            newDir = Quaternion.AngleAxis(angleVertical, newRight) * basedir;
        }
    }

    private void DirectionReset()
    {
        Vector3 temp = this.gameObject.transform.eulerAngles;
        lookDir = Quaternion.Euler(new Vector3(temp.x, temp.y, 0f));
    }
}
