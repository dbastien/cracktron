using TMPro;
using UnityEngine;

public class TextMeshScroll : MonoBehaviour
{
    [Range(0.01f, 1.0f)] public float ScrollSpeed = 0.08f;

    private TMP_Text textComponent;
    private RectTransform rectComponent;
    private float scrollPosition;

    void Update()
    {
        if (textComponent == null)
        {
            textComponent = gameObject.GetComponent<TMP_Text>();

            textComponent.havePropertiesChanged = true; // Need to force the TextMeshPro Object to be updated.
            //textComponent.ForceMeshUpdate(); // Generate the mesh and populate the textInfo with data we can use and manipulate.
        }

        if (rectComponent == null)
        {
            rectComponent = gameObject.GetComponent<RectTransform>();
        }

        scrollPosition += ScrollSpeed * Time.deltaTime;
        scrollPosition %= 1.0f;

        var xPos = -Mathf.Lerp(-rectComponent.rect.width, textComponent.preferredWidth + rectComponent.rect.width, scrollPosition);

        rectComponent.position = new Vector3(xPos,
                                             rectComponent.position.y,
                                             rectComponent.position.z);
    }
}
