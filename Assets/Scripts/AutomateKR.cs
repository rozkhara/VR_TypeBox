using UnityEngine;
using System.Collections;
using System.Collections.Generic;



//public class AutomateKR : MonoBehaviour {
public class AutomateKR
{

    public static int KEY_CODE_SPACE = -1;      // ����
    public static int KEY_CODE_ENTER = -2;      // �Ϸ� or Ŭ����
    public static int KEY_CODE_BACKSPACE = -3;      // �����

    public static Dictionary<char, int> HANGULE_KEY_TABLE = new Dictionary<char, int>
    {
        {'Q', 8},
        {'W', 13},
        {'E', 4},
        {'R', 1},
        {'T', 10},
        {'O', 22},
        {'P', 26},

        {'q', 7},
        {'w', 12},
        {'e', 3},
        {'r', 0},
        {'t', 9},
        {'y', 31},
        {'u', 25},
        {'i', 21},
        {'o', 20},
        {'p', 24},

        {'a', 6},
        {'s', 2},
        {'d', 11},
        {'f', 5},
        {'g', 18},
        {'h', 27},
        {'j', 23},
        {'k', 19},
        {'l', 39},

        {'z', 15},
        {'x', 16},
        {'c', 14},
        {'v', 17},
        {'b', 36},
        {'n', 32},
        {'m', 37},

        //{'B', -3},
        //{'S', -1},
        //{'C', -2},
    };

    // �ʼ�, �߼�, ���� ���̺�.
    public static string SOUND_TABLE =
    /* �ʼ� 19�� 0 ~ 18 */
    "��������������������������������������" +
    /* �߼� 21�� 19 ~ 39 */
    "�������¤äĤŤƤǤȤɤʤˤ̤ͤΤϤФѤҤ�" +
    /* ���� 28�� 40 ~ 67 */
    " ������������������������������������������������������";

    public enum HAN_STATUS		// �ܾ����ջ���
    {
        HS_FIRST = 0,		// �ʼ�
        HS_FIRST_V,			// ���� + ���� 
        HS_FIRST_C,			// ���� + ����
        HS_MIDDLE_STATE,	// �ʼ� + ���� + ����
        HS_END,				// �ʼ� + �߼� + ����
        HS_END_STATE,		// �ʼ� + �߼� + ���� + ����
        HS_END_EXCEPTION	// �ʼ� + �߼� + ����(������)
    };

    public string ingWord;		    // �ۼ��� ����
    public string completeText;	    // �ϼ� ���ڿ�

    const int BASE_CODE = 0xac00;		// ��������(��)
    const int LIMIT_MIN = 0xac00;		// �������� MIN(��)
    const int LIMIT_MAX = 0xd7a3;		// �������� MAX

    HAN_STATUS m_nStatus;               // �ܾ����ջ���
    int[] m_nPhonemez = new int[5];   // ����[��,��,��,������1,������2]

    string m_completeWord;	// �ϼ�����


    /*
    // �ʼ� �ռ� ���̺�
    int[,] MIXED_CHO_CONSON = new int[14, 3]
    {
        { 0, 0,15}, // ��,��,��
	    {15, 0, 1}, // ��,��,��
	    { 1, 0, 0}, // ��,��,��

	    { 3, 3,16}, // ��,��,��
	    {16, 3, 4}, // ��,��,��
	    { 4, 3, 3}, // ��,��,��

	    { 7, 7,17}, // ��,��,��
	    {17, 7, 8}, // ��,��,��
	    { 8, 7, 7}, // ��,��,��

	    { 9, 9,10}, // ��,��,��
	    {10, 9, 9}, // ��,��,��

	    {12,12,14}, // ��,��,��
	    {14,12,13}, // ��,��,��
	    {13,12,12}  // ��,��,��
    };
    */

    // �ʼ�,�߼� ���� �ռ� ���̺�
    public static int[,] MIXED_VOWEL = new int[7, 3] {
        {27,19,28},	// ��,��,��
        {27,20,29},	// ��,��,��
	    {27,39,30},	// ��,��,��

	    {32,23,33},	// ��,��,��
        {32,24,34}, // ��,��,��

	    {32,39,35},	// ��,��,��

	    {37,39,38},	// ��,��,��
    };

