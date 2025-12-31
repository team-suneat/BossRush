using TeamSuneat;
using UnityEngine;

public class RandomPosition : MonoBehaviour
{
    public float OffsetX;

    public float OffsetY;

    private Vector3 _randomPosition;

    private void OnEnable()
    {
        if (false == OffsetX.IsZero() && false == OffsetY.IsZero())
        {
            _randomPosition = new Vector3(RandomEx.Range(-OffsetX, OffsetX), RandomEx.Range(-OffsetY, OffsetY));
        }
        else if (OffsetY.IsZero())
        {
            _randomPosition = new Vector3(RandomEx.Range(-OffsetX, OffsetX), 0);
        }
        else if (OffsetX.IsZero())
        {
            _randomPosition = new Vector3(0, RandomEx.Range(-OffsetY, OffsetY));
        }
        else
        {
            return;
        }

        transform.localPosition = _randomPosition;
    }
}