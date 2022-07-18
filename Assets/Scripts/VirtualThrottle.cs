using System;
using UnityEngine;
using Valve.VR;

namespace EVRC
{
    /**
     * A virtual 1-axis movable throttle that outputs to vJoy when grabbed
     */
    using Events = SteamVR_Events;

    public class VirtualThrottle : MonoBehaviour, IGrabable, IHighlightable
    {

        public Color forwardColor;
        public Color highlightColor;
        public Color reverseColor;
        public Color deadzoneColor;
        [Range(0f, 1f)]
        public float magnitudeLength = 1f;
        [Range(0f, 100f)]
        public float deadzonePercentage = 0f;
        public bool useColorChange = true; 
        public bool useHapticDetent = true; 
        public Transform handle;
        public HolographicRect line;
        public VirtualThrottleButtons buttons;
        protected CockpitStateController controller;
        private bool highlighted = false;
        private ControllerInteractionPoint attachedInteractionPoint;
        private Transform attachPoint;

        public GrabMode GetGrabMode()
        {
            return GrabMode.VirtualControl;
        }

        public class Throttle{
            private float _power;

            // -1 = reverse, +1 = forward
            public int Direction { get; set; }

            // Percent throttle (power)
            public float Power
            {
                get { return _power; }
                set {
                    _power = value;

                    if (_power == 0) {
                        Direction = 0;
                        this?.OnDirectionChanged(this, Direction);
                    }
                    else if (Math.Sign(_power) != Direction)
                    {
                        Direction = Math.Sign(_power);
                        this?.OnDirectionChanged(this, Direction);
                    }
                    
                }
            } 
           
            public void OnDirectionChanged(Throttle t, int n) {
                SteamVR_Actions.default_Haptic[SteamVR_Input_Sources.LeftHand].Execute(0, 0.1f, 250, 0.8f);
                Debug.LogFormat("-----------------dir change: {0} -----------------", n);
            }
        }

        public Throttle throttle = new Throttle();      

        void Start()
        {
            controller = CockpitStateController.instance; 
            Refresh();
        }

        void OnEnable()
        {
            EDStateManager.EliteDangerousStarted.Listen(OnGameStart);
        }

        void OnDisable()
        {
            EDStateManager.EliteDangerousStarted.Listen(OnGameStart);

            // Auto-release controls when they are hidden
            if (attachedInteractionPoint)
            {
                attachedInteractionPoint.ForceUngrab(this);
            }
        }

        void OnGameStart()
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
            if (!controller.editLocked) return false;

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

        public void OnHover()
        {
            highlighted = true;
            Refresh();
        }

        public void OnUnhover()
        {
            highlighted = false;
            Refresh();
        }

        public void Refresh()
        {
            if (line)
            {
                line.width = magnitudeLength * 2;
                line.pxWidth = 10;
                if (highlighted)
                {
                    line.color = highlightColor;
                }
                else
                {
                    line.color = forwardColor;
                }
            }

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
            throttle.Power = Mathf.Clamp(p.z, -magnitudeLength, magnitudeLength) / magnitudeLength;
            SetValue(throttle.Power);
        }

        /**
         * Update handle position without updating throttle
         */
        public void SetHandle(float power)
        {
            if (handle)
            {
                handle.localPosition = new Vector3(0, 0, power * magnitudeLength);
            }
        }

        /**
         * Set the throttle magnitude
         */
        public void SetValue(float power)
        {
            // Apply deadzone
            power = Mathf.Abs(power) < (deadzonePercentage / 100f) ? 0f : power;

            if (useColorChange){HandleThrottleColors(power);}

            // Update handle position
            SetHandle(power);

            // Change vJoy 
            vJoyInterface.instance.SetThrottle(power);
        }


        public void HandleThrottleColors(float power)
        {
            // Set color to indicate deadzone
            if (power == 0) 
            {
                line.color = deadzoneColor;
            }
            // Set color for forward thrust
            if (throttle.Direction == 1) 
            {
                line.color = forwardColor;
            } 

            // Set color for reverse thrust
            else if (throttle.Direction == -1) 
            {
                line.color = reverseColor;
            }
        }

    }
}
