using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TracingApp.Tracing;

namespace TracingApp.Input
{
    /// <summary>
    /// Gaze-based laser pointer that originates from the camera (headset center).
    /// In VR: ray fires from CenterEyeAnchor forward (head/gaze direction).
    /// In Editor: ray fires from mouse position through camera.
    /// </summary>
    public class LaserPointer : MonoBehaviour
    {
        [Header("Laser Visual")]
        public LineRenderer LaserLine;
        public float LaserMaxDistance = 10f;
        public float BeamStartOffset = 0.03f;

        [Header("Beam Fallback")]
        [Tooltip("Render a mesh beam between headset and hit point as a fallback when LineRenderer is not visible in XR")]
        public bool UseMeshBeamFallback = true;
        public float MeshBeamRadius = 0.006f;

        [Header("Colors")]
        public Color IdleColor = new Color(1f, 0f, 0f, 0.3f);
        public Color ActiveColor = new Color(1f, 0f, 0f, 1f);

        [Header("Laser Dot")]
        [Tooltip("Optional dot/reticle at the laser hit point")]
        public Transform LaserDot;

        [Header("References")]
        [Tooltip("The CenterEyeAnchor camera from OVRCameraRig")]
        public Camera GazeCamera;
        public TracingRecorder TracingRecorder;

        [Header("Settings")]
        [Tooltip("Layer mask for the pattern canvas")]
        public LayerMask PatternLayerMask = ~0;

        [Tooltip("Use mouse for desktop testing when no VR headset is connected")]
        public bool UseMouseFallback = true;

        // State
        public bool IsActive { get; private set; }
        public Vector2 CurrentUV { get; private set; }
        public bool IsHittingPattern { get; private set; }

        private Vector2 _lastUV;
        private bool _hadLastUV;
        private bool _vrActive;
        private Transform _meshBeam;
        private MeshRenderer _meshBeamRenderer;

        private void Start()
        {
            if (LaserLine == null)
                LaserLine = GetComponent<LineRenderer>();

            if (LaserLine == null)
                LaserLine = gameObject.AddComponent<LineRenderer>();

            if (GazeCamera == null)
                GazeCamera = Camera.main;

            EnsureLaserMaterial();
            EnsureLaserLineState();
            EnsureMeshBeam();

            SetLaserState(false);
        }

        private void Update()
        {
            if (GazeCamera == null)
            {
                GazeCamera = Camera.main;

                if (GazeCamera == null)
                {
                    var centerEye = GameObject.Find("CenterEyeAnchor");
                    if (centerEye != null)
                        GazeCamera = centerEye.GetComponent<Camera>();
                }
            }

            // Cache VR state once per frame
            _vrActive = OVRManager.isHmdPresent;

            EnsureLaserMaterial();
            EnsureLaserLineState();
            EnsureMeshBeam();

            PerformRaycast();
            UpdateLaserVisual();
        }

        private void EnsureLaserMaterial()
        {
            if (LaserLine == null) return;

            Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null)
                shader = Shader.Find("Sprites/Default");

            if (shader == null) return;

            var current = LaserLine.sharedMaterial;
            bool needsReplacement = current == null ||
                current.shader == null ||
                current.shader.name == "Hidden/InternalErrorShader" ||
                current.shader.name.Contains("Particles/Standard", System.StringComparison.OrdinalIgnoreCase);

            if (needsReplacement)
            {
                var material = new Material(shader);
                material.color = IdleColor;
                if (material.HasProperty("_BaseColor"))
                    material.SetColor("_BaseColor", IdleColor);
                if (material.HasProperty("_Color"))
                    material.SetColor("_Color", IdleColor);
                LaserLine.material = material;
            }
        }

        private void EnsureLaserLineState()
        {
            if (LaserLine == null) return;

            if (!LaserLine.enabled)
                LaserLine.enabled = true;

            if (LaserLine.positionCount != 2)
                LaserLine.positionCount = 2;

            LaserLine.useWorldSpace = true;
            LaserLine.alignment = LineAlignment.View;
            LaserLine.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            LaserLine.receiveShadows = false;
            LaserLine.sortingOrder = 5000;

            if (LaserLine.startWidth <= 0.0001f)
                LaserLine.startWidth = 0.01f;
            if (LaserLine.endWidth <= 0.0001f)
                LaserLine.endWidth = 0.005f;

            if (LaserLine.startWidth < 0.003f)
                LaserLine.startWidth = 0.012f;
            if (LaserLine.endWidth < 0.002f)
                LaserLine.endWidth = 0.006f;
        }

        private void EnsureMeshBeam()
        {
            if (!UseMeshBeamFallback)
            {
                if (_meshBeam != null)
                    _meshBeam.gameObject.SetActive(false);
                return;
            }

            if (_meshBeam != null) return;

            var beamGO = GameObject.CreatePrimitive(PrimitiveType.Cube);
            beamGO.name = "LaserBeamMesh";
            beamGO.transform.SetParent(transform, false);
            beamGO.layer = gameObject.layer;

            var collider = beamGO.GetComponent<Collider>();
            if (collider != null)
                Destroy(collider);

            _meshBeamRenderer = beamGO.GetComponent<MeshRenderer>();

            Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
            if (shader == null)
                shader = Shader.Find("Sprites/Default");

            if (shader != null)
            {
                var mat = new Material(shader);
                mat.color = IdleColor;
                if (mat.HasProperty("_BaseColor"))
                    mat.SetColor("_BaseColor", IdleColor);
                if (mat.HasProperty("_Color"))
                    mat.SetColor("_Color", IdleColor);
                _meshBeamRenderer.sharedMaterial = mat;
            }

            _meshBeamRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            _meshBeamRenderer.receiveShadows = false;

            _meshBeam = beamGO.transform;
        }

