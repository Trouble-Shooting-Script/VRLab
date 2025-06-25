using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Hands.Samples.GestureSample;

public class GestureCombination : MonoBehaviour
{
    public GestureDetector gesture1;
    public GestureDetector gesture2;
    private float gesture1EndTime;
    public float expireTime;
    public UnityEvent gestureCombinationPerformed;

    public GameObject TestObject;

    private void Start()
    {
        gesture1.gesturePerformed.AddListener(() => gesture1EndTime = Time.timeSinceLevelLoad);
        gesture2.gesturePerformed.AddListener(Perform);
    }

    private void Perform()
    {
        if(gesture1EndTime + expireTime > Time.timeSinceLevelLoad)
        {
            gestureCombinationPerformed?.Invoke();
            TestObject.SetActive(!TestObject.activeSelf);
        }
    }
}