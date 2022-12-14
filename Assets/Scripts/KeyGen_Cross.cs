using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using TMPro;

public class KeyGen_Cross : MonoBehaviour
{
    private readonly string[] leftkeyName = new string[4] { "QWERT", "qwert", "asdfg", "zxcv" };
    private readonly string[] rightkeyName = new string[4] { "OP", "yuiop", "hjkl", "bnm" };
    private const float distance = 0.7f;
    private const float angleHorizontal = 13f;
    private const float itemAngle = -45f;
    public bool isStarted = false;
    private GameObject go;
    private Quaternion lookDir;
    private List<List<GameObject>> leftKeyObjects = null;
    private List<List<GameObject>> rightKeyObjects = null;
    public List<GameObject> FlatItemListLeft { get; private set; } = null;
    public List<GameObject> FlatItemListRight { get; private set; } = null;
    private List<int> ranNumListLeft = null;
    private List<int> ranNumListRight = null;
    private bool isKeyInstantiatedLeft = false;
    private bool isKeyInstantiatedRight = false;
    private direction curDirectionLeft = direction.front;
    private direction curDirectionRight = direction.front;
    private Vector3 objectPosLeft = new();
    private Vector3 objectPosRight = new();

    [SerializeField] private GameObject keyObject;


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
        isKeyInstantiatedLeft = false;
        isKeyInstantiatedRight = false;
    }

    public void OnStartButtonClicked()
    {
        GameManager.Instance.Timer = 60f;
        GameManager.Instance.IsGameOver = false;
        Initialize();
        InputManager.Instance.Initialize();
        DirectionReset();
        isStarted = true;
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
            DirectionReset();
        }
        //for PC debug purpose
        if (Input.GetKeyDown(KeyCode.Return))
        {
            DirectionReset();
        }

        if (SteamVR_Input.GetStateDown("LowerButtonLeft", SteamVR_Input_Sources.LeftHand))
        {
            InputManager.Instance.KeyDownHangul('B');
            SetNewPosRot(false);
        }
        if (SteamVR_Input.GetStateDown("LowerButtonRight", SteamVR_Input_Sources.RightHand))
        {
            InputManager.Instance.KeyDownHangul('C');
            SetNewPosRot(false);
        }
        GetDPadInputLeft();
        GetDPadInputRight();
    }

    public void SetNewPosRot(bool useNewDir = false)
    {
        Vector3 position = this.gameObject.transform.position;
        Vector3 translateVector = new(0f, 0f, distance);
        Quaternion newDirLeft = Quaternion.AngleAxis(-angleHorizontal * 2, Vector3.up) * lookDir;
        Quaternion newDirRight = Quaternion.AngleAxis(angleHorizontal * 2, Vector3.up) * lookDir;
        RenewCrossKeysLeft();
        RenewCrossKeysRight();
        if (useNewDir)
        {
            RotateObjectItemsLeft(position, translateVector, newDirLeft, true);
            RotateObjectItemsRight(position, translateVector, newDirRight, true);
        }
        else
        {
            RotateObjectItemsLeft(position, translateVector, newDirLeft);
            RotateObjectItemsRight(position, translateVector, newDirRight);
        }
        curDirectionLeft = direction.front;
        curDirectionRight = direction.front;
    }

    private void RenewCrossKeysLeft()
    {
        if (isKeyInstantiatedLeft)
        {
            for (int i = 0; i < 5; i++)
            {
                FlatItemListLeft[ranNumListLeft[i]].SetActive(false);
            }
            ranNumListLeft.Clear();
        }
        else
        {
            FlatItemListLeft = leftKeyObjects.SelectMany(x => x).ToList();
            ranNumListLeft = new List<int>();
            isKeyInstantiatedLeft = true;
        }
        char targetKey = InputManager.Instance.GetNextKey();
        Debug.Log("Target Key : " + targetKey);
        GameObject go = FlatItemListLeft.Where(x => x.name == targetKey.ToString()).SingleOrDefault();
        int keyNum = FlatItemListLeft.IndexOf(go);
        if (keyNum != -1)
        {
            ranNumListLeft.Add(keyNum);
            int ranNum;
            for (int i = 0; i < 4; i++)
            {
                do { ranNum = Random.Range(0, FlatItemListLeft.Count); }
                while (ranNumListLeft.Contains(ranNum));
                ranNumListLeft.Add(ranNum);
            }
        }
        else
        {
            int ranNum;
            for (int i = 0; i < 5; i++)
            {
                do { ranNum = Random.Range(0, FlatItemListLeft.Count); }
                while (ranNumListLeft.Contains(ranNum));
                ranNumListLeft.Add(ranNum);
            }
        }
        for (int i = 0; i < 5; i++)
        {
            FlatItemListLeft[ranNumListLeft[i]].SetActive(true);
        }
        ranNumListLeft = ranNumListLeft.OrderBy(_ => Random.Range(0, int.MaxValue)).ToList();
    }

    private void RenewCrossKeysRight()
    {
        if (isKeyInstantiatedRight)
        {
            for (int i = 0; i < 5; i++)
            {
                FlatItemListRight[ranNumListRight[i]].SetActive(false);
            }
            ranNumListRight.Clear();
        }
        else
        {
            FlatItemListRight = rightKeyObjects.SelectMany(x => x).ToList();
            ranNumListRight = new List<int>();
            isKeyInstantiatedRight = true;
        }
        char targetKey = InputManager.Instance.GetNextKey();
        GameObject go = FlatItemListRight.Where(x => x.name == targetKey.ToString()).SingleOrDefault();
        int keyNum = FlatItemListRight.IndexOf(go);
        if (keyNum != -1)
        {
            ranNumListRight.Add(keyNum);
            int ranNum;
            for (int i = 0; i < 4; i++)
            {
                do { ranNum = Random.Range(0, FlatItemListRight.Count); }
                while (ranNumListRight.Contains(ranNum));
                ranNumListRight.Add(ranNum);
            }
        }
        else
        {
            int ranNum;
            for (int i = 0; i < 5; i++)
            {
                do { ranNum = Random.Range(0, FlatItemListRight.Count); }
                while (ranNumListRight.Contains(ranNum));
                ranNumListRight.Add(ranNum);
            }
        }
        for (int i = 0; i < 5; i++)
        {
            FlatItemListRight[ranNumListRight[i]].SetActive(true);
        }
        ranNumListRight = ranNumListRight.OrderBy(_ => Random.Range(0, int.MaxValue)).ToList();
    }

    private void RotateObjectItemsLeft(Vector3 position, Vector3 translateVector, Quaternion newDir, bool renewDirection = false)
    {
        if (renewDirection)
        {
            objectPosLeft = newDir * translateVector + position;
        }
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
                    objectDir = Quaternion.AngleAxis(itemAngle, FlatItemListLeft[ranNumListLeft[0]].transform.up) * baseObjDir;
                    break;
                case direction.right:
                    objectDir = Quaternion.AngleAxis(-itemAngle, FlatItemListLeft[ranNumListLeft[0]].transform.up) * baseObjDir;
                    break;
                case direction.up:
                    objectDir = Quaternion.AngleAxis(itemAngle, FlatItemListLeft[ranNumListLeft[0]].transform.right) * baseObjDir;
                    break;
                case direction.down:
                    objectDir = Quaternion.AngleAxis(-itemAngle, FlatItemListLeft[ranNumListLeft[0]].transform.right) * baseObjDir;
                    break;
                default:
                    break;
            }
            Vector3 objectItemPos = objectDir * objectTVector + objectPosLeft;
            FlatItemListLeft[ranNumListLeft[i]].transform.SetPositionAndRotation(objectItemPos, objectDir);
        }
    }

    private void RotateObjectItemsRight(Vector3 position, Vector3 translateVector, Quaternion newDir, bool renewDirection = false)
    {
        if (renewDirection)
        {
            objectPosRight = newDir * translateVector + position;
        }
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
                    objectDir = Quaternion.AngleAxis(itemAngle, FlatItemListRight[ranNumListRight[0]].transform.up) * baseObjDir;
                    break;
                case direction.right:
                    objectDir = Quaternion.AngleAxis(-itemAngle, FlatItemListRight[ranNumListRight[0]].transform.up) * baseObjDir;
                    break;
                case direction.up:
                    objectDir = Quaternion.AngleAxis(itemAngle, FlatItemListRight[ranNumListRight[0]].transform.right) * baseObjDir;
                    break;
                case direction.down:
                    objectDir = Quaternion.AngleAxis(-itemAngle, FlatItemListRight[ranNumListRight[0]].transform.right) * baseObjDir;
                    break;
                default:
                    break;
            }
            Vector3 objectItemPos = objectDir * objectTVector + objectPosRight;
            FlatItemListRight[ranNumListRight[i]].transform.SetPositionAndRotation(objectItemPos, objectDir);
        }
    }

    private void DirectionReset()
    {
        Vector3 temp = this.gameObject.transform.eulerAngles;
        lookDir = Quaternion.Euler(new Vector3(temp.x, temp.y, 0f));
        SetNewPosRot(true);
    }

    private void VirtualObjectRotate(direction desiredDirection, bool isLeft = true)
    {
        if (isLeft)
        {
            if (curDirectionLeft == direction.left)
            {
                if (desiredDirection == direction.right)
                {
                    curDirectionLeft = direction.front;
                    for (int i = 0; i < ranNumListLeft.Count(); i++)
                    {
                        FlatItemListLeft[ranNumListLeft[i]].transform.RotateAround(objectPosLeft, FlatItemListLeft[ranNumListLeft[0]].transform.up, itemAngle);
                    }
                }
            }
            else if (curDirectionLeft == direction.front)
            {
                if (desiredDirection == direction.right)
                {
                    curDirectionLeft = direction.right;
                    for (int i = 0; i < ranNumListLeft.Count(); i++)
                    {
                        FlatItemListLeft[ranNumListLeft[i]].transform.RotateAround(objectPosLeft, FlatItemListLeft[ranNumListLeft[0]].transform.up, itemAngle);
                    }
                }
                else if (desiredDirection == direction.up)
                {
                    curDirectionLeft = direction.up;
                    for (int i = 0; i < ranNumListLeft.Count(); i++)
                    {
                        FlatItemListLeft[ranNumListLeft[i]].transform.RotateAround(objectPosLeft, FlatItemListLeft[ranNumListLeft[0]].transform.right, -itemAngle);
                    }
                }
                else if (desiredDirection == direction.down)
                {
                    curDirectionLeft = direction.down;
                    for (int i = 0; i < ranNumListLeft.Count(); i++)
                    {
                        FlatItemListLeft[ranNumListLeft[i]].transform.RotateAround(objectPosLeft, FlatItemListLeft[ranNumListLeft[0]].transform.right, itemAngle);
                    }
                }
                else if (desiredDirection == direction.left)
                {
                    curDirectionLeft = direction.left;
                    for (int i = 0; i < ranNumListLeft.Count(); i++)
                    {
                        FlatItemListLeft[ranNumListLeft[i]].transform.RotateAround(objectPosLeft, FlatItemListLeft[ranNumListLeft[0]].transform.up, -itemAngle);
                    }
                }
            }
            else if (curDirectionLeft == direction.right)
            {
                if (desiredDirection == direction.left)
                {
                    curDirectionLeft = direction.front;
                    for (int i = 0; i < ranNumListLeft.Count(); i++)
                    {
                        FlatItemListLeft[ranNumListLeft[i]].transform.RotateAround(objectPosLeft, FlatItemListLeft[ranNumListLeft[0]].transform.up, -itemAngle);
                    }
                }
            }
            else if (curDirectionLeft == direction.up)
            {
                if (desiredDirection == direction.down)
                {
                    curDirectionLeft = direction.front;
                    for (int i = 0; i < ranNumListLeft.Count(); i++)
                    {
                        FlatItemListLeft[ranNumListLeft[i]].transform.RotateAround(objectPosLeft, FlatItemListLeft[ranNumListLeft[0]].transform.right, itemAngle);
                    }
                }
            }
            else if (curDirectionLeft == direction.down)
            {
                if (desiredDirection == direction.up)
                {
                    curDirectionLeft = direction.front;
                    for (int i = 0; i < ranNumListLeft.Count(); i++)
                    {
                        FlatItemListLeft[ranNumListLeft[i]].transform.RotateAround(objectPosLeft, FlatItemListLeft[ranNumListLeft[0]].transform.right, -itemAngle);
                    }
                }
            }
            else
            {
                Debug.LogError("Undefined");
            }
        }
        else
        {
            if (curDirectionRight == direction.left)
            {
                if (desiredDirection == direction.right)
                {
                    curDirectionRight = direction.front;
                    for (int i = 0; i < ranNumListRight.Count(); i++)
                    {
                        FlatItemListRight[ranNumListRight[i]].transform.RotateAround(objectPosRight, FlatItemListRight[ranNumListRight[0]].transform.up, itemAngle);
                    }
                }
            }
            else if (curDirectionRight == direction.front)
            {
                if (desiredDirection == direction.right)
                {
                    curDirectionRight = direction.right;
                    for (int i = 0; i < ranNumListRight.Count(); i++)
                    {
                        FlatItemListRight[ranNumListRight[i]].transform.RotateAround(objectPosRight, FlatItemListRight[ranNumListRight[0]].transform.up, itemAngle);
                    }
                }
                else if (desiredDirection == direction.up)
                {
                    curDirectionRight = direction.up;
                    for (int i = 0; i < ranNumListRight.Count(); i++)
                    {
                        FlatItemListRight[ranNumListRight[i]].transform.RotateAround(objectPosRight, FlatItemListRight[ranNumListRight[0]].transform.right, -itemAngle);
                    }
                }
                else if (desiredDirection == direction.down)
                {
                    curDirectionRight = direction.down;
                    for (int i = 0; i < ranNumListRight.Count(); i++)
                    {
                        FlatItemListRight[ranNumListRight[i]].transform.RotateAround(objectPosRight, FlatItemListRight[ranNumListRight[0]].transform.right, itemAngle);
                    }
                }
                else if (desiredDirection == direction.left)
                {
                    curDirectionRight = direction.left;
                    for (int i = 0; i < ranNumListRight.Count(); i++)
                    {
                        FlatItemListRight[ranNumListRight[i]].transform.RotateAround(objectPosRight, FlatItemListRight[ranNumListRight[0]].transform.up, -itemAngle);
                    }
                }
            }
            else if (curDirectionRight == direction.right)
            {
                if (desiredDirection == direction.left)
                {
                    curDirectionRight = direction.front;
                    for (int i = 0; i < ranNumListRight.Count(); i++)
                    {
                        FlatItemListRight[ranNumListRight[i]].transform.RotateAround(objectPosRight, FlatItemListRight[ranNumListRight[0]].transform.up, -itemAngle);
                    }
                }
            }
            else if (curDirectionRight == direction.up)
            {
                if (desiredDirection == direction.down)
                {
                    curDirectionRight = direction.front;
                    for (int i = 0; i < ranNumListRight.Count(); i++)
                    {
                        FlatItemListRight[ranNumListRight[i]].transform.RotateAround(objectPosRight, FlatItemListRight[ranNumListRight[0]].transform.right, itemAngle);
                    }
                }
            }
            else if (curDirectionRight == direction.down)
            {
                if (desiredDirection == direction.up)
                {
                    curDirectionRight = direction.front;
                    for (int i = 0; i < ranNumListRight.Count(); i++)
                    {
                        FlatItemListRight[ranNumListRight[i]].transform.RotateAround(objectPosRight, FlatItemListRight[ranNumListRight[0]].transform.right, -itemAngle);
                    }
                }
            }
            else
            {
                Debug.LogError("Undefined");
            }
        }
    }

    private direction GetDPadInputLeft()
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

    private direction GetDPadInputRight()
    {
        if (SteamVR_Input.GetStateDown("NorthRightHand", SteamVR_Input_Sources.RightHand))
        {
            direction desiredDirection = direction.up;
            VirtualObjectRotate(desiredDirection, false);
            return desiredDirection;
        }
        else if (SteamVR_Input.GetStateDown("EastRightHand", SteamVR_Input_Sources.RightHand))
        {
            direction desiredDirection = direction.right;
            VirtualObjectRotate(desiredDirection, false);
            return desiredDirection;
        }
        else if (SteamVR_Input.GetStateDown("WestRightHand", SteamVR_Input_Sources.RightHand))
        {
            direction desiredDirection = direction.left;
            VirtualObjectRotate(desiredDirection, false);
            return desiredDirection;
        }
        else if (SteamVR_Input.GetStateDown("SouthRightHand", SteamVR_Input_Sources.RightHand))
        {
            direction desiredDirection = direction.down;
            VirtualObjectRotate(desiredDirection, false);
            return desiredDirection;
        }
        else
        {
            direction desiredDirection = direction.front;
            VirtualObjectRotate(desiredDirection, false);
            return desiredDirection;
        }
    }

    private enum direction
    {
        front, left, right, up, down
    }
}