﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Conversation : MonoBehaviour
{
    private enum Character { Computer, Player };

    [SerializeField] private string[] m_computerLines;
    [SerializeField] private string[] m_playerLines;
    public TextMeshProUGUI m_computerText;
    public TextMeshProUGUI m_playerText;

    // Start is called before the first frame update
    void Start()
    {
        DisplayLine(Character.Computer, 0);
    }

    private void DisplayLine(Character l_char, int l_index, float l_time = 1.5f)
    {
        StartCoroutine(FadeText(true, l_char, l_time));
    }

    IEnumerator FadeText(bool l_fadeIn, Character l_char, float l_time)
    {
        float time = Time.time;
        float until = Time.time + l_time;
        int alphaFrom = l_fadeIn ? 0 : 1;
        int alphaGoal = l_fadeIn ? 1 : 0;
        Color textColor;
        while (time < until)
        {
            if (l_char == Character.Computer)
            {
                textColor = m_computerText.color;
                textColor.a = Mathf.Lerp(alphaFrom, alphaGoal, Time.time / l_time);
                m_computerText.color = textColor;
            }
            else
            {
                textColor = m_playerText.color;
                textColor.a = Mathf.Lerp(alphaFrom, alphaGoal, Time.time / l_time);
                m_playerText.color = textColor;
            }
            
            time += Time.deltaTime;
            yield return null;
        }
        textColor.a = alphaGoal;
    }
}
