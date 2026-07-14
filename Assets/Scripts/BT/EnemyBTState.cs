using System;
using Unity.Behavior;

[BlackboardEnum]
public enum EnemyBTState
{
	Idle,
	Patrol,
	Chase,
	Attack,
	Dead
}
