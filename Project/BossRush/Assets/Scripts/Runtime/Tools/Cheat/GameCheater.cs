using UnityEngine;

namespace TeamSuneat
{
    public class GameCheater : MonoBehaviour
    {
        private bool UseInputCheat { get; set; }

        private void Awake()
        {
            if (GameDefine.IS_EDITOR_OR_DEVELOPMENT_BUILD)
            {
                UseInputCheat = true;
            }
            else
            {
                UseInputCheat = false;
                gameObject.SetActive(false);
            }
        }

        private void Update()
        {
            if (UseInputCheat)
            {
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    // Game Time Scale

                    if (Input.GetKeyDown(KeyCode.Alpha0))
                    {
                        GameTimeManager.Instance.SetFactor(0.1f);
                    }
                    if (Input.GetKeyDown(KeyCode.Alpha1))
                    {
                        GameTimeManager.Instance.SetFactor(1f);
                    }
                    if (Input.GetKeyDown(KeyCode.Alpha2))
                    {
                        GameTimeManager.Instance.SetFactor(2f);
                    }
                    if (Input.GetKeyDown(KeyCode.Alpha3))
                    {
                        GameTimeManager.Instance.SetFactor(3f);
                    }
                    if (Input.GetKeyDown(KeyCode.Alpha4))
                    {
                        GameTimeManager.Instance.SetFactor(4f);
                    }
                    if (Input.GetKeyDown(KeyCode.Alpha5))
                    {
                        GameTimeManager.Instance.SetFactor(5f);
                    }
                }
            }
        }
    }
}