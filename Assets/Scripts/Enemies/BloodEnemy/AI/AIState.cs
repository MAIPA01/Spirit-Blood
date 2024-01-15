public interface AIState
{
    public void OnEnter(BloodEnemyAIController sc);
    public void UpdateState(BloodEnemyAIController sc);
    public void OnExit(BloodEnemyAIController sc);
    public void OnHurt(BloodEnemyAIController sc);
}
