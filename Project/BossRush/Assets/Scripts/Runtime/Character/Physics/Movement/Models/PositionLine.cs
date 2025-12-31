using Sirenix.OdinInspector;
using UnityEngine;

namespace TeamSuneat
{
	public class PositionLine : XBehaviour
	{
		public LineRenderer LineRenderer;

#if UNITY_EDITOR

		public override void AutoSetting()
		{
			base.AutoSetting();

			LineRenderer = GetComponent<LineRenderer>();
		}

		[Button("ShowLine")]
		public void ShowLine()
		{
			SetEnabledLineRenderer(true);
		}

		[Button("HideLine")]
		public void HideLine()
		{
			SetEnabledLineRenderer(false);
		}

#endif

		public void SetLinePositions(Vector3 next)
		{
			if (LineRenderer != null)
				LineRenderer.SetPositions(new Vector3[] { transform.position, next });
		}

		public void SetEnabledLineRenderer(bool enabled)
		{
			if (LineRenderer != null)
				LineRenderer.enabled = enabled;
		}
	}
}