using UnityEngine;

public static class TransformExtensions
{
    public static void Reset(this Transform transform)
    {
        if (transform == null)
        {
            return;
        }

        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    public static void ResetExceptPosition(this Transform transform, Vector3 oldPosition)
    {
        if (transform == null)
        {
            return;
        }

        transform.localPosition = oldPosition;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }
}
