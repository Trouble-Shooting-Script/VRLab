using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SpawnEffect : MonoBehaviour
{
    [SerializeField] private Renderer m_Renderer;
    [SerializeField] private Material m_TrlOrg;
    [SerializeField] private Material m_Dissolve;
    [SerializeField] private Material m_TrlPhase;
    [SerializeField] private float m_FadeTime = 2.0f;
    [SerializeField] private bool m_IsDissolve;

    private void Start()
    {
        if (m_IsDissolve)
        {
            m_Renderer.material = m_Dissolve;
            DoFade(0, 1, m_FadeTime);
        }
        else
        {
            m_Renderer.material = m_TrlPhase;
            DoFade(0, 2, m_FadeTime);
        }
    }

    void DoFade(float start, float dest, float time)
    {
        iTween.ValueTo(gameObject, iTween.Hash("from", start, "to", dest, "time", time, "onupdatetarget", gameObject, "onupdate", "TweenOnUpdate", "oncomplete", "TweenOnComplete",
            "easetype", iTween.EaseType.easeInOutCubic));
    }

    void TweenOnUpdate(float value)
    {
        m_Renderer.material.SetFloat("_Split", value);
    }

    void TweenOnComplete()
    {
        m_Renderer.material = m_TrlOrg;
    }
}
