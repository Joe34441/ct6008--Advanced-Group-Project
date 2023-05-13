using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class MaterialSetter : MonoBehaviour
{
    public Color newColour;

    public List<Material> matsToChange;

    public int id;

    private void Awake()
    {
        SkinnedMeshRenderer _renderer = GetComponent<SkinnedMeshRenderer>();
        for(int i = 2; i <= 8; i++)
        {
            matsToChange[i - 2] = _renderer.materials[i];
            //_renderer.materials[i] = matsToChange[i - 2];
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public void SetMats()
    {
        for (int i = 0; i < matsToChange.Count; i++)
        {
            matsToChange[i].SetColor("_New_Colour", newColour);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
