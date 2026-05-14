public readonly struct PanicViewState
{
    public PanicViewState(
        float panicRatio,
        float profileFillAmount,
        float vignetteAlpha,
        bool isControlLost)
    {
        PanicRatio = panicRatio;
        ProfileFillAmount = profileFillAmount;
        VignetteAlpha = vignetteAlpha;
        IsControlLost = isControlLost;
    }

    public float PanicRatio { get; }
    public float ProfileFillAmount { get; }
    public float VignetteAlpha { get; }
    public bool IsControlLost { get; }
}