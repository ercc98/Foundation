using UnityEngine;

namespace ErccDev.Foundation.Rendering
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SpriteRenderer))]
    public class BillboardSpriteRenderer : MonoBehaviour, IBillboardRenderer
    {
        [Header("Billboard")]
        [SerializeField] private BillboardMode _mode = BillboardMode.Spherical;

        [Header("Binding")]
        [SerializeField] private Camera _camera;

        public BillboardMode Mode        { get => _mode;   set => _mode   = value; }
        public Camera        TargetCamera { get => _camera; set => _camera = value; }

        void OnEnable()
        {
            if (!_camera)
                _camera = Camera.main;
        }

        void LateUpdate()
        {
            if (!_camera) return;

            if (_mode == BillboardMode.Spherical)
            {
                transform.rotation = _camera.transform.rotation;
            }
            else
            {
                Vector3 dir = _camera.transform.position - transform.position;
                dir.y = 0f;
                if (dir.sqrMagnitude < 1e-6f) return;
                transform.rotation = Quaternion.LookRotation(-dir.normalized, Vector3.up);
            }
        }
    }
}
