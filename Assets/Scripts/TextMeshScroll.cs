﻿using TMPro;
using UnityEngine;

public class TextMeshScroll : MonoBehaviour
{
    [Range(0.01f, 1.0f)] public float ScrollSpeed = 0.08f;

    private TMP_Text textComponent;
    private RectTransform rectComponent;
    private float scrollPosition;

    public void Update()
    {
        if (this.textComponent == null)
        {
            this.textComponent = this.gameObject.GetComponent<TMP_Text>();

            this.textComponent.havePropertiesChanged = true; // Need to force the TextMeshPro Object to be updated.
            //textComponent.ForceMeshUpdate(); // Generate the mesh and populate the textInfo with data we can use and manipulate.
        }

        if (this.rectComponent == null)
        {
            this.rectComponent = this.gameObject.GetComponent<RectTransform>();
        }

        this.scrollPosition += this.ScrollSpeed * Time.deltaTime;
        this.scrollPosition %= 1.0f;

        var xPos = -Mathf.Lerp(-this.rectComponent.rect.width, this.textComponent.preferredWidth + this.rectComponent.rect.width, this.scrollPosition);

        this.rectComponent.position = new Vector3(xPos,
                                                  this.rectComponent.position.y,
                                                  this.rectComponent.position.z);
    }
}
