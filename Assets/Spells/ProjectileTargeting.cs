using UnityEngine;
using System.Collections;

public class ProjectileTargeting : MonoBehaviour
{
	public const SpellTargetingType targetType = SpellTargetingType.Projectile;

	private Vector3 direction;
	
	public void SetDirection (Vector3 direction)
	{
		this.direction = direction;
	}
	
	public Vector3 GetDirection ()
	{
		return direction;
	}
}
