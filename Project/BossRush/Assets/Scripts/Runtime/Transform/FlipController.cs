using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat.Assets
{
    public class FlipController : XBehaviour
    {
        [SuffixLabel("뒤집기를 사용하지 않음")]
        public bool IsBlockFlip;

        [SuffixLabel("무작위 뒤집기를 사용함")]
        public bool IsRandomFlip;

        public void BlockFlip()
        {
            IsBlockFlip = true;
        }

        public void SetFlip(bool facingRight)
        {
            if (IsBlockFlip)
            {
                return;
            }

            if (IsRandomFlip)
            {
                facingRight = RandomEx.GetBoolValue();
            }

            if (facingRight)
            {
                localScale = localScale.ApplyX(Mathf.Abs(localScale.x));
            }
            else
            {
                localScale = localScale.ApplyX(-Mathf.Abs(localScale.x));
            }
        }

        public void ResetFlip()
        {
            localScale = new Vector3(Mathf.Abs(localScale.x), localScale.y, 1);
        }
    }
}