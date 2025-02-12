﻿using System.Collections.Generic;

namespace EVRC
{
    using ActionChange = ActionsController.ActionChange;
    using DirectionActionChange = ActionsController.DirectionActionChange;
    using OutputAction = ActionsController.OutputAction;
    using ActionChangeUnpressHandler = PressManager.UnpressHandlerDelegate<ActionsController.ActionChange>;
    using DirectionActionChangeUnpressHandler = PressManager.UnpressHandlerDelegate<ActionsController.DirectionActionChange>;
    using Direction = ActionsController.Direction;
    using HatDirection = vJoyInterface.HatDirection;

    /**
     * Outputs joystick buttons to vJoy when the associated throttle is grabbed
     */
    public class VirtualThrottleButtons : VirtualControlButtons
    {
        // Map of abstracted action presses to vJoy joystick button numbers
        private static Dictionary<OutputAction, uint> joyBtnMap = new Dictionary<OutputAction, uint>()
        {
            { OutputAction.ButtonPrimary, 8 },
            { OutputAction.ButtonSecondary, 7 },
            { OutputAction.ButtonAlt, 9 },
            { OutputAction.POV3, 6 },
        };

        // PARKER copied this over from the joystick controls to try to add directional thumbsticks to the throttle side
        private static Dictionary<OutputAction, uint> joyHatMap = new Dictionary<OutputAction, uint>()
        {
            { OutputAction.POV3, 3 }, //removed POV 2 for the throttle side
        };
        private static Dictionary<Direction, HatDirection> directionMap = new Dictionary<Direction, HatDirection>()
        {
            { Direction.Up, HatDirection.Up },
            { Direction.Right, HatDirection.Right },
            { Direction.Down, HatDirection.Down },
            { Direction.Left, HatDirection.Left },
        };

        private ActionsControllerPressManager actionsPressManager;

        override protected void OnEnable()
        {
            base.OnEnable();
            actionsPressManager = new ActionsControllerPressManager(this)
                .ButtonPrimary(OnAction)
                .ButtonSecondary(OnAction)
                .ButtonAlt(OnAction)
                .ButtonPOV3(OnAction)
                .DirectionPOV3(OnDirectionAction);
        }

        override protected void OnDisable()
        {
            base.OnDisable();
            actionsPressManager.Clear();
        }

        private PressManager.UnpressHandlerDelegate<ActionChange> OnAction(ActionChange pEv)
        {
            if (IsValidHand(pEv.hand) && joyBtnMap.ContainsKey(pEv.action))
            {
                uint btnIndex = joyBtnMap[pEv.action];
                PressButton(btnIndex);

                return (uEv) => { UnpressButton(btnIndex); };
            }

            return (uEv) => { };
        }

        private DirectionActionChangeUnpressHandler OnDirectionAction(DirectionActionChange pEv)
        {
            if (IsValidHand(pEv.hand) && joyHatMap.ContainsKey(pEv.action))
            {
                uint hatNumber = joyHatMap[pEv.action];
                SetHatDirection(hatNumber, directionMap[pEv.direction]);

                return (uEv) => { ReleaseHatDirection(hatNumber); };
            }

            return (uEv) => { };
        }
    }
}
