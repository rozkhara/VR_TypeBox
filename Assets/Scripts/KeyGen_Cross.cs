using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using TMPro;

public class KeyGen_Cross : MonoBehaviour
{
    private readonly string[] leftkeyName = new string[4] { "QWERT", "qwert", "asdfg", "zxcv" };
    private readonly string[] rightkeyName = new string[4] { "BSCOP", "yuiop", "hjkl", "bnm" };
    private const float distance = 0.7f;
    private const float angleVertical = 13f;
    private const float angleHorizontal = 13f;
    private const float itemAngle = 45f;

    private GameObject go;
    private Quaternion lookDir;
    private List<List<GameObject>> leftKeyObjects = null;
    private List<List<GameObject>> rightKeyObjects = null;
    private List<GameObject> flatItemList = null;
    private List<int> ranNumList = null;
    private bool isKeyInstantiated = false;
    private direction curDirection = direction.front;
    private Vector3 objectPos = new();

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
            for (int j = leftkeyName[i].Length - 1; j >= 0; j--)
            {
                go = Instantiate(keyObject);
                temp = leftkeyName[i][j];
                go.name = temp.ToString();
                go.GetComponentInChildren<TextMeshPro>().text = AutomateKR.SOUND_TABLE[AutomateKR.HANGULE_KEY_TABLE[temp]].ToString();
                go.SetActive(false);
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
                go.SetActive(false);
                rightKeyObjects[i].Add(go);
            }
        }
        //SetNewPosRot();
    }
    private void Update()
    {
        if ((SteamVR_Input.GetState("LowerButtonLeft", SteamVR_Input_Sources.LeftHand) && SteamVR_Input.GetStateDown("LowerButtonRight", SteamVR_Input_Sources.RightHand)) ||
            (SteamVR_Input.GetStateDown("LowerButtonLeft", SteamVR_Input_Sources.LeftHand) && SteamVR_Input.GetState("LowerButtonRight", SteamVR_Input_Sources.RightHand)))
        {
            //Debug.Log("Both Buttons Pressed");
            SetNewPosRot();
        }
        GetDPadInput();
    }

    private void SetNewPosRot()
    {
        DirectionReset();
        Vector3 position = this.gameObject.transform.position;
        Vector3 translateVector = new(0f, 0f, distance);
        Quaternion newDir = lookDir;
        Instantiate_CrossKeys(position, translateVector, newDir);
        RotateObjectItems(position, translateVector, newDir);
    }

    private void Instantiate_CrossKeys(Vector3 position, Vector3 translateVector, Quaternion newDir)
    {
        if (isKeyInstantiated)
        {
            return;
        }
        flatItemList = leftKeyObjects.SelectMany(x => x).ToList();
        ranNumList = new List<int>();
        int ranNum;
        for (int i = 0; i < 5; i++)
        {
            do { ranNum = Random.Range(0, flatItemList.Count); }
            while (ranNumList.Contains(ranNum));
            ranNumList.Add(ranNum);
        }
        for (int i = 0; i < 5; i++)
        {
            flatItemList[ranNumList[i]].SetActive(true);
        }
        isKeyInstantiated = true;
    }

    private void RotateObjectItems(Vector3 position, Vector3 translateVector, Quaternion newDir)
    {
        objectPos = newDir * translateVector + position;
        Quaternion baseObjDir = newDir;
        Quaternion objectDir = baseObjDir;
        Vector3 objectTVector = new(0f, 0f, -0.15f);
        for (int i = 0; i < 5; i++)
        {
            switch ((direction)i)
            {
                case direction.front:
                    break;
                case direction.left:
                    objectDir = Quaternion.AngleAxis(itemAngle, flatItemList[ranNumList[0]].transform.up) * baseObjDir;
                    break;
                case direction.right:
                    objectDir = Quaternion.AngleAxis(-itemAngle, flatItemList[ranNumList[0]].transform.up) * baseObjDir;
                    break;
                case direction.up:
                    objectDir = Quaternion.AngleAxis(itemAngle, flatItemList[ranNumList[0]].transform.right) * baseObjDir;
                    break;
                case direction.down:
                    objectDir = Quaternion.AngleAxis(-itemAngle, flatItemList[ranNumList[0]].transform.right) * baseObjDir;
                    break;
                default:
                    break;
            }
            Vector3 objectItemPos = objectDir * objectTVector + objectPos;
            flatItemList[ranNumList[i]].transform.SetPositionAndRotation(objectItemPos, objectDir);
        }
    }

    private void DirectionReset()
    {
        Vector3 temp = this.gameObject.transform.eulerAngles;
        lookDir = Quaternion.Euler(new Vector3(temp.x, temp.y, 0f));
    }

    private void VirtualObjectRotate(direction desiredDirection)
    {
        if (curDirection == direction.left)
        {
            if (desiredDirection == direction.right)
            {
                curDirection = direction.front;
                for (int i = 0; i < ranNumList.Count(); i++)
                {
                    flatItemList[ranNumList[i]].transform.RotateAround(objectPos, flatItemList[ranNumList[0]].transform.up, itemAngle);
                }
            }
        }
        else if (curDirection == direction.front)
        {
            if (desiredDirection == direction.right)
            {
                curDirection = direction.right;
                for (int i = 0; i < ranNumList.Count(); i++)
                {
                    flatItemList[ranNumList[i]].transform.RotateAround(objectPos, flatItemList[ranNumList[0]].transform.up, itemAngle);
                }
            }
            else if (desiredDirection == direction.up)
            {
                curDirection = direction.up;
                for (int i = 0; i < ranNumList.Count(); i++)
                {
                    flatItemList[ranNumList[i]].transform.RotateAround(objectPos, flatItemList[ranNumList[0]].transform.right, -itemAngle);
                }
            }
            else if (desiredDirection == direction.down)
            {
                curDirection = direction.down;
                for (int i = 0; i < ranNumList.Count(); i++)
                {
                    flatItemList[ranNumList[i]].transform.RotateAround(objectPos, flatItemList[ranNumList[0]].transform.right, itemAngle);
                }
            }
            else if (desiredDirection == direction.left)
            {
                curDirection = direction.left;
                for (int i = 0; i < ranNumList.Count(); i++)
                {
                    flatItemList[ranNumList[i]].transform.RotateAround(objectPos, flatItemList[ranNumList[0]].transform.up, -itemAngle);
                }
            }
        }
        else if (curDirection == direction.right)
        {
            if (desiredDirection == direction.left)
            {
                curDirection = direction.front;
                for (int i = 0; i < ranNumList.Count(); i++)
                {
                    flatItemList[ranNumList[i]].transform.RotateAround(objectPos, flatItemList[ranNumList[0]].transform.up, -itemAngle);
                }
            }
        }
        else if (curDirection == direction.up)
        {
            if (desiredDirection == direction.down)
            {
                curDirection = direction.front;
                for (int i = 0; i < ranNumList.Count(); i++)
                {
                    flatItemList[ranNumList[i]].transform.RotateAround(objectPos, flatItemList[ranNumList[0]].transform.right, itemAngle);
                }
            }
        }
        else if (curDirection == direction.down)
        {
            if (desiredDirection == direction.up)
            {
                curDirection = direction.front;
                for (int i = 0; i < ranNumList.Count(); i++)
                {
                    flatItemList[ranNumList[i]].transform.RotateAround(objectPos, flatItemList[ranNumList[0]].transform.right, -itemAngle);
                }
            }
        }
        else
        {
            Debug.LogError("Undefined");
        }

    }

    private direction GetDPadInput()
    {
        if (SteamVR_Input.GetStateDown("NorthLeftHand", SteamVR_Input_Sources.LeftHand))
        {
            direction desiredDirection = direction.up;
            VirtualObjectRotate(desiredDirection);
            return desiredDirection;
        }
        else if (SteamVR_Input.GetStateDown("EastLeftHand", SteamVR_Input_Sources.LeftHand))
        {
            direction desiredDirection = direction.right;
            VirtualObjectRotate(desiredDirection);
            return desiredDirection;
        }
        else if (SteamVR_Input.GetStateDown("WestLeftHand", SteamVR_Input_Sources.LeftHand))
        {
            direction desiredDirection = direction.left;
            VirtualObjectRotate(desiredDirection);
            return desiredDirection;
        }
        else if (SteamVR_Input.GetStateDown("SouthLeftHand", SteamVR_Input_Sources.LeftHand))
        {
            direction desiredDirection = direction.down;
            VirtualObjectRotate(desiredDirection);
            return desiredDirection;
        }
        else
        {
            direction desiredDirection = direction.front;
            VirtualObjectRotate(desiredDirection);
            return desiredDirection;
        }
    }

    private enum direction
    {
        front, left, right, up, down
    }
}