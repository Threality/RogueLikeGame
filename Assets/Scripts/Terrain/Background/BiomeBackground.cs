using System.Collections.Generic;
using UnityEngine;

public class BiomeBackground : MonoBehaviour
{
    public string biome;

    private List<SpriteRenderer> renderers = new List<SpriteRenderer>();

    private void Awake()
    {
        renderers.AddRange(GetComponentsInChildren<SpriteRenderer>());
    }

    public void SetOpacity(float opacity)
    {
        foreach (SpriteRenderer render in renderers)
        {
            Color colour = render.color;
            colour.a = opacity;
            render.color = colour;
        }
    }
}