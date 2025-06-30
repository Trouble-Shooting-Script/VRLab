using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKTarget : MonoBehaviour
{
    public Transform bindingBone;
    public new Renderer renderer;

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
    }

    private void OnEnable()
    {
        renderer.enabled = true;
    }

    private void OnDisable()
    {
        renderer.enabled = false;
    }
}