        /// <summary>
        /// Set the laser to active (tracing) or idle state.
        /// </summary>
        public void SetLaserState(bool active)
        {
            IsActive = active;

            if (!active)
            {
                _hadLastUV = false;
            }
        }

        /// <summary>
        /// Perform the gaze raycast.
        /// In VR: ray from CenterEyeAnchor camera forward (gaze/head direction).
        /// In Editor: ray from mouse screen position through camera.
        /// </summary>
        private void PerformRaycast()
        {
            if (GazeCamera == null) return;

            Ray ray;

            if (_vrActive)
            {
                // VR mode: use camera forward (gaze direction from CenterEyeAnchor)
                ray = new Ray(GazeCamera.transform.position, GazeCamera.transform.forward);
            }
            else if (UseMouseFallback)
            {
                // Desktop mode: use mouse position
                Vector2 mousePos = Mouse.current != null
                    ? Mouse.current.position.ReadValue()
                    : new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
                ray = GazeCamera.ScreenPointToRay(mousePos);
            }
            else
            {
                // No VR, no mouse fallback — just use camera forward
                ray = new Ray(GazeCamera.transform.position, GazeCamera.transform.forward);
            }

            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, LaserMaxDistance, PatternLayerMask))
            {
                IsHittingPattern = true;

                // Get UV coordinates from the hit
                CurrentUV = ResolveHitUV(hit);

                // Update laser line endpoint
                if (LaserLine != null)
                {
                    Vector3 beamStart = GetBeamStart(ray);
                    LaserLine.SetPosition(0, beamStart);
                    LaserLine.SetPosition(1, hit.point);
                    UpdateMeshBeam(beamStart, hit.point);
                }

                // Update dot position
                if (LaserDot != null)
                {
                    LaserDot.position = hit.point + hit.normal * 0.001f;
                    LaserDot.gameObject.SetActive(true);
                }

                // Paint on trace texture if active
                if (IsActive && TracingRecorder != null && TracingRecorder.IsRecording)
                {
                    if (_hadLastUV)
                    {
                        TracingRecorder.PaintLineUV(_lastUV, CurrentUV);
                    }
                    else
                    {
                        TracingRecorder.PaintAtUV(CurrentUV);
                    }
                    _lastUV = CurrentUV;
                    _hadLastUV = true;
                }
            }
            else
            {
                IsHittingPattern = false;

                // Extend laser to max distance
                if (LaserLine != null)
                {
                    Vector3 beamStart = GetBeamStart(ray);
                    Vector3 beamEnd = beamStart + ray.direction * LaserMaxDistance;
                    LaserLine.SetPosition(0, beamStart);
                    LaserLine.SetPosition(1, beamEnd);
                    UpdateMeshBeam(beamStart, beamEnd);
                }

                if (LaserDot != null)
                {
                    LaserDot.gameObject.SetActive(false);
                }

                _hadLastUV = false;
            }
        }

        private Vector2 ResolveHitUV(RaycastHit hit)
        {
            if (hit.collider is MeshCollider)
            {
                return hit.textureCoord;
            }

            var rectTransform = hit.collider != null ? hit.collider.GetComponent<RectTransform>() : null;
            if (rectTransform != null)
            {
                Vector3 localPoint = rectTransform.InverseTransformPoint(hit.point);
                Rect rect = rectTransform.rect;

                if (rect.width > 0.0001f && rect.height > 0.0001f)
                {
                    float u = (localPoint.x / rect.width) + 0.5f;
                    float v = (localPoint.y / rect.height) + 0.5f;
                    return new Vector2(Mathf.Clamp01(u), Mathf.Clamp01(v));
                }
            }

            return hit.textureCoord;
        }

        private Vector3 GetBeamStart(Ray ray)
        {
            float nearClip = GazeCamera != null ? GazeCamera.nearClipPlane : 0.01f;
            float offset = Mathf.Max(BeamStartOffset, nearClip + 0.005f);
            return ray.origin + ray.direction * offset;
        }

        private void UpdateMeshBeam(Vector3 start, Vector3 end)
        {
            if (!UseMeshBeamFallback || _meshBeam == null) return;

            Vector3 direction = end - start;
            float length = direction.magnitude;
            if (length <= 0.0001f)
            {
                _meshBeam.gameObject.SetActive(false);
                return;
            }

            _meshBeam.gameObject.SetActive(true);
            _meshBeam.position = (start + end) * 0.5f;
            _meshBeam.rotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
            _meshBeam.localScale = new Vector3(MeshBeamRadius, MeshBeamRadius, length);
        }

        /// <summary>
        /// Update the laser visual based on active state.
        /// </summary>
        private void UpdateLaserVisual()
        {
            if (LaserLine == null) return;

            Color color = IsActive ? ActiveColor : IdleColor;
            LaserLine.startColor = color;
            LaserLine.endColor = color;

            if (LaserLine.material != null)
            {
                LaserLine.material.color = color;
                if (LaserLine.material.HasProperty("_BaseColor"))
                    LaserLine.material.SetColor("_BaseColor", color);
                if (LaserLine.material.HasProperty("_Color"))
                    LaserLine.material.SetColor("_Color", color);
            }

            if (_meshBeamRenderer != null && _meshBeamRenderer.sharedMaterial != null)
            {
                _meshBeamRenderer.sharedMaterial.color = color;
                if (_meshBeamRenderer.sharedMaterial.HasProperty("_BaseColor"))
                    _meshBeamRenderer.sharedMaterial.SetColor("_BaseColor", color);
                if (_meshBeamRenderer.sharedMaterial.HasProperty("_Color"))
                    _meshBeamRenderer.sharedMaterial.SetColor("_Color", color);
            }
        }
    }
}
