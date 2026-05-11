using UnityEngine;

public class GroundType : MonoBehaviour
{
    public enum SurfaceType
    {
        Grass,
        Wood,
        Stone,
        Metal,
        Water,
        Sand
    }

    public SurfaceType surfaceType = SurfaceType.Grass;
}