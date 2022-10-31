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

    public TextMeshProUGUI mTextField = null;
    List<Dictionary<string, object>> wordData;

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
        wordData = CSVReader.Read("wordLen");

        // random 단어를 key sequeance로 분해
        keySequence(wordData[Random.Range(0, wordData.Count)][Random.Range(1, 4).ToString()].ToString());

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
        if(_key == 'C') keySequence(wordData[Random.Range(0, wordData.Count)][Random.Range(1, 4 ).ToString()].ToString());

        mAutomateKR.SetKeyCode(_key);

        TextField = mAutomateKR.completeText + mAutomateKR.ingWord;
    }

    char[] keySequence(string originWord) // originWord를 입력하는데 필요한 key 문자 배열, keyTest 이름 배열을 반환
    {
        int[] ires = new int[15];
        char[] res = new char[15];
        char[] c = originWord.ToCharArray();
        int cho, jung, jong;
        int temp, count = 0;
        for (int i = 0; i < c.Length; i++)
        {
            // 한글 유니코드를 초성/중성/종성으로 분해
            temp = c[i] - 0xAC00;
            cho = temp / (28 * 21);
            jung = (temp % (28 * 21) / 28) + 19;
            jong = temp % 28 + 40;
            // Debug.LogFormat("{0} 초성 : {1} 중성 : {2} 종성 : {3}", c[i], s[cho], s[jung], s[jong]);

            // res에 정답 키 순서대로 저장
            ires[count] = cho;
            res[count++] = st[cho];
            int j = 0;
            do
            {
                if (mv[j, 2] == jung)
                {
                    ires[count] = mv[j, 0];
                    ires[count] = mv[j, 1];
                    res[count++] = st[mv[j, 0]];
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
                    ires[count] = mjc[j, 1];
                    res[count++] = st[mjc[j, 0]];
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

        // keyTest 객체 이름을 반환
        char[] keyTextName = new char[15];
        for (int i = 0; i < count; i++)
        {
            keyTextName[i] = AutomateKR.HANGULE_KEY_TABLE.FirstOrDefault(x => x.Value == ires[i]).Key;
        }
        Debug.LogFormat("{0}, {1}, {2}", originWord, res.ArrayToString(), keyTextName.ArrayToString());
        return res;        
    }
}
