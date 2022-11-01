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

        // random 단어를 key sequeance로 분해
        //KeySequence(wordData[Random.Range(0, wordData.Count)][Random.Range(1, 4).ToString()].ToString());
        UpdateTargetWord();

        TextField = "키를 누르세요.";
    }

    public void Clear()
    {
        mAutomateKR.Clear();

        TextField = mAutomateKR.completeText + mAutomateKR.ingWord;
        inputKeySeq = null;
    }

    // 한글키
    
    public void KeyDownHangul(char _key)
    {
        if (_key == 'C') // 나중엔 그냥 단어 완성되면 자동으로 넘어가게 수정?
        {
            CheckInput(targetWord);
            // Clear();
        }

        mAutomateKR.SetKeyCode(_key);

        TextField = mAutomateKR.completeText + mAutomateKR.ingWord;
        UpdateNextKey();
    }

    void UpdateTargetWord()
    {
        // 중복 없이 할거면 중복 확인 함수?도 만들어야 할 듯 
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

    }

    void CheckInput(string originWord)
    {
        //Debug.LogFormat("{0} == {1} ? => {2}", TextField, originWord, TextField == originWord);
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

    char[] KeySequence(string originWord) // originWord를 입력하는데 필요한 key 문자 배열, keyTest 이름 배열을 반환
    {
        int[] ires = new int[15];
        char[] res = new char[15];
        char[] c = originWord.ToCharArray();
        int cho, jung, jong;
        int temp, count = 0;
        for (int i = 0; i < c.Length; i++)
        {
            // 한글 유니코드를 초성/중성/종성으로 분해
            if (c[i] > 0xAC00)
            {
                temp = c[i] - 0xAC00;
                cho = temp / (28 * 21);
                jung = (temp % (28 * 21) / 28) + 19;
                jong = temp % 28 + 40;

                // res에 정답 키 순서대로 저장
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
            { // 그냥 자모 하나일 때
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

        // keyTest 객체 이름을 반환
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
