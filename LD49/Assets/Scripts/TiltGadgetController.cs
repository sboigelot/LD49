using UnityEngine;

public class TiltGadgetController : MonoBehaviour
{
    public GameObject TiltHandle;

    public float FullDegreeRotation = 360;

    public void UpdateTilt(float tilt)
    {
        if (TiltHandle == null)
        {
            return;
        }

        TiltHandle.transform.rotation = Quaternion.identity;
        TiltHandle.transform.Rotate(new Vector3(0f, 0f, -tilt));
    }
}
