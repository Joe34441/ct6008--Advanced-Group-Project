using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private RectTransform backgroundImage;
    [SerializeField]
    private float movementSpeed = 1.0f;
    private Vector2 imageStartPos;

    // Start is called before the first frame update
    void Start()
    {
        imageStartPos = backgroundImage.anchoredPosition;
    }

    // Update is called once per frame
    void Update()
    {
        backgroundImage.anchoredPosition += Vector2.right * Time.deltaTime * movementSpeed;
    }
}
