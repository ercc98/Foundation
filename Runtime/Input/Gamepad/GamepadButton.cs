namespace ErccDev.Foundation.Input.Gamepad
{
    /// <summary>
    /// Digital buttons, named by physical position so the contract stays controller-agnostic.
    /// South/East/West/North map to A/B/X/Y on Xbox and Cross/Circle/Square/Triangle on PlayStation.
    /// </summary>
    public enum GamepadButton
    {
        South,
        East,
        West,
        North,
        LeftShoulder,
        RightShoulder,
    }
}