    // ���� �ռ� ���̺�
    public static int[,] MIXED_JONG_CONSON = new int[11, 3] {
        {41,59,43}, // ��,��,�� 
	    {44,62,45}, // ��,��,��
	    {44,67,46}, // ��,��,�� 
	    {48,41,49}, // ��,��,��
	    {48,56,50}, // ��,��,�� 
	    {48,57,51}, // ��,��,��
	    {51,57,54}, // ��,��,�� 
	    {48,59,52}, // ��,��,��
	    {48,65,53}, // ��,��,��	
	    {48,67,55}, // ��,��,�� 
	    {57,59,58}, // ��,��,��
    };

    /*
    // ���� ���� ���̺� -> MIXED_JONG_CONSON�� ����
    int[,] DIVIDE_JONG_CONSON = new int[11, 3] {
	    {41,59,43}, // ��,��,��
	    {44,62,45}, // ��,��,��
	    {44,67,46}, // ��,��,��
	    {48,41,49}, // ��,��,��
	    {48,56,50}, // ��,��,��
	    {48,57,51}, // ��,��,��
	    {48,66,54}, // ��,��,��
	    {48,59,52}, // ��,��,��
	    {48,65,53}, // ��,��,��	
	    {48,67,55}, // ��,��,��
	    {57,59,58}, // ��,��,��
        
    };
    */

    int currentCode;// ������ ��ü

    // ���� �ʱ�ȭ
    public void Clear()
    {
        m_nStatus = HAN_STATUS.HS_FIRST;
        completeText = "";
        ingWord = null;
        m_completeWord = null;
    }

    public HAN_STATUS GetStatus()
    {
        return m_nStatus;
    }

    public string SetKeyCode(char _key)
    {
        return SetKeyCode(HANGULE_KEY_TABLE[_key]);
    }


