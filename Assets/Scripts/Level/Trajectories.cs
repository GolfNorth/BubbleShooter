using UnityEngine;

namespace BubbleShooter
{
    public sealed class Trajectories : MonoBehaviour
    {
        [SerializeField] private LineRenderer _lineRendererA;
        [SerializeField] private LineRenderer _lineRendererB;

        public void DrawTrajectory(BubbleTrajectory trajectory)
        {
            _lineRendererA.enabled = true;
            _lineRendererB.enabled = false;

            DrawTrajectory(trajectory, _lineRendererA);
        }

        public void DrawTrajectories(BubbleTrajectory trajectoryA, BubbleTrajectory trajectoryB)
        {
            _lineRendererA.enabled = true;
            _lineRendererB.enabled = true;

            DrawTrajectory(trajectoryA, _lineRendererA);
            DrawTrajectory(trajectoryB, _lineRendererB);
        }

        public void HideTrajectories()
        {
            _lineRendererA.enabled = false;
            _lineRendererB.enabled = false;
        }

        private void DrawTrajectory(BubbleTrajectory trajectory, LineRenderer lineRenderer)
        {
            var points = trajectory.Points;

            lineRenderer.positionCount = points.Count;

            var index = 0;

            foreach (var point in points)
            {
                lineRenderer.SetPosition(index, point.Value);

                index++;
            }
        }
    }
}