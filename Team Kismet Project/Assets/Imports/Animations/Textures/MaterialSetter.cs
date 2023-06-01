using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class MaterialSetter : MonoBehaviour
{
    public Color newColour;

    public List<Material> matsToChange;

    public int id;

    private SkinnedMeshRenderer _renderer;

    private void Awake()
    {
        _renderer = GetComponent<SkinnedMeshRenderer>();
        //the materials that will be changed are in the index 2,3,4,5,6,7,8, so start at 2 and increase from there
        for(int i = 2; i <= 8; i++)
        {
            //the mats to change starts at 0, so just -2 to keep it the same as the renderer materials list
            matsToChange[i - 2] = _renderer.materials[i];
        }
    }

    public void SetMats()
    {
        //loop through the materials and set the colour property on the shader
        for (int i = 0; i < matsToChange.Count; i++)
        {
            matsToChange[i].SetColor("_New_Colour", newColour);
        }
    }

    public void SetTagged()
    {
        //set the bool for tagged on the shader to change the fresnel colour
        for(int i = 0; i < _renderer.materials.Length; i++)
        {
            _renderer.materials[i].SetInt("_IsTagged", 1);
        }
    }

    public void SetUnTagged()
    {
        //set the fresnel colour back on the shader
        for (int i = 0; i < _renderer.materials.Length; i++)
        {
            _renderer.materials[i].SetInt("_IsTagged", 0);
        }
    }

    public void SetInvisible()
    {
        //set another bool on the shader to change the opacity to 0
        for (int i = 0; i < _renderer.materials.Length; i++)
        {
            _renderer.materials[i].SetInt("_IsInvisible", 1);
        }
    }

    public void SetVisible()
    {
        //changes the opacity back to 1
        for (int i = 0; i < _renderer.materials.Length; i++)
        {
            _renderer.materials[i].SetInt("_IsInvisible", 0);
        }
    }

}
