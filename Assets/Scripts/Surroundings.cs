using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class Surroundings : MonoBehaviour
{
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
        for (int i = 0; i < 3; i++)
        {
            leftKeyObjects.Add(new List<GameObject>());
            rightKeyObjects.Add(new List<GameObject>());
            for (int j = 0; j < 5; j++)
            {
                go = Instantiate(keyObject);
                leftKeyObjects[i].Add(go);
                go = Instantiate(keyObject);
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
