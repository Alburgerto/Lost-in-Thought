using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Conversation : MonoBehaviour
{
    private enum Character { Computer, Player };

    [SerializeField] private string[] m_computerLines;
    [SerializeField] private string[] m_playerLines;
    [SerializeField] private GameObject m_wordContainer;

    public TextMeshProUGUI m_computerText;
    public TextMeshProUGUI m_playerText;
    public float m_wordSpawnTime;
    public float m_wordScreenDistanceOffset;
    public float m_wordDistanceOffset;


    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(DisplayLine(Character.Computer, 0));
    }

    IEnumerator DisplayLine(Character l_char, int l_index, float l_time = 1.5f)
    {
        //if (m_computerText.text != "")
        //{
        //    yield return StartCoroutine(FadeText(false, m_computerText, l_time));
        //}
        yield return StartCoroutine(SpawnWordPopups("ola ke ase"));
        if (l_char == Character.Computer)
        {
            m_computerText.text = m_computerLines[l_index];
            StartCoroutine(FadeText(true, m_computerText, l_time));

        }
        else
        {
            m_playerText.text = m_playerLines[l_index];
            StartCoroutine(FadeText(true, m_playerText, l_time));

        }
    }

    private bool IsWordOverlapping(List<Vector3> l_list, Vector3 l_location)
    {
        foreach (var loc in l_list)
        {
            if (Mathf.Abs(l_location.x - loc.x ) > m_wordDistanceOffset || Mathf.Abs(l_location.y - loc.y) > m_wordDistanceOffset)
            {
                return true;
            }
        }
        return false;
    }

    IEnumerator SpawnWordPopups(string l_line)
    {
        string[] words = l_line.Split(' ');
        List<Vector3> wordPositions = new List<Vector3>(); 
        for (int i = 0; i < words.Length; ++i)
        {
            Vector3 spawnLocation = new Vector3(Random.Range(m_wordScreenDistanceOffset, Screen.width - m_wordScreenDistanceOffset), Random.Range(m_wordScreenDistanceOffset, Screen.height - m_wordScreenDistanceOffset), 0);
            //if (IsWordOverlapping(wordPositions, spawnLocation))
            //{
            //    Debug.Log(i);
            //    continue;
            //}
            //wordPositions.Add(spawnLocation);
            Debug.Log(wordPositions.Count);
            GameObject wordContainer = Instantiate(m_wordContainer, transform.parent);
            wordContainer.transform.position = spawnLocation;
            TextMeshProUGUI tmpText = wordContainer.GetComponentInChildren<TextMeshProUGUI>();
            tmpText.text = words[i];

            yield return FadeText(true, tmpText, 2);

            yield return new WaitForSeconds(m_wordSpawnTime);
        }
    }

    IEnumerator FadeText(bool l_fadeIn, TextMeshProUGUI l_textBox, float l_time)
    {
        float time = Time.time;
        float until = Time.time + l_time;
        int alphaFrom = l_fadeIn ? 0 : 1;
        int alphaGoal = l_fadeIn ? 1 : 0;
        Color textColor = l_textBox.color;
        textColor.a = l_fadeIn ? 0 : 1;
        l_textBox.color = textColor;

        while (time < until)
        {
            textColor = l_textBox.color;
            textColor.a = Mathf.Lerp(alphaFrom, alphaGoal, Time.time / l_time);
            l_textBox.color = textColor;
            time += Time.deltaTime;
            yield return null;
        }
        textColor.a = l_fadeIn ? 1 : 0;
        l_textBox.color = textColor;

    }
}
