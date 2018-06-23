using UnityEngine;

public class SnakeStepData : MonoBehaviour
{
    public int Number;
    public int StartNumber;

    // Use this for initialization
    void Start()
    {
        var textElement = transform.GetChild(0).GetComponent<TextMesh>();
        textElement.text = Number.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
