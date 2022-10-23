using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class KeyInteract : MonoBehaviour
{
    private Interactable interactable;
    GameObject go;

    private void Start()
    {
        interactable = GetComponent<Interactable>();
        go = GameObject.Find("HeadCollider");

    }
    private void Update()
    {
        var dist = Vector2.Distance(new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.z), new Vector3(go.transform.position.x, go.transform.position.z));
        //Debug.Log("Distance : " + dist.ToString());
    }
}
