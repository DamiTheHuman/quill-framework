/// <summary>
/// Duplicate of <see cref="SolidContactGimmick"/> but for special stages
/// </summary>
public class SpecialStageContactGimmick : SpecialStageContactEvent
{
    public override void Reset() => this.gameObject.layer = 19;
}