    public string SetKeyCode(int nKeyCode)
    {
        m_completeWord = null;

        // Ư��Ű �Է�
        if (nKeyCode < 0)
        {
            m_nStatus = HAN_STATUS.HS_FIRST;

            if (nKeyCode == KEY_CODE_SPACE) // ����
            {
                if (ingWord != null)
                {
                    completeText += ingWord;
                    completeText += " ";
                }
                else
                    completeText += " ";

                ingWord = null;
            }
            else if (nKeyCode == KEY_CODE_ENTER)
            {
                // �ۼ��� text�� ����(completeText) & clear
                if (ingWord != null)
                    completeText += ingWord;

                // ���� üũ �Լ�
                // Debug.Log(completeText);
            }
            else if (nKeyCode == KEY_CODE_BACKSPACE) // �����
            {
                if (ingWord == null) // ���� ���ڰ� ������
                {
                    if (completeText != null)
                    {
                        if (completeText.Length > 0)
                        {
                            int n = completeText.LastIndexOf("\n");
                            if (n == -1)
                                completeText = completeText.Remove(completeText.Length - 1);
                            else
                                completeText = completeText.Remove(completeText.Length - 2);
                        }
                    }
                }
                else
                {
                    m_nStatus = DownGradeIngWordStatus(ingWord);
                }
            }

            return m_completeWord;
        }


        // Ư��Ű�� �ƴ� ���
        switch (m_nStatus)
        {
            case HAN_STATUS.HS_FIRST:// �ʼ�
                // �ʼ�
                m_nPhonemez[0] = nKeyCode;
                ingWord = new string(SOUND_TABLE[m_nPhonemez[0]], 1);
                m_nStatus = (nKeyCode > 18) ? HAN_STATUS.HS_FIRST_C : HAN_STATUS.HS_FIRST_V;
                break;



            case HAN_STATUS.HS_FIRST_C:// ���� + ����
                // ���� + ����
                m_completeWord = new string(SOUND_TABLE[m_nPhonemez[0]], 1);
                m_nPhonemez[0] = nKeyCode;
                if (nKeyCode > 18)	// ����
                {

                }
                else				// ����
                {

                    m_nStatus = HAN_STATUS.HS_FIRST_V;
                }
                break;

            case HAN_STATUS.HS_FIRST_V:// ���� + ���� 
                // ���� + ����
                if (nKeyCode > 18)	// ����
                {
                    m_nPhonemez[1] = nKeyCode;
                    m_nStatus = HAN_STATUS.HS_MIDDLE_STATE;
                }
                else // ����
                {
                    //	if(!MixInitial(nKeyCode))
                    {
                        m_completeWord = new string(SOUND_TABLE[m_nPhonemez[0]], 1);
                        m_nPhonemez[0] = nKeyCode;
                        m_nStatus = HAN_STATUS.HS_FIRST_V;
                    }
                }
                break;

            case HAN_STATUS.HS_MIDDLE_STATE:// �ʼ� + ���� + ����
                // �ʼ� + ���� + ����
                if (nKeyCode > 18)
                {
                    if (MixVowel(m_nPhonemez[1], nKeyCode) == false) // ����+���� �ռ� �ȵɶ� 
                    {
                        m_completeWord = CombineHangle(1);
                        m_nPhonemez[0] = nKeyCode;
                        m_nStatus = HAN_STATUS.HS_FIRST_C;
                    }
                    else
                    {
                        m_nPhonemez[1] = currentCode; // �߼� ����+���� �ռ�
                        m_nStatus = HAN_STATUS.HS_MIDDLE_STATE;
                    }
                }
                else // ������ ���ö� 
                {
                    int jungCode = ToFinal(nKeyCode);

                    if (jungCode > 0)
                    {
                        m_nPhonemez[2] = jungCode;
                        m_nStatus = HAN_STATUS.HS_END_STATE;
                    }
                    else
                    {
                        m_completeWord = CombineHangle(1);
                        m_nPhonemez[0] = nKeyCode;
                        m_nStatus = (nKeyCode > 18) ? HAN_STATUS.HS_FIRST_C : HAN_STATUS.HS_FIRST_V;
                        //m_nStatus = HAN_STATUS.HS_FIRST;
                    }
                }
                break;

            case HAN_STATUS.HS_END:// �ʼ� + �߼� + ����

                if (nKeyCode > 18)
                {
                    m_completeWord = CombineHangle(1);
                    m_nPhonemez[0] = nKeyCode;
                    m_nStatus = HAN_STATUS.HS_FIRST;
                }
                else
                {
                    int jungCode = ToFinal(nKeyCode);
                    if (jungCode > 0)
                    {
                        m_nPhonemez[2] = jungCode;
                        m_nStatus = HAN_STATUS.HS_END_STATE;
                    }
                    else
                    {
                        m_completeWord = CombineHangle(1);
                        m_nPhonemez[0] = nKeyCode;
                        m_nStatus = HAN_STATUS.HS_FIRST;
                    }
                }
                break;

            case HAN_STATUS.HS_END_STATE:
                // �ʼ� + �߼� + ����(��) + ����(��)
                if (nKeyCode > 18)
                {
                    m_completeWord = CombineHangle(1);

                    m_nPhonemez[0] = ToInitial(m_nPhonemez[2]);
                    m_nPhonemez[1] = nKeyCode;
                    m_nStatus = HAN_STATUS.HS_MIDDLE_STATE;
                }
                else
                {
                    int jungCode = ToFinal(nKeyCode);
                    if (jungCode > 0)
                    {
                        m_nStatus = HAN_STATUS.HS_END_EXCEPTION;

                        if (!MixFinal(jungCode)) // ���� ���� �ռ� ����
                        {
                            m_completeWord = CombineHangle(2);
                            m_nPhonemez[0] = nKeyCode;
                            m_nStatus = HAN_STATUS.HS_FIRST_V;
                        }
                    }
                    else
                    {
                        m_completeWord = CombineHangle(2);
                        m_nPhonemez[0] = nKeyCode;
                        m_nStatus = HAN_STATUS.HS_FIRST_V;
                    }
                }
                break;

            case HAN_STATUS.HS_END_EXCEPTION:
                // �ʼ� + �߼� + ����(������)
                if (nKeyCode > 18)
                {
                    DecomposeConsonant();
                    m_nPhonemez[1] = nKeyCode;
                    m_nStatus = HAN_STATUS.HS_MIDDLE_STATE;
                }
                else
                {
                    int jungCode = ToFinal(nKeyCode);
                    if (jungCode > 0)
                    {
                        m_nStatus = HAN_STATUS.HS_END_EXCEPTION;

                        if (!MixFinal(jungCode))
                        {
                            m_completeWord = CombineHangle(2);
                            m_nPhonemez[0] = nKeyCode;
                            m_nStatus = HAN_STATUS.HS_FIRST_V;
                        }
                    }
                    else
                    {
                        m_completeWord = CombineHangle(2);
                        m_nPhonemez[0] = nKeyCode;
                        m_nStatus = HAN_STATUS.HS_FIRST_V;
                    }
                }
                break;
        }

        // ���� ���̴� ���ڻ���
        CombineIngWord(m_nStatus);

        // �ϼ� ���ڿ� �����
        if (m_completeWord != null)
            completeText += m_completeWord;

        return m_completeWord;
    }


