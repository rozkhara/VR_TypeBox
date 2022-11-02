using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR.InteractionSystem;
using TMPro;

public class KeyInteractAlt : MonoBehaviour
{
    static public InputManager _Keybord = null;
    private Interactable interactable;
    public TextMeshPro keyName = null;
    private KeyGen_Cross KG = null;

    GameObject go;

    private void Awake()
    {
        KG = GameObject.Find("HeadCollider").GetComponent<KeyGen_Cross>();
    }
    private void Start()
    {
        interactable = GetComponent<Interactable>();
        go = GameObject.Find("HeadCollider");
        //keyName.text = this.name;
    }
    private void Update()
    {
        //var dist = Vector2.Distance(new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.z), new Vector3(go.transform.position.x, go.transform.position.z));
        //Debug.Log("Distance : " + dist.ToString());
    }
    private void OnHandHoverEnd(Hand hand)
    {
        //Debug.Log("Key name : " + this.name); 
        if (_Keybord != null)
        {
            _Keybord.KeyDownHangul(this.name[0]);
            KG.SetNewPosRot(false);
        }
    }
    private void OnHandHoverBegin(Hand hand)
    {
        Managers.SoundManager.Instance.PlaySFXSound("punch");
    }
}
