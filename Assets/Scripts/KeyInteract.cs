using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Valve.VR.InteractionSystem;
using TMPro;

public class KeyInteract : MonoBehaviour
{
    static public InputManager _Keybord = null;
    private Interactable interactable;
    public TextMeshPro keyName = null;

    GameObject go;

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
    private void OnHandHoverBegin(Hand hand)
    {
        //Debug.Log("Key name : " + this.name); 
        if (_Keybord != null)
        { 
            ParticleManager.Instance.PlayParticle("Hit_VFX", this.gameObject.transform.GetChild(0).transform.position);
            _Keybord.KeyDownHangul(this.name[0]);
            Managers.SoundManager.Instance.PlaySFXSound("punch");
        }
    }
}
