using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InputManager : MonoBehaviour
{
    AutomateKR mAutomateKR = new AutomateKR();
    public TextMeshProUGUI mTextField = null;
    public string TextField
    {
        set
        {
            if (mTextField != null)
            {
                mTextField.text = value;
            }
        }
        get
        {
            if (mTextField != null)
            {
                return mTextField.text;
            }
            return "";
        }
    }

    void Awake()
    {
        KeyInteract._Keybord = this;
    }

    private void Start()
    {
        List<Dictionary<string, object>> wordData = CSVReader.Read("wordLen");

        mTextField.text = "키를 누르세요.";
    }

    public void Clear()
    {
        mAutomateKR.Clear();

        TextField = mAutomateKR.completeText + mAutomateKR.ingWord;
    }

    // 한글키
    
    public void KeyDownHangul(char _key)
    {
        mAutomateKR.SetKeyCode(_key);

        TextField = mAutomateKR.completeText + mAutomateKR.ingWord;
    }

public AutomateKR.HAN_STATUS GetStatus()
{
    return mAutomateKR.GetStatus();
}
}
