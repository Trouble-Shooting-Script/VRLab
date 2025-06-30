using System.Collections;
using System.Collections.Generic;
using PaintIn3D;
using UnityEngine;

public class CreateSpray : MonoBehaviour
{
    public GameObject sprayPrefab;
    public Transform spawnPoint;
    public Material material;
    
    public void CreateSprayObject()
    {
        if (sprayPrefab != null)
        {
            GameObject spray = Instantiate(sprayPrefab, spawnPoint.position, Quaternion.identity);
            ParticleSystem particleComponent = spray.GetComponentInChildren<ParticleSystem>();
            CwPaintSphere paintSphere = spray.GetComponentInChildren<CwPaintSphere>();

            // ParticleSystem의 MainModule을 가져와서 startColor를 설정
            var main = particleComponent.main;
            main.startColor = material.color;
            paintSphere.Color = material.color;
            spray.transform.SetParent(transform);
        }
        else
        {
            Debug.LogError("Spray prefab is not assigned.");
        }
    }
}
