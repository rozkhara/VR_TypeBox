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

    private void Start()
    {
        mTextField.text = "입력한 단어";
    }

    public void Clear()
    {
        mAutomateKR.Clear();

        TextField = mAutomateKR.completeText + mAutomateKR.ingWord;
    }

    // 한글키
    /*
    public void KeyDownHangul(char _key)
    {
        mAutomateKR.SetKeyCode(_key);

        TextField = mAutomateKR.completeText + mAutomateKR.ingWord;
    }
    */

    // 특수키 KeyDown() 추가해야함... 
    // Backspace, Space, Enter ?

    /*public void KeyDown(VirtualKey _key)
    {
        switch(_key.KeyType)
        {
            case VirtualKey.kType.kBackspace:
                {
                    mAutomateKR.SetKeyCode(AutomateKR.KEY_CODE_BACKSPACE);

                }
                break;
            case VirtualKey.kType.kSpace:
                {
                    mAutomateKR.SetKeyCode(AutomateKR.KEY_CODE_SPACE);
                }
                break;
        }

        TextField = mAutomateKR.completeText + mAutomateKR.ingWord;
    }*/

/*
public void DeleteChar()
{
    if (mTextField.text.Length > 0)
    {
        mTextField.text = mTextField.text.Substring(0, mTextField.text.Length - 1);
    }
}
*/

public AutomateKR.HAN_STATUS GetStatus()
{
    return mAutomateKR.GetStatus();
}
}
