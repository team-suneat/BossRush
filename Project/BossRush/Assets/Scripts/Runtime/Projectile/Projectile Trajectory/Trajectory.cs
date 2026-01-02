using TeamSuneat;

using UnityEngine;

public class Trajectory : MonoBehaviour
{
	[SerializeField] protected LineRenderer m_lineRenderer;

	[SerializeField] protected SpriteRenderer m_spriteRenderer;

	[SerializeField] protected LayerMask m_collisionLayerMask;

	[Range(20, 40)]
	[SerializeField] protected int m_numberOfDots = 20;

	[SerializeField] protected float m_separation = 3f;

	[SerializeField] protected float m_shift = 3f;

	[SerializeField] protected Transform m_parent;

	[SerializeField] protected Vector2 m_shotForce;

	protected int m_positionCount;

	protected float m_x1;

	protected float m_y1;

	public void Setup(Transform parent, float forceX, float forceY)
	{
		m_parent = parent;
		m_shotForce = new Vector2(forceX, forceY);

		if (m_lineRenderer.enabled)
		{
			m_lineRenderer.positionCount = m_numberOfDots;
			m_lineRenderer.SetPositions(null);
		}

		for (int k = 0; k < m_numberOfDots; k++)
		{
			m_positionCount++;

			m_x1 = m_parent.position.x + m_shotForce.x * Time.fixedDeltaTime * (m_separation * k + m_shift);
			m_y1 = m_parent.position.y + m_shotForce.y * Time.fixedDeltaTime * (m_separation * k + m_shift);
			m_y1 -= (-Physics2D.gravity.y / 2f * Time.fixedDeltaTime * Time.fixedDeltaTime * (m_separation * k + m_shift) * (m_separation * k + m_shift));

			if (m_lineRenderer.enabled)
				m_lineRenderer.SetPosition(k, new Vector3(m_x1, m_y1));

			RaycastHit2D hit = Physics2D.Raycast(new Vector2(m_x1, m_y1), Vector3.zero, 0.1f, m_collisionLayerMask);
			if (hit.collider != null)
			{
				if (false == hit.collider.CompareTag(GameTags.Through))
				{
					m_positionCount--;
					continue;
				}

				if (m_lineRenderer.enabled)
					m_lineRenderer.positionCount = m_positionCount;

				if (m_spriteRenderer.enabled)
				{
					m_spriteRenderer.transform.position = new Vector3(m_x1, m_y1);
				}
				break;
			}
		}
	}

	public void SetEnabled(bool useLine, bool useArea)
	{
		m_lineRenderer.enabled = useLine;
		m_spriteRenderer.enabled = useArea;
	}
}