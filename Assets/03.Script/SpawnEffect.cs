using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class SpawnEffect : MonoBehaviour
{
    [SerializeField] private Renderer m_Renderer;
    [SerializeField] private Material[] m_OrgMats;
    [SerializeField] private Material[] m_DissolveMats;
    [SerializeField] private Material[] m_PhaseMats;
    [SerializeField] private float m_FadeTime = 3.0f;
    [SerializeField] private bool m_IsDissolve;
    [SerializeField] private GameObject[] m_EnableObjects;
    private Material[] m_CDissolveMats;
    private bool m_IsDestroy = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            DestroyObject();
        }
    }

    private void Awake()
    {
        m_CDissolveMats = m_DissolveMats;
        if (m_IsDissolve)
        {
            m_Renderer.materials = m_CDissolveMats;
            DoFade(0, 1, m_FadeTime);
        }
        else
        {
            m_Renderer.materials = m_PhaseMats;
            DoFade(0, 2, m_FadeTime);
        }
    }

    private void DestroyObject()
    {
        m_CDissolveMats = m_DissolveMats;
        m_Renderer.materials = m_CDissolveMats;
        DoFade(4, 0, m_FadeTime);
        m_IsDestroy = true;
    }


    void DoFade(float start, float dest, float time)
    {
        iTween.ValueTo(gameObject, iTween.Hash("from", start, "to", dest, "time", time, 
            "onupdatetarget", gameObject, "onupdate", "TweenOnUpdate", "oncomplete", "TweenOnComplete",
            "easetype", iTween.EaseType.easeInOutCubic));
    }

    void TweenOnUpdate(float value)
    {
        foreach (var mat in m_Renderer.materials)
        {
            mat.SetFloat("_Split", value);
        }
    }

    void TweenOnComplete()
    {
        m_Renderer.materials = m_OrgMats;
        foreach (GameObject obj in m_EnableObjects)
        {
            obj.SetActive(true);
        }
        if (m_IsDestroy)
        {
            Destroy(gameObject);
        }
    }
    /*[SerializeField] private Renderer m_Renderer;
    [SerializeField] private Material m_OrgMat;
    [SerializeField] private Material m_Dissolve;
    [SerializeField] private Material m_Phase;
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
            m_Renderer.material = m_Phase;
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
        m_Renderer.material = m_OrgMat;
    }*/
}
