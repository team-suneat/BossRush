using System.Collections.Generic;
using UnityEngine;

namespace TeamSuneat
{
    public class BaseLightningBolt
    {
        private List<LineRenderer> LineRenderers { get; set; } = new List<LineRenderer>();

        public LineRenderer LightRenderer { get; set; }

        public float SegmentLength { get; set; }

        public int Index { get; private set; }

        public bool IsActive { get; private set; }

        private Color _lightColor;

        public BaseLightningBolt(float segmentLength, int index)
        {
            SegmentLength = segmentLength;
            Index = index;
        }

        public void SpawnLineRenderer(GameObject prefab, int count, Transform parent)
        {
            LineRenderers.Clear();

            for (int i = 0; i < count; i++)
            {
                LineRenderer renderer = ResourcesManager.Instantiate<LineRenderer>(prefab, parent);
                if (renderer != null)
                {
                    renderer.enabled = false;

                    LineRenderers.Add(renderer);
                }
            }
        }

        public void SpawnLightRenderer(GameObject prefab, Transform parent)
        {
            LineRenderer renderer = ResourcesManager.Instantiate<LineRenderer>(prefab, parent);
            if (renderer != null)
            {
                _lightColor = renderer.startColor;

                LightRenderer = renderer;
            }
        }

        public void Init()
        {
            IsActive = false;
        }

        public void Activate()
        {
            if (LineRenderers != null)
            {
                for (int i = 0; i < LineRenderers.Count; i++)
                {
                    LineRenderers[i].enabled = true;
                }
            }

            if (LightRenderer != null)
            {
                LightRenderer.enabled = true;
            }

            IsActive = true;
        }

        public void Deactivate()
        {
            ClearLineRenderers();
            ResetLightRenderer();
        }

        private void ClearLineRenderers()
        {
            if (LineRenderers != null)
            {
                for (int i = 0; i < LineRenderers.Count; i++)
                {
                    if (LineRenderers[i] != null)
                    {
                        LineRenderers[i].enabled = false;
                        LineRenderers[i].positionCount = 0;

                        ResourcesManager.Despawn(LineRenderers[i].gameObject);
                    }
                }

                LineRenderers.Clear();
            }
        }

        private void ResetLightRenderer()
        {
            if (LightRenderer != null)
            {
                LightRenderer.enabled = false;
                LightRenderer.positionCount = 0;

                ResourcesManager.Despawn(LightRenderer.gameObject);

                LightRenderer = null;
            }
        }

        public void DrawLightning(Vector2 source, Vector2 target)
        {
            //Calculated amount of Segments
            float distance = Vector2.Distance(source, target);
            int segments;
            if (distance > SegmentLength)
            {
                segments = Mathf.FloorToInt(distance / SegmentLength) + 2;
            }
            else
            {
                segments = 4;
            }

            if (LineRenderers != null)
            {
                for (int i = 0; i < LineRenderers.Count; i++)
                {
                    // Set the amount of points to the calculated value
                    LineRenderers[i].positionCount = segments;
                    LineRenderers[i].SetPosition(0, source);

                    for (int j = 1; j < segments - 1; j++)
                    {
                        //Go linear from source to target
                        Vector2 tmp = Vector2.Lerp(source, target, j / (float)segments);

                        //Add randomness
                        Vector2 lastPosition = new(tmp.x + Random.Range(-0.1f, 0.1f), tmp.y + Random.Range(-0.1f, 0.1f));

                        //Set the calculated position
                        LineRenderers[i].SetPosition(j, lastPosition);
                    }
                    LineRenderers[i].SetPosition(segments - 1, target);
                }
            }

            if (LightRenderer != null)
            {
                LightRenderer.positionCount = 2;
                LightRenderer.SetPosition(0, source);
                LightRenderer.SetPosition(1, target);

                Color lightColor = new(_lightColor.r, _lightColor.g, _lightColor.b, Random.Range(0.2f, 1f));
                LightRenderer.startColor = lightColor;
                LightRenderer.endColor = lightColor;
            }
        }
    }
}