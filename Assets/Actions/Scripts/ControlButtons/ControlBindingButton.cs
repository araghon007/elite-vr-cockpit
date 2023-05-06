﻿using EVRC.Core.Overlay;
using UnityEngine;

namespace EVRC.Core.Actions
{
    using static KeyboardInterface;

    /**
     * A component that specifies which EDControlButton to emit when activated
     */
    [RequireComponent(typeof(Tooltip))]
    public class ControlBindingButton : BaseButton
    {
        public ControlBindingsState controlBindingsState;

        [Tooltip("The button binding to press the associated keyboard key for")]
        public EDControlButton buttonBinding;

        protected override void OnEnable()
        {
            base.OnEnable();

            Refresh();
        }

        protected override void OnDisable()
        {
            base.OnDisable();
        }

        protected override Unpress Activate()
        {
            var unpress = CallbackPress(controlBindingsState.GetControlButton(buttonBinding, null));
            return () => unpress();
        }
    }
}