    // �ʼ����� ��ȯ
    public int ToInitial(int nKeyCode)
    {
        switch (nKeyCode)
        {
            case 41: return 0;  // ��
            case 42: return 1;  // ��
            case 44: return 2;  // ��
            case 47: return 3;  // ��
            case 48: return 5;  // ��
            case 56: return 6;  // ��
            case 57: return 7;  // ��
            case 59: return 9;  // ��
            case 60: return 10; // ��
            case 61: return 11; // ��
            case 62: return 12; // ��
            case 63: return 14; // ��
            case 64: return 15; // ��
            case 65: return 16; // ��
            case 66: return 17; // ��
            case 67: return 18; // ��
        }
        return -1;
    }

    // �������� ��ȯ
    int ToFinal(int nKeyCode)
    {
        switch (nKeyCode)
        {
            case 0: return 41;  // ��
            case 1: return 42;  // ��
            case 2: return 44;  // ��
            case 3: return 47;  // ��
            case 5: return 48;  // ��
            case 6: return 56;  // ��
            case 7: return 57;  // ��
            case 9: return 59;  // ��
            case 10: return 60; // ��
            case 11: return 61; // ��
            case 12: return 62; // ��
            case 14: return 63; // ��
            case 15: return 64; // ��
            case 16: return 65; // ��
            case 17: return 66; // ��
            case 18: return 67; // ��
        }
        return -1;
    }

    // ��������
    void DecomposeConsonant()
    {
        int i = 0;
        if (m_nPhonemez[3] > 40 || m_nPhonemez[4] > 40)
        {
            do
            {
                if (MIXED_JONG_CONSON[i, 2] == m_nPhonemez[2])
                {
                    m_nPhonemez[3] = MIXED_JONG_CONSON[i, 0];
                    m_nPhonemez[4] = MIXED_JONG_CONSON[i, 1];

                    m_completeWord = CombineHangle(3);
                    m_nPhonemez[0] = ToInitial(m_nPhonemez[4]);
                    return;
                }
            }
            while (++i < 11);
        }

        m_completeWord = CombineHangle(1);
        m_nPhonemez[0] = ToInitial(m_nPhonemez[2]);
    }

    /*
    // �ʼ��ռ�
    bool MixInitial(int nKeyCode)
    {
        if (nKeyCode >= 19)
            return false;

        int i = 0;
        do
        {
            if (MIXED_CHO_CONSON[i, 0] == m_nPhonemez[0] && MIXED_CHO_CONSON[i, 1] == nKeyCode)
            {
                m_nPhonemez[0] = MIXED_CHO_CONSON[i, 2];
                return true;
            }
        }
        while (++i < 14);

        return false;
    }
    */

    // �����ռ�
    bool MixFinal(int nKeyCode)
    {
        if (nKeyCode <= 40) return false;

        int i = 0;
        do
        {
            if (MIXED_JONG_CONSON[i, 0] == m_nPhonemez[2] && MIXED_JONG_CONSON[i, 1] == nKeyCode)
            {
                m_nPhonemez[3] = m_nPhonemez[2];
                m_nPhonemez[4] = nKeyCode;
                m_nPhonemez[2] = MIXED_JONG_CONSON[i, 2];

                return true;
            }
        }
        while (++i < 11);

        return false;
    }

    // �����ռ�
    bool MixVowel(int currentC, int inputCode)
    {
        currentCode = currentC;
        int i = 0;
        do
        {
            if (MIXED_VOWEL[i, 0] == currentCode && MIXED_VOWEL[i, 1] == inputCode)
            {
                //                print(MIXED_VOWEL[i, 2]);
                currentCode = MIXED_VOWEL[i, 2];
                return true;
            }
        }
        while (++i < 7);

        return false;
    }


    // �ѱ����� Check
    string CombineHangle(int cho, int jung, int jong)
    {
        // �ʼ� * 21 * 28 + (�߼� - 19) * 28 + (���� - 40) + BASE_CODE;
        return new string(System.Convert.ToChar(BASE_CODE - 572 + jong + cho * 588 + jung * 28), 1);
    }

    string CombineHangle(int status)
    {
        switch (status)
        {
            //�ʼ� + �߼�
            case 1:
                return CombineHangle(m_nPhonemez[0], m_nPhonemez[1], 40);

            //�ʼ� + �߼� + ����
            case 2:
                return CombineHangle(m_nPhonemez[0], m_nPhonemez[1], m_nPhonemez[2]);

            //�ʼ� + �߼� + ������01
            case 3:
                return CombineHangle(m_nPhonemez[0], m_nPhonemez[1], m_nPhonemez[3]);
        }
        return "";
    }

    void CombineIngWord(HAN_STATUS status)
    {
        switch (m_nStatus)
        {
            case HAN_STATUS.HS_FIRST:
            case HAN_STATUS.HS_FIRST_V:
            case HAN_STATUS.HS_FIRST_C:
                ingWord = new string(SOUND_TABLE[m_nPhonemez[0]], 1);
                break;

            case HAN_STATUS.HS_MIDDLE_STATE:
            case HAN_STATUS.HS_END:
                ingWord = CombineHangle(1);
                break;

            case HAN_STATUS.HS_END_STATE:
            case HAN_STATUS.HS_END_EXCEPTION:
                ingWord = CombineHangle(2);
                break;
        }
    }

    // �ۼ����� �ѱ��� ������� ����
    HAN_STATUS DownGradeIngWordStatus(string word)
    {
        //        print("target : " + word + " : "+ word.Length);
        //print("Convert : " + );


        int iWord = System.Convert.ToInt32(char.Parse(word));

        //�ʼ��� �ִ� ���
        if (iWord < LIMIT_MIN || iWord > LIMIT_MAX)
        {
            ingWord = null;
            return HAN_STATUS.HS_FIRST;
        }

        // �����ڵ� ü��
        // iWord = firstWord * (21*28)
        //		+ middleWord * 28
        //		+ lastWord * 27
        //		+ BASE_CODE;
        //

        int totalWord = iWord - BASE_CODE;
        int iFirstWord = totalWord / 28 / 21;	//�ʼ�
        int iMiddleWord = totalWord / 28 % 21;	//�߼�
        int iLastWord = totalWord % 28;		//����

        m_nPhonemez[0] = iFirstWord; //�ʼ�����

        if (iLastWord == 0)	//������ ���� ���
        {
            ingWord = new string(SOUND_TABLE[m_nPhonemez[0]], 1);
            return HAN_STATUS.HS_FIRST_V;
        }

        m_nPhonemez[1] = iMiddleWord + 19; //�߼�����

        iLastWord += 40;

        for (int i = 0; i < 11; i++)
        {

            if (iLastWord == MIXED_JONG_CONSON[i, 2])
            {
                ingWord = CombineHangle(3);
                m_nPhonemez[2] = MIXED_JONG_CONSON[i, 0]; // ��������
                return HAN_STATUS.HS_END_EXCEPTION;
            }
        }
        ingWord = CombineHangle(1);

        return HAN_STATUS.HS_MIDDLE_STATE;
    }
}