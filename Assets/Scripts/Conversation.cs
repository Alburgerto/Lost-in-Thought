using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Conversation : MonoBehaviour
{
    private enum Character { Computer, Player };

    [SerializeField] private string[] m_computerLines;
    [SerializeField] private string[] m_playerLines;
    [SerializeField] private TextMeshProUGUI m_wordPopup;

    public TextMeshProUGUI m_computerText;
    public TextMeshProUGUI m_playerText;
    public float m_wordSpawnTime;
    public float m_wordSpacingOffset;

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

    IEnumerator SpawnWordPopups(string l_line)
    {
        string[] words = l_line.Split(' ');
        foreach (var word in words)
        {
            Vector3 spawnLocation = new Vector3(Random.Range(m_wordSpacingOffset, Screen.width - m_wordSpacingOffset), Random.Range(m_wordSpacingOffset, Screen.height - m_wordSpacingOffset), 0);
            Debug.Log(spawnLocation);
            //TextMeshProUGUI wordPopup = Instantiate(m_wordPopup, transform.parent);
            GameObject textParent = Instantiate(new GameObject(), transform.parent);
            TextMeshProUGUI wordPopup = Instantiate(m_wordPopup, textParent.transform);
            textParent.name = "WordContainer";
            textParent.transform.position = spawnLocation;
            wordPopup.text = word;
            wordPopup.GetComponent<Animation>().Play();
            yield return new WaitForSeconds(m_wordSpawnTime);
        }
    }

    IEnumerator FadeText(bool l_fadeIn, TextMeshProUGUI l_textBox, float l_time)
    {
        float time = Time.time;
        float until = Time.time + l_time;
        int alphaFrom = l_fadeIn ? 0 : 1;
        int alphaGoal = l_fadeIn ? 1 : 0;
        Color textColor;
        while (time < until)
        {
            textColor = l_textBox.color;
            textColor.a = Mathf.Lerp(alphaFrom, alphaGoal, Time.time / l_time);
            m_playerText.color = textColor;

            time += Time.deltaTime;
            yield return null;
        }
    }
}
