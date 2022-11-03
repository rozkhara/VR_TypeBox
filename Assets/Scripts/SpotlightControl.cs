using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotlightControl : MonoBehaviour
{
    private GameObject _player;
    private void Awake()
    {
        _player = GameObject.Find("Player");
    }
    private void Update()
    {
        this.gameObject.transform.rotation = Quaternion.LookRotation(_player.transform.position - this.gameObject.transform.position);
    }
}
