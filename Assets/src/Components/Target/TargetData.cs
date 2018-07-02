using UnityEngine;

public class TargetData : MonoBehaviour
{
    public int Value;

    // Use this for initialization
    void Start()
    {
        var textElement = transform.GetChild(0).GetComponent<TextMesh>();
        textElement.text = Value.ToString();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void RemoveTargetMessage()
    {
        gameObject.SetActive(false);
    }
}
