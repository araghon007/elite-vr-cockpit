﻿using EVRC.Core.Overlay;
using UnityEngine;

namespace EVRC.Core.Actions
{
    /**
     * A virtual 1-axis movable throttle that outputs to vJoy when grabbed
     */
    public class VirtualThrottle : MonoBehaviour, IGrabable
    {
        [Range(0f, 1f)]
        public float magnitudeLength = 0.1f;
        [Range(0f, 100f)]
        public float deadzonePercentage = 0f;
        public Transform handle;
        public VirtualThrottleButtons buttons;
        public OverlayEditLockState editLockedState;
        private ControllerInteractionPoint attachedInteractionPoint;
        private Transform attachPoint;
        

        public GrabMode GetGrabMode()
        {
            return GrabMode.VirtualControl;
        }

        void Start()
        {
            Refresh();
        }

        void OnEnable()
        {
            // EDStateManager.eliteDangerousStarted.Listen(OnGameStart);

        }

        void OnDisable()
        {
            // EDStateManager.eliteDangerousStarted.Listen(OnGameStart);

            // Auto-release controls when they are hidden
            if (attachedInteractionPoint)
            {
                attachedInteractionPoint.ForceUngrab(this);
            }
        }

        public void OnGameStart()
        {
            if (attachedInteractionPoint == null)
            {
                // Reset the throttle on game start so we don't start with a full throttle
                // @todo Perhaps listening to game start / docking events would be better
                SetHandle(0);
            }
        }

        public bool Grabbed(ControllerInteractionPoint interactionPoint)
        {
            if (attachedInteractionPoint != null) return false;
            // Don't allow throttle use when editing is unlocked, so the movable surface can be used instead
            if (!editLockedState.EditLocked) return false;

            attachedInteractionPoint = interactionPoint;

            var attachPointObject = new GameObject("[AttachPoint]");
            attachPointObject.transform.SetParent(attachedInteractionPoint.transform);
            attachPointObject.transform.SetPositionAndRotation(handle.position, handle.rotation);
            attachPoint = attachPointObject.transform;

            if (buttons)
            {
                buttons.Grabbed(interactionPoint.Hand);
            }

            return true;
        }

        public void Ungrabbed(ControllerInteractionPoint interactionPoint)
        {
            if (interactionPoint == attachedInteractionPoint)
            {
                attachedInteractionPoint = null;
                Destroy(attachPoint.gameObject);
                attachPoint = null;

                if (buttons)
                {
                    buttons.Ungrabbed();
                }
            }
        }

        void OnValidate()
        {
            Refresh();
        }

        public void Refresh()
        {
            var collider = GetComponent<CapsuleCollider>();
            if (collider)
            {
                collider.height = magnitudeLength * 2 + 0.04f;
            }
        }
        
        void Update()
        {
            if (attachedInteractionPoint == null) return;

            var p = transform.InverseTransformPoint(attachPoint.position);
            float throttle = Mathf.Clamp(p.z, -magnitudeLength, magnitudeLength) / magnitudeLength;
            SetValue(throttle);
        }

        /**
         * Update handle position without updating throttle
         */
        public void SetHandle(float throttle)
        {
            if (handle)
            {
                handle.localPosition = new Vector3(0, 0, throttle * magnitudeLength);
            }
        }

        /**
         * Set the throttle magnitude
         */
        public void SetValue(float throttle)
        {
            // Apply deadzone
            throttle = Mathf.Abs(throttle) < (deadzonePercentage / 100f) ? 0f : throttle;

            // Update handle position
            SetHandle(throttle);

            // Change vJoy throttle
            vJoyInterface.instance.SetThrottle(throttle);
        }

    }
}
