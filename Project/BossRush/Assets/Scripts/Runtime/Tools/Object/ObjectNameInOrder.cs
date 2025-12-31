using UnityEngine;

namespace TeamSuneat
{
    public class ObjectNameInOrder : XBehaviour
    {
        public string objectName;
        public bool IsIndexFromZero;

        public override void AutoSetting()
        {
            if (string.IsNullOrEmpty(objectName))
            {
                return;
            }

            for (int i = 0; i < transform.childCount; i++)
            {
                Transform child = transform.GetChild(i);

                if (IsIndexFromZero)
                {
                    child.name = string.Format("{0} ({1})", objectName, i);
                }
                else
                {
                    child.name = string.Format("{0} ({1})", objectName, i + 1);
                }
            }
        }
    }
}