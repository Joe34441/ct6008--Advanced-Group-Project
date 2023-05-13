using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialSetter : MonoBehaviour
{
    public Color newColour;

    public List<Material> matsToChange;

    // Start is called before the first frame update
    void Start()
    {
        for(int i = 0; i < matsToChange.Count; i++)
        {
            matsToChange[i].SetColor("_New_Colour", newColour);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
