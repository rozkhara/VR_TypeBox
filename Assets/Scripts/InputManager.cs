using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class InputManager : MonoBehaviour
{
    AutomateKR mAutomateKR = new AutomateKR();
    string st = AutomateKR.SOUND_TABLE;
    int[,] mv = AutomateKR.MIXED_VOWEL;
    int[,] mjc = AutomateKR.MIXED_JONG_CONSON;

    public TextMeshProUGUI targetTextField = null;
    public TextMeshProUGUI inputTextField = null;
    List<Dictionary<string, object>> wordData;

    string targetWord = null;
    char[] targetKeySeq = new char[15];
    char[] inputKeySeq = new char[15];
    int keyCnt = 0;

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

        TextField = "Enter";
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
            DeleteInput();
            return;
        }

        if (_key == 'C') // ���߿� �׳� �ܾ� �ϼ��Ǹ� �ڵ����� �Ѿ�� ����?
        {
            CheckInput(targetWord);
            return;
        }

        mAutomateKR.SetKeyCode(_key);
        TextField = mAutomateKR.completeText + mAutomateKR.ingWord;
        UpdateNextKey();

        char[] targetWordArray = KeySequence(targetWord);
        int idx = 0;
        int wordIdx = 0;
        string completeText = null;
        bool check = false;

        if (check) return;

        if (_key == targetWordArray[idx])
        {
            idx++;
        }
        else
        {
            check = true;

            string curText = null;

            if (mAutomateKR.completeText == null)
            {
                curText = mAutomateKR.ingWord;
            }
            else
            {
                curText = mAutomateKR.completeText;
            }

            if (mAutomateKR.completeText != completeText)
            {
                completeText = mAutomateKR.completeText;
                wordIdx++;

                if (_key != targetWordArray[idx])
                {
                    targetTextField.text = targetWord.Substring(0, wordIdx) + "<color=red>" + targetWord[wordIdx] + "</color>" + targetWord.Substring(wordIdx + 1);
                }
            }
        }
    }


    bool StringCmp(string _ingWord, string curChar)
    {
        char[] curCharArray = KeySequence(curChar);
        char[] ingCharArray = KeySequence(_ingWord);

        int cnt = 0;

        foreach (var ingChar in ingCharArray)
        {
            if (ingChar - 'a' < 0 || ingChar - 'a' >= 26) continue;

            Debug.Log("HI! : " + ingChar);
            if (ingChar == curCharArray[cnt]) cnt++;
            else return false;
        }

        return true;
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
