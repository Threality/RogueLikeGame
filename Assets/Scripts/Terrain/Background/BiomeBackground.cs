using System.Collections.Generic;
using UnityEngine;

public class BiomeBackground : MonoBehaviour
{
    // The name of the biome
    public string biome;

    private List<SpriteRenderer> renderers = new List<SpriteRenderer>();

    private void Start()
    {
        // Add all of the sprites used for the background
        renderers.AddRange(GetComponentsInChildren<SpriteRenderer>());
    }

    public void SetOpacity(float opacity)
    {
        foreach (SpriteRenderer render in renderers)
        {
            // Change the opacity of each sprite
            Color colour = render.color;
            colour.a = opacity;
            render.color = colour;
        }
    }
}