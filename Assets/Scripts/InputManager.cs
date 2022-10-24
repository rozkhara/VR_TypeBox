using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputManager : MonoBehaviour
{
    public TextMeshProUGUI inputField = null;

    private void Start()
    {
        inputField.text = "입력한 단어";
    }

    public void InsertChar(string c)
    {
    }

    public void DeleteChar()
    {
        if (inputField.text.Length > 0)
        {
            inputField.text = inputField.text.Substring(0, inputField.text.Length - 1);
        }
    }
}
