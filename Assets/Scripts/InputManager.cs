using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    AutomateKR mAutomateKR = new AutomateKR();
    string st = AutomateKR.SOUND_TABLE;
    int[,] mv = AutomateKR.MIXED_VOWEL;
    int[,] mjc = AutomateKR.MIXED_JONG_CONSON;

    public TextMeshProUGUI targetTextField = null;
    public TextMeshProUGUI inputTextField = null;
    List<Dictionary<string, object>> wordData;

    public TextMeshProUGUI ScoreText = null;

    string targetWord = null;
    char[] targetKeySeq = new char[15];
    char[] inputKeySeq = new char[15];
    int keyCnt = 0;
    int idx = 0;
    int wordIdx = 0;
    string completeText = null;
    bool preCheck = true;
    bool check = true;

    public string TextField
    {
        set
        {
            if (inputTextField != null)
            {
                inputTextField.text = value;
            }
        }
        get
        {
            if (inputTextField != null)
            {
                return inputTextField.text;
            }
            return "";
        }
    }

    void Awake()
    {
        KeyInteract._Keybord = this;
        KeyInteractAlt._Keybord = this;
    }

    private void Start()
    {
        wordData = CSVReader.Read("wordLen");

        // random �ܾ key sequeance�� ����
        //KeySequence(wordData[Random.Range(0, wordData.Count)][Random.Range(1, 4).ToString()].ToString());
        UpdateTargetWord();
        DebugNextKey();

        TextField = "Ű�� ��������.";
    }

    void DebugNextKey()
    {
        if (keyCnt < targetKeySeq.ArrayToString().Length)
        {
            // 정답 키 : targetKeySeq[keyCnt]
            Debug.LogFormat("{0}키({1}키)를 포함해야 함 )", st[AutomateKR.HANGULE_KEY_TABLE[targetKeySeq[keyCnt]]], targetKeySeq[keyCnt]);
        }
        else
        {
            Debug.Log("아무 키나 포함해도 됨");
        }
    }

    public char GetNextKey()
    {
        return targetKeySeq[keyCnt];
    }

    public void Clear()
    {
        mAutomateKR.Clear();
        TextField = mAutomateKR.completeText + mAutomateKR.ingWord;
        inputKeySeq = null;
        UpdateNextKey();
    }

    // �ѱ�Ű

    public void KeyDownHangul(char _key)
    {
        if (_key == 'B')
        {
            bool bCheck = false;
            bool tmp = check;

            if (mAutomateKR.ingWord == null) bCheck = true;

            SetPreCheck();

            DeleteInput();

            SetCheck();

            if (check)
            {
                if (bCheck && mAutomateKR.ingWord == null && mAutomateKR.completeText != null && mAutomateKR.completeText != "")
                {
                    if (tmp && mAutomateKR.completeText != completeText && wordIdx > 0)
                    {
                        completeText = mAutomateKR.completeText;
                        wordIdx--;
                    }

                    string completeString = KeySequence(mAutomateKR.completeText[wordIdx].ToString()).ArrayToString().Trim();
                    idx -= completeString.Length;
                }
                else if (!bCheck && preCheck) idx--;

                targetTextField.text = targetWord;
            }

            return;
        }

        if (_key == 'C') // ���߿� �׳� �ܾ� �ϼ��Ǹ� �ڵ����� �Ѿ�� ����?
        {
            CheckInput(targetWord);
            SetCheck();
            completeText = "";
            return;
        }

        mAutomateKR.SetKeyCode(_key);
        TextField = mAutomateKR.completeText + mAutomateKR.ingWord;
        UpdateNextKey();

        string targetCharArray = KeySequence(targetWord).ArrayToString().Trim();
        string curTextCharArray = KeySequence(TextField).ArrayToString().Trim();

        /*
        if (check && _key == 'B')
        {
            if (mAutomateKR.ingWord == null)
            {
                string completeString = KeySequence(mAutomateKR.completeText[wordIdx].ToString()).ArrayToString().Trim();
                idx -= completeString.Length;
            }
            else
            {
                string ingString = KeySequence(mAutomateKR.ingWord).ArrayToString().Trim();
                string curTargetString = KeySequence(mAutomateKR.completeText[wordIdx].ToString()).ArrayToString().Trim();
                int curIdx = 0;
                bool key = false;

                foreach (var ingChar in ingString)
                {
                    if (ingChar == curTargetString[curIdx]) curIdx++;
                    else
                    {
                        key = true;
                        break;
                    }
                }

                if (!key)
                {
                    targetTextField.text = targetWord;
                    check = true;
                }
                idx--;
            }

            if (mAutomateKR.completeText != completeText && wordIdx > 0) wordIdx--;

            return;
        }
        */

        if (check && mAutomateKR.completeText != completeText)
        {
            if (wordIdx < targetWord.Length - 1) wordIdx++;

            completeText = mAutomateKR.completeText;
        }

        if (check && _key != targetCharArray[idx])
        {
            if (targetWord.Length == 1) targetTextField.text = "<color=red>" + targetWord + "</color>";
            else targetTextField.text = targetWord.Substring(0, wordIdx) + "<color=red>" + targetWord[wordIdx] + "</color>" + targetWord.Substring(wordIdx + 1);

            check = false;
        }

        if (check && _key == targetCharArray[idx])
        {
            idx++;
        }
    }
    private void SetPreCheck()
    {
        if (mAutomateKR.ingWord == null && (mAutomateKR.completeText == "" || mAutomateKR.completeText == null))
        {
            targetTextField.text = targetWord;
            preCheck = true;
            idx = 0;
            wordIdx = 0;

            return;
        }

        string targetCharArray = KeySequence(targetWord).ArrayToString().Trim();
        string curTextCharArray = KeySequence(TextField).ArrayToString().Trim();
        int tmp = 0;

        foreach (var curTextChar in curTextCharArray)
        {
            if (curTextChar == targetCharArray[tmp]) tmp++;
            else
            {
                preCheck = false;
                return;
            }
        }

        preCheck = true;
    }

    private void SetCheck()
    {
        if (mAutomateKR.ingWord == null && (mAutomateKR.completeText == "" || mAutomateKR.completeText == null))
        {
            Debug.Log("1!");
            targetTextField.text = targetWord;
            check = true;
            idx = 0;
            wordIdx = 0;

            return;
        }

        Debug.Log("2!");
        string targetCharArray = KeySequence(targetWord).ArrayToString().Trim();
        string curTextCharArray = KeySequence(TextField).ArrayToString().Trim();
        int tmp = 0;

        foreach (var curTextChar in curTextCharArray)
        {
            if (curTextChar == targetCharArray[tmp]) tmp++;
            else
            {
                check = false;
                return;
            }
        }

        check = true;
    }

    void UpdateTargetWord()
    {
        // �ߺ� ���� �ҰŸ� �ߺ� Ȯ�� �Լ�?�� ������ �� �� 
        targetWord = wordData[Random.Range(0, wordData.Count)][Random.Range(1, 4).ToString()].ToString();
        targetTextField.text = string.Copy(targetWord);
        targetKeySeq = KeySequence(targetWord);
        keyCnt = 0;
        // Debug.Log(targetKeySeq.ArrayToString());
    }

    void UpdateNextKey()
    {
        inputKeySeq = KeySequence(TextField);
        //Debug.LogFormat("{0} {1}", inputKeySeq.ArrayToString(), targetKeySeq.ArrayToString());
        for (int i = 0; i < inputKeySeq.ArrayToString().Length; i++)
        {
            if (inputKeySeq[i] != targetKeySeq[i])
            {
                keyCnt = i;
                DebugNextKey();
                return;
            }
        }
        keyCnt = inputKeySeq.ArrayToString().Length;
        DebugNextKey();
    }

    void DeleteInput()
    {
        mAutomateKR.SetKeyCode(AutomateKR.KEY_CODE_BACKSPACE);
        TextField = mAutomateKR.completeText + mAutomateKR.ingWord;
        UpdateNextKey();
    }

    void CheckInput(string originWord)
    {
        if (TextField == originWord)
        {
            UpdateTargetWord();
            GameManager.Instance.Score += TextField.Length;
            // GameManager.Instance.Score++;
            ScoreText.text = "Score : " + GameManager.Instance.Score.ToString();
        }
        Clear();
    }

    char[] KeySequence(string originWord) // originWord�� �Է��ϴµ� �ʿ��� key ���� �迭, keyTest �̸� �迭�� ��ȯ
    {
        int[] ires = new int[15];
        char[] res = new char[15];
        char[] c = originWord.ToCharArray();
        int cho, jung, jong;
        int temp, count = 0;
        for (int i = 0; i < c.Length; i++)
        {
            // �ѱ� �����ڵ带 �ʼ�/�߼�/�������� ����
            if (c[i] > 0xAC00)
            {
                temp = c[i] - 0xAC00;
                cho = temp / (28 * 21);
                jung = (temp % (28 * 21) / 28) + 19;
                jong = temp % 28 + 40;

                // res�� ���� Ű ������� ����
                ires[count] = cho;
                res[count++] = st[cho];
                int j = 0;
                do
                {
                    if (mv[j, 2] == jung)
                    {
                        ires[count] = mv[j, 0];
                        res[count++] = st[mv[j, 0]];
                        ires[count] = mv[j, 1];
                        res[count++] = st[mv[j, 1]];
                        break;
                    }
                } while (++j < 7);
                if (j == 7)
                {
                    ires[count] = jung;
                    res[count++] = st[jung];
                }
                j = 0;
                do
                {
                    if (mjc[j, 2] == jong)
                    {
                        ires[count] = mAutomateKR.ToInitial(mjc[j, 0]);
                        res[count++] = st[mjc[j, 0]];
                        ires[count] = mAutomateKR.ToInitial(mjc[j, 1]);
                        res[count++] = st[mjc[j, 1]];
                        break;
                    }
                } while (++j < 11);
                if (j == 11 && jong != 40)
                {
                    ires[count] = mAutomateKR.ToInitial(jong);
                    res[count++] = st[jong];
                }
            }
            else
            { // �׳� �ڸ� �ϳ��� ��
                res[count] = c[i];
                int j = 0;
                do
                {
                    if (c[i] == st[j])
                    {
                        ires[count] = j;
                        break;
                    }
                } while (++j < 40);
                count++;
            }

        }

        // keyTest ��ü �̸��� ��ȯ
        char[] keyTextName = new char[15];
        for (int i = 0; i < count; i++)
        {
            keyTextName[i] = AutomateKR.HANGULE_KEY_TABLE.FirstOrDefault(x => x.Value == ires[i]).Key;
        }
        // Debug.LogFormat("{0}, {1}, {2}", originWord, res.ArrayToString(), keyTextName.ArrayToString());
        return keyTextName;
        // return res;
    }
}
