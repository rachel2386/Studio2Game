using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using InControl;

public class DoubleRachelPlayerActions : PlayerActionSet
{
    public PlayerAction Fire;
    public PlayerAction Jump;
    public PlayerAction MoveLeft;
    public PlayerAction MoveRight;
    public PlayerAction MoveUp;
    public PlayerAction MoveDown;

    public PlayerAction LookLeft;
    public PlayerAction LookRight;
    public PlayerAction LookUp;
    public PlayerAction LookDown;


    public PlayerTwoAxisAction Move;
    public PlayerTwoAxisAction Look;


    public DoubleRachelPlayerActions()
    {
        Fire = CreatePlayerAction("Fire");
        Jump = CreatePlayerAction("Jump");


        MoveLeft = CreatePlayerAction("Move Left");
        MoveRight = CreatePlayerAction("Move Right");
        MoveUp = CreatePlayerAction("Move Up");
        MoveDown = CreatePlayerAction("Move Down");

        LookLeft = CreatePlayerAction("Look Left");
        LookRight = CreatePlayerAction("Look Right");
        LookUp = CreatePlayerAction("Look Up");
        LookDown = CreatePlayerAction("Look Down");

        Move = CreateTwoAxisPlayerAction(MoveLeft, MoveRight, MoveDown, MoveUp);
        Look = CreateTwoAxisPlayerAction(LookLeft, LookRight, LookDown, LookUp);
    }


    public static DoubleRachelPlayerActions CreateWithDefaultBindings()
    {
        var playerActions = new DoubleRachelPlayerActions();

        // How to set up mutually exclusive keyboard bindings with a modifier key.
        // playerActions.Back.AddDefaultBinding( Key.Shift, Key.Tab );
        // playerActions.Next.AddDefaultBinding( KeyCombo.With( Key.Tab ).AndNot( Key.Shift ) );

        // playerActions.Fire.AddDefaultBinding(Key.A);
        playerActions.Fire.AddDefaultBinding(InputControlType.Action1);
        playerActions.Fire.AddDefaultBinding(Mouse.LeftButton);

        playerActions.Jump.AddDefaultBinding(Key.Space);
        playerActions.Jump.AddDefaultBinding(InputControlType.Action3);
        playerActions.Jump.AddDefaultBinding(InputControlType.Back);

        //playerActions.MoveUp.AddDefaultBinding(Key.UpArrow);
        //playerActions.MoveDown.AddDefaultBinding(Key.DownArrow);
        //playerActions.MoveLeft.AddDefaultBinding(Key.LeftArrow);
        //playerActions.MoveRight.AddDefaultBinding(Key.RightArrow);

        playerActions.MoveUp.AddDefaultBinding(Key.W);
        playerActions.MoveDown.AddDefaultBinding(Key.S);
        playerActions.MoveLeft.AddDefaultBinding(Key.A);
        playerActions.MoveRight.AddDefaultBinding(Key.D);

        playerActions.MoveLeft.AddDefaultBinding(InputControlType.LeftStickLeft);
        playerActions.MoveRight.AddDefaultBinding(InputControlType.LeftStickRight);
        playerActions.MoveUp.AddDefaultBinding(InputControlType.LeftStickUp);
        playerActions.MoveDown.AddDefaultBinding(InputControlType.LeftStickDown);

        playerActions.MoveLeft.AddDefaultBinding(InputControlType.DPadLeft);
        playerActions.MoveRight.AddDefaultBinding(InputControlType.DPadRight);
        playerActions.MoveUp.AddDefaultBinding(InputControlType.DPadUp);
        playerActions.MoveDown.AddDefaultBinding(InputControlType.DPadDown);



        playerActions.LookUp.AddDefaultBinding(Mouse.PositiveY);
        playerActions.LookDown.AddDefaultBinding(Mouse.NegativeY);
        playerActions.LookLeft.AddDefaultBinding(Mouse.NegativeX);
        playerActions.LookRight.AddDefaultBinding(Mouse.PositiveX);


        playerActions.LookUp.AddDefaultBinding(InputControlType.RightStickUp);
        playerActions.LookDown.AddDefaultBinding(InputControlType.RightStickDown);
        playerActions.LookLeft.AddDefaultBinding(InputControlType.RightStickLeft);
        playerActions.LookRight.AddDefaultBinding(InputControlType.RightStickRight);

        playerActions.ListenOptions.IncludeUnknownControllers = true;
        playerActions.ListenOptions.MaxAllowedBindings = 4;
        //playerActions.ListenOptions.MaxAllowedBindingsPerType = 1;
        //playerActions.ListenOptions.AllowDuplicateBindingsPerSet = true;
        playerActions.ListenOptions.UnsetDuplicateBindingsOnSet = true;
        //playerActions.ListenOptions.IncludeMouseButtons = true;
        //playerActions.ListenOptions.IncludeModifiersAsFirstClassKeys = true;
        //playerActions.ListenOptions.IncludeMouseButtons = true;
        //playerActions.ListenOptions.IncludeMouseScrollWheel = true;

        playerActions.ListenOptions.OnBindingFound = (action, binding) =>
        {
            if (binding == new KeyBindingSource(Key.Escape))
            {
                action.StopListeningForBinding();
                return false;
            }
            return true;
        };

        playerActions.ListenOptions.OnBindingAdded += (action, binding) =>
        {
            Debug.Log("Binding added... " + binding.DeviceName + ": " + binding.Name);
        };

        playerActions.ListenOptions.OnBindingRejected += (action, binding, reason) =>
        {
            Debug.Log("Binding rejected... " + reason);
        };

        return playerActions;
    }
}