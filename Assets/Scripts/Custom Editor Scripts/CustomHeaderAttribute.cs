using UnityEngine;

public class CustomHeaderAttribute : PropertyAttribute
{
    public string headerText;
    public Color color;

    public CustomHeaderAttribute(string headerText)
    {
        this.headerText = headerText;
        this.color = new Color(1f, .5f, 0f);
    }
}