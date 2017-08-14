using AcrylecSkeleton.MVC;

namespace Assets.Objects.Enemy
{
	/// <summary>
	/// Base state enum for Enemy MVC object.
	/// </summary>
	public enum EnemyState
    {
        Idle, //When the enemy doesn't do anything.
        Patrol, //When the enemy is patrolling.
        Chase //When the enemy is chasing something.
    }

	/// <summary>
	/// Application class for Enemy MVC object.
	/// Created by: MP-L
	/// Data: Monday, August 14, 2017
	/// </summary>
	public class EnemyApplication : Application<EnemyModel, EnemyView, EnemyController, EnemyState>
	{
	}
}