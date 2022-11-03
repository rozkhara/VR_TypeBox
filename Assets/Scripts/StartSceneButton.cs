using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using TMPro;

public class StartSceneButton : MonoBehaviour
{
    private const float distance = 0.6f;
    private const float angle = 15f;

    [SerializeField] private GameObject CrossButton;
    [SerializeField] private GameObject FullButton;

    private GameObject _CrossButton;
    private GameObject _FullButton;

    private Quaternion lookDir;


    private void Initialize()
    {
        _CrossButton = Instantiate(CrossButton);
        _FullButton = Instantiate(FullButton);
        SetNewPosRot();
    }

    private void Update()
    {
        if (_CrossButton == null && _FullButton == null)
        {
            Initialize();
        }
        if ((SteamVR_Input.GetStateDown("StickClick", SteamVR_Input_Sources.LeftHand) && SteamVR_Input.GetState("StickClick", SteamVR_Input_Sources.RightHand)) ||
            (SteamVR_Input.GetState("StickClick", SteamVR_Input_Sources.LeftHand) && SteamVR_Input.GetStateDown("StickClick", SteamVR_Input_Sources.RightHand)))
        {
            //Debug.Log("Both Buttons Pressed");
            SetNewPosRot();
        }
    }

    private void SetNewPosRot()
    {
        DirectionReset();
        Vector3 position = this.gameObject.transform.position;
        Vector3 translateVector = new(0f, 0f, distance);
        Vector3 newPos = lookDir * translateVector + position;
        _CrossButton.transform.position = newPos;
        _FullButton.transform.position = newPos;
        _CrossButton.transform.RotateAround(position, _CrossButton.transform.up, angle);
        _FullButton.transform.RotateAround(position, _FullButton.transform.up, -angle);
        _CrossButton.transform.rotation = Quaternion.LookRotation(newPos - position, Vector3.up);
        _FullButton.transform.rotation = Quaternion.LookRotation(newPos - position, Vector3.up);
    }

    private void DirectionReset()
    {
        Vector3 temp = this.gameObject.transform.eulerAngles;
        lookDir = Quaternion.Euler(new Vector3(temp.x, temp.y, 0f));
    }
}
