using UnityEngine;
using UnityEngine.InputSystem;
using TracingApp.Core;

namespace TracingApp.Input
{
    /// <summary>
    /// Handles controller input for triggering recording start/stop.
    /// Uses OVRInput for Meta Quest VR controllers (either hand trigger).
    /// Falls back to keyboard/mouse for desktop editor testing.
    /// </summary>
    public class ControllerInput : MonoBehaviour
    {
        [Header("References")]
        public TrialManager TrialManager;
        public LaserPointer LaserPointer;

        [Header("VR Trigger Settings")]
        [Tooltip("Which OVR button triggers recording toggle")]
        public OVRInput.Button VRTriggerButton = OVRInput.Button.PrimaryIndexTrigger;

        [Tooltip("Use raw button (no dead zone) for trigger detection")]
        public bool UseRawInput = false;

        [Header("Desktop Testing")]
        [Tooltip("Key to toggle recording in desktop mode")]
        public Key DesktopToggleKey = Key.Space;

        [Tooltip("Alternative: use mouse button")]
        public bool UseMouseButton = true;
        public int MouseButton = 0; // Left click

        [Header("Input System (Optional Fallback)")]
        [Tooltip("Optional Input Action for trigger (used if OVRInput unavailable)")]
        public InputActionReference TriggerAction;

        // Track previous frame state to detect press edge (down event)
        private bool _leftTriggerWasDown;
        private bool _rightTriggerWasDown;

        private void OnEnable()
        {
            if (TriggerAction != null && TriggerAction.action != null)
            {
                TriggerAction.action.Enable();
                TriggerAction.action.performed += OnTriggerPerformed;
            }
        }

        private void OnDisable()
        {
            if (TriggerAction != null && TriggerAction.action != null)
            {
                TriggerAction.action.performed -= OnTriggerPerformed;
            }
        }

        private void Update()
        {
            bool ovrTriggered = HandleOVRInput();

            // Always allow desktop fallback when OVR did not trigger this frame.
            // This is especially useful in simulator/editor where controllers may report
            // as connected but trigger events may not be routed through OVRInput.
            if (!ovrTriggered)
            {
                HandleDesktopInput();
            }
        }

        /// <summary>
        /// Handle Meta Quest controller input via OVRInput.
        /// Detects trigger press on EITHER hand controller.
        /// Returns true only when an OVR trigger press is detected this frame.
        /// </summary>
        private bool HandleOVRInput()
        {
            // Check if OVR is available by testing if any controller is connected
            if (!OVRInput.IsControllerConnected(OVRInput.Controller.LTouch) &&
                !OVRInput.IsControllerConnected(OVRInput.Controller.RTouch))
            {
                return false;
            }

            bool triggered = false;

            if (UseRawInput)
            {
                // Raw button detection (no dead zone processing)
                // Left controller trigger
                bool leftDown = OVRInput.GetDown(VRTriggerButton, OVRInput.Controller.LTouch);
                // Right controller trigger
                bool rightDown = OVRInput.GetDown(VRTriggerButton, OVRInput.Controller.RTouch);

                triggered = leftDown || rightDown;
            }
            else
            {
                // Standard button detection with OVR dead zones
                // Check either controller — ambidextrous design
                triggered = OVRInput.GetDown(VRTriggerButton, OVRInput.Controller.LTouch) ||
                            OVRInput.GetDown(VRTriggerButton, OVRInput.Controller.RTouch);
            }

            if (triggered)
            {
                ToggleRecording();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Handle VR trigger input via Unity Input System (fallback if OVRInput unavailable).
        /// </summary>
        private void OnTriggerPerformed(InputAction.CallbackContext context)
        {
            ToggleRecording();
        }

        /// <summary>
        /// Handle desktop keyboard/mouse input for editor testing.
        /// </summary>
        private void HandleDesktopInput()
        {
            // Keyboard toggle
            if (Keyboard.current != null && Keyboard.current[DesktopToggleKey].wasPressedThisFrame)
            {
                ToggleRecording();
                return;
            }

            // Mouse button toggle
            if (UseMouseButton && Mouse.current != null)
            {
                bool isPressed = MouseButton == 0
                    ? Mouse.current.leftButton.wasPressedThisFrame
                    : Mouse.current.rightButton.wasPressedThisFrame;

                if (isPressed)
                {
                    ToggleRecording();
                }
            }
        }

        /// <summary>
        /// Toggle recording state on the TrialManager and update laser visual.
        /// </summary>
        private void ToggleRecording()
        {
            if (TrialManager == null) return;

            TrialManager.ToggleRecording();

            // Update laser pointer state
            if (LaserPointer != null)
            {
                LaserPointer.SetLaserState(TrialManager.IsRecording);
            }
        }
    }
}
