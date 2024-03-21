using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlight : MonoBehaviour
{
    [SerializeField] private List<Renderer> renderers;
    [SerializeField] private Color color = Color.yellow;
    [SerializeField] private float emissionIntensity = 0.2f;
    
    private List<Material> materials;
    
    private void Awake()
    {
        //renderers = new List<Renderer>();
        materials = new List<Material>();
        //renderers.Add(GetComponent<MeshRenderer>());
        foreach (var renderer in renderers)
        {
            materials.AddRange(new List<Material>(renderer.materials));
        }
    }
    
    public void ToggleHighlight(bool val)
    {
        if (val)
        {
            foreach (var material in materials)
            {
                material.EnableKeyword("_EMISSION");
                Color newEmissionColor = color * emissionIntensity;
                material.SetColor("_EmissionColor", newEmissionColor);
            }
        }
        else
        {
            foreach (var material in materials)
            {
                material.DisableKeyword("_EMISSION");
            }
        }
    }
}