public class Config{
	public const float FadeOutTime = 0.6f;
	public const float FadeInTime = 0.6f;

	public const int SingleTurnCount = 15;
	public const int SingleScoreTarget = 30;

	public const int SumModeTurnCount = 10;

    public const float WorldScale = 3.0f;

    /// <summary>
    /// 2017-08-27 ARKit Update 이후로 뭔가 변한듯.
    /// 그래서 이것보다 가까우면 카메라가 그리질 못한다.
    /// </summary>
    public const float MinimumRenderingDistance = 0.08f;
}