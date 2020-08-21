using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Conversation : MonoBehaviour
{
    private enum Character { Computer, Player };

    [SerializeField] private string[] m_computerLines;
    [SerializeField] private string[] m_playerLines;
    [SerializeField] private GameObject m_wordContainer;
    private int m_currentLine = 0;
    
    public Image m_blackPanel;
    public Color m_notSelectedColor;
    public Color m_selectedColor;
    public RectTransform m_trailRT;
    public TextMeshProUGUI m_endingText;
    public TextMeshProUGUI m_computerText;
    public TextMeshProUGUI m_playerText;
    public float m_wordSpawnTime;
    public float m_wordScreenDistanceOffset;
    public float m_wordDistanceOffset;
    public float m_touchWordDistance;
    
    void Start()
    {
        StartCoroutine(RunConversation());
    }

    private IEnumerator RunConversation()
    {
        while (true)
        {
            if (m_computerLines.Length > m_currentLine)
            {
                yield return DisplayLine(Character.Computer, m_currentLine);
                yield return new WaitForSeconds(1.5f);
            }
            else
            {
                break;
            }

            if (m_playerLines.Length > m_currentLine)
            {
                StartCoroutine(FadePanel(true, m_blackPanel, 0.75f));
                yield return PlayerInput(m_playerLines[m_currentLine]);
                StartCoroutine(FadePanel(false, m_blackPanel, 0.75f));
                yield return DisplayLine(Character.Player, m_currentLine);
                yield return new WaitForSeconds(1);
            }
            else
            {
                break;
            }

            ++m_currentLine;
        }
        StartCoroutine(FadePanel(true, m_blackPanel, 0.75f));
        yield return FadeText(true, m_endingText, 1);
    }

    private IEnumerator DisplayLine(Character l_char, int l_index, float l_time = 1)
    {
        if (l_char == Character.Computer)
        {
            yield return FadeText(false, m_playerText, l_time);
            m_computerText.text = m_computerLines[l_index];
            yield return FadeText(true, m_computerText, l_time);
        }
        else
        {
            yield return FadeText(false, m_computerText, l_time);
            m_playerText.text = m_playerLines[l_index];
            yield return FadeText(true, m_playerText, l_time);
        }
    }

    private bool IsWordOverlapping(List<Vector3> l_list, Vector3 l_location)
    {
        foreach (var loc in l_list)
        {
            if (Mathf.Abs(l_location.x - loc.x) < m_wordDistanceOffset || Mathf.Abs(l_location.y - loc.y) < m_wordDistanceOffset)
            {
                return true;
            }
        }
        return false;
    }

    private IEnumerator PlayerInput(string l_line)
    {
        // SPAWN WORDS
        string[] words = l_line.Split(' ');
        List<Vector3> wordPositions = new List<Vector3>();
        List<TextMeshProUGUI> wordListUI = new List<TextMeshProUGUI>();
        for (int i = 0; i < words.Length; ++i)
        {
            Vector3 spawnLocation = new Vector3(Random.Range(m_wordScreenDistanceOffset, Screen.width - m_wordScreenDistanceOffset), Random.Range(m_wordScreenDistanceOffset, Screen.height - m_wordScreenDistanceOffset), 0);
            if (IsWordOverlapping(wordPositions, spawnLocation))
            {
                --i;
                continue;
            }
            wordPositions.Add(spawnLocation);
            GameObject wordContainer = Instantiate(m_wordContainer, transform.parent);
            wordContainer.transform.position = spawnLocation;
            TextMeshProUGUI tmpText = wordContainer.GetComponentInChildren<TextMeshProUGUI>();
            tmpText.color = m_notSelectedColor;
            wordListUI.Add(tmpText);
            tmpText.text = words[i];
            
            yield return FadeText(true, tmpText, 0.75f);
        }

        // CONNECT WORDS WITH INPUT
        int touchedWords = 0;
        Touch touch;
        while (touchedWords < words.Length)
        {
            if (Input.touchCount > 0)
            {
                touch = Input.GetTouch(0);
                m_trailRT.anchoredPosition = new Vector2(touch.position.x - Screen.width / 2, touch.position.y - Screen.height / 2);
                if (Mathf.Abs(touch.position.x - wordListUI[touchedWords].transform.position.x) < m_touchWordDistance && 
                    Mathf.Abs(touch.position.y - wordListUI[touchedWords].transform.position.y) < m_touchWordDistance)
                {
                    wordListUI[touchedWords].color = m_selectedColor;
                    touchedWords++;
                }
            }
            else
            {
                touchedWords = 0;
                foreach (TextMeshProUGUI word in wordListUI)
                {
                    word.color = m_notSelectedColor;
                }
            }
            yield return new WaitForSeconds(0.05f);
        }

        // DESPAWN WORDS
        foreach (var word in wordListUI)
        {
            yield return FadeText(false, word, 0.25f);
            Destroy(word.transform.parent.gameObject);
        }

    }

    private IEnumerator FadeText(bool l_fadeIn, TextMeshProUGUI l_textBox, float l_duration)
    {
        float start = Time.time;
        float until = start + l_duration;
        int alphaFrom = l_fadeIn ? 0 : 1;
        int alphaGoal = 1 - alphaFrom;
        Color textColor = l_textBox.color;

        while (Time.time < until)
        {
            textColor.a = Mathf.Lerp(alphaFrom, alphaGoal, (Time.time - start) / l_duration);
            l_textBox.color = textColor;
            yield return null;
        }
        textColor.a = alphaGoal;
        l_textBox.color = textColor;
    }

    private IEnumerator FadePanel(bool l_fadeIn, Image l_panel, float l_duration)
    {
        float start = Time.time;
        float until = start + l_duration;
        float alphaFrom = l_fadeIn ? 0 : 0.5f;
        float alphaGoal = 0.5f - alphaFrom;
        Color textColor = l_panel.color;

        while (Time.time < until)
        {
            textColor.a = Mathf.Lerp(alphaFrom, alphaGoal, (Time.time - start) / l_duration);
            l_panel.color = textColor;
            yield return null;
        }
        textColor.a = alphaGoal;
        l_panel.color = textColor;
    }
}
