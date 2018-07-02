using UnityEngine;

public class BoardElementData : MonoBehaviour
{
    public bool ContainsTarget;

    public bool ContainsSnakeStep;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    void RemoveAllStepsMessage()
    {
        ContainsSnakeStep = false;
    }

    void RemoveTargetMessage()
    {
        ContainsTarget = false;
    }
}
