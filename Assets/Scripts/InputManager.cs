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
    }

    private void Start()
    {
        wordData = CSVReader.Read("wordLen");

        // random �ܾ key sequeance�� ����
        //KeySequence(wordData[Random.Range(0, wordData.Count)][Random.Range(1, 4).ToString()].ToString());
        UpdateTargetWord(); 
        if (keyCnt < targetKeySeq.Length)
            Debug.LogFormat("{0}키({1}키)를 입력하세요. )", st[AutomateKR.HANGULE_KEY_TABLE[targetKeySeq[keyCnt]]], targetKeySeq[keyCnt]);
        else
        {
            Debug.Log("완성");
        }

        TextField = "Enter";
    }

    public void Clear()
    {
        mAutomateKR.Clear();

        TextField = mAutomateKR.completeText + mAutomateKR.ingWord;
        inputKeySeq = null;
    }

    // �ѱ�Ű
    
    public void KeyDownHangul(char _key)
    {
        if(_key == 'B')
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
        Debug.LogFormat("{0} < {1} ?", keyCnt, targetKeySeq.Length);
        if (keyCnt < targetKeySeq.Length)
            Debug.LogFormat("{0}키({1}키)를 입력하세요. )", st[AutomateKR.HANGULE_KEY_TABLE[targetKeySeq[keyCnt]]], targetKeySeq[keyCnt]);
        else
        {
            Debug.Log("완성");
            // 지금 방식으로 처리하면 정답이 '배'인데 'ㅂㅐ'로 입력한 상태에도 완성으로 처리됨
            // 근데 게임하는 사람이 눈으로 보고 잘못 친거 아니까 알아서 전부 backspace 할거고 그럼 그냥 다시 'ㅂ' 입력하라고 뜸
            // 결론 : 이름이 targetKeySeq[keyCnt]인 keyTest 오브젝트를 포함시키면 정답 완성을 위해 눌러야 할 키가 없는 경우는 발생하지 않음.
        }

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
        for (int i = 0; i < inputKeySeq.Length; i++)
        {
            if (inputKeySeq[i] != targetKeySeq[i])
            {
                keyCnt = i;
                return;
            }            
        }
        keyCnt = inputKeySeq.Length;

    void DeleteInput()
    {
        mAutomateKR.SetKeyCode(AutomateKR.KEY_CODE_BACKSPACE);
        TextField = mAutomateKR.completeText + mAutomateKR.ingWord;
        UpdateNextKey();
        if (keyCnt < targetKeySeq.Length)
            Debug.LogFormat("{0}키({1}키)를 입력하세요. )", st[AutomateKR.HANGULE_KEY_TABLE[targetKeySeq[keyCnt]]], targetKeySeq[keyCnt]);
        else
        {
            Debug.Log("완성");
            // 지금 방식으로 처리하면 정답이 '배'인데 'ㅂㅐ'로 입력한 상태에도 완성으로 처리됨
            // 근데 게임하는 사람이 눈으로 보고 잘못 친거 아니까 알아서 전부 backspace 할거고 그럼 그냥 다시 'ㅂ' 입력하라고 뜸
            // 결론 : 이름이 targetKeySeq[keyCnt]인 keyTest 오브젝트를 포함시키면 정답 완성을 위해 눌러야 할 키가 없는 경우는 발생하지 않음.
        }
    }

    void CheckInput(string originWord)
    {
        if(TextField == originWord)
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
                        ires[count] = mjc[j, 0];
                        res[count++] = st[mjc[j, 0]];
                        ires[count] = mjc[j, 1];
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
                    if(c[i] == st[j])
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
