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

        inputTextField.text = "Ű�� ��������.";
    }

    public void Clear()
    {
        mAutomateKR.Clear();

        TextField = mAutomateKR.completeText + mAutomateKR.ingWord;
    }

    // �ѱ�Ű
    
    public void KeyDownHangul(char _key)
    {
        if (_key == 'C')
        {
            CheckInput(targetWord);
            // Clear();
        }

        mAutomateKR.SetKeyCode(_key);

        TextField = mAutomateKR.completeText + mAutomateKR.ingWord;
    }

    void UpdateTargetWord()
    {
        // �ߺ� ���� �ҰŸ� �ߺ� Ȯ�� �Լ�?�� ������ �� �� 
        targetWord = wordData[Random.Range(0, wordData.Count)][Random.Range(1, 4).ToString()].ToString();
        targetTextField.text = targetWord;
    }

    void CheckInput(string originWord)
    {
        Debug.LogFormat("{0} == {1} ? => {2}", TextField, originWord, TextField == originWord);
        if(TextField == originWord)
        {
            UpdateTargetWord();
            //KeySequence(wordData[Random.Range(0, wordData.Count)][Random.Range(1, 4).ToString()].ToString());
        }
        else
        {
            Clear();
        }
    }

    char[] KeySequence(string originWord) // originWord�� �Է��ϴµ� �ʿ��� key ���� �迭, keyTest �̸� �迭�� ��ȯ
    {
        targetTextField.text = originWord;

        int[] ires = new int[15];
        char[] res = new char[15];
        char[] c = originWord.ToCharArray();
        int cho, jung, jong;
        int temp, count = 0;
        for (int i = 0; i < c.Length; i++)
        {
            // �ѱ� �����ڵ带 �ʼ�/�߼�/�������� ����
            temp = c[i] - 0xAC00;
            cho = temp / (28 * 21);
            jung = (temp % (28 * 21) / 28) + 19;
            jong = temp % 28 + 40;
            // Debug.LogFormat("{0} �ʼ� : {1} �߼� : {2} ���� : {3}", c[i], s[cho], s[jung], s[jong]);

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

        // keyTest ��ü �̸��� ��ȯ
        char[] keyTextName = new char[15];
        for (int i = 0; i < count; i++)
        {
            keyTextName[i] = AutomateKR.HANGULE_KEY_TABLE.FirstOrDefault(x => x.Value == ires[i]).Key;
        }
        // Debug.LogFormat("{0}, {1}, {2}", originWord, res.ArrayToString(), keyTextName.ArrayToString());
        return res;        
    }
}
