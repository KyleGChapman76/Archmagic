using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
	public int maxHealth;
	public bool  respawn;
	public bool  allowOverheal;
	public int maxOverhealedHealth;

	public Mana mana;
	
	private int health;
	
	private bool  alive;
	
	private static RespawnLocation[] locations;

	public Text healthText;
	public Image healthPanel;
	
	private void  Start ()
	{
		alive = true;
		health = maxHealth;
	}

	private void Update ()
	{
		if (Input.GetButtonDown("Damage"))
		{
			health -= 5;
		}

		if (health <= 0)
			alive = false;
		if (!alive)
		{
			if (respawn)
				Respawn();
			else
				Destroy(gameObject);
		}
	}

	private void OnGUI ()
	{
		if (healthText != null)
			healthText.text = health.ToString();

		if (healthPanel != null)
		{
			float redFactor = .3f + .9f * (health / (float) maxHealth);
            Color panelColor = new Color(1, redFactor, redFactor);
            healthPanel.color = panelColor;
		}
	}

	public int GetHealth ()
	{
		return health;
	}
	
	public bool Kill ()
	{
		if (!alive)
		{
			print(gameObject.name + " is already dead!");
			return false;
		}
		alive = false;
		return true;
	}
	
	public bool Revive ()
	{
		if (alive)
		{
			print(gameObject.name + " is already alive!");
			return false;
		}
		alive = true;
		return true;
	}
	
	public bool Damage (int amount)
	{
		if (amount < 0)
		{
			print("You are not allowed to heal with the damage function: " + gameObject.name);
			return false;
		}
		health -= amount;
		return true;
	}
	
	public bool Heal (int amount)
	{
		if (amount < 0)
		{
			print("You are not allowed to damage with the heal function: " + gameObject.name);
			return false;
		}
		
		health += amount;
		
		if (health > maxHealth && !allowOverheal)
			health = maxHealth;
			
		if (health > maxOverhealedHealth && allowOverheal)
			health = maxOverhealedHealth;
		
		return true;
	}
	
	public bool HealIgnoreOverheal ( int amount)
	{
		print("Asked to heal this while ignoring overheal.");
		if (amount < 0)
		{
			print("You are not allowed to damage with the heal function: " + gameObject.name);
			return false;
		}
		health += amount;
		if (health > maxOverhealedHealth)
			health = maxOverhealedHealth;
		return true;
	}
	
	public static RespawnLocation GetRespawnLocation()
	{
		GameObject[] array = GameObject.FindGameObjectsWithTag("Respawn");
		locations = new RespawnLocation[array.Length];
		for (int i = 0;i<array.Length;i++)
		{
			RespawnLocation location = array[i].GetComponent<RespawnLocation>();
			locations[i] = location;
		}
	
		if (locations.Length == 0)
		{
			print("No respawn locations found!");
			return null;
		}
		
		int index = Random.Range(0, locations.Length);
		return locations[index];
	}
	
	public void Respawn ()
	{
		RespawnLocation location = GetRespawnLocation();
		
		if (location == null)
			return;
		
		alive = true;
		health = maxHealth;
		transform.position = location.transform.position;
		transform.rotation = location.transform.rotation;

		mana.ResetMana();

		FPDPhysics physics = GetComponent<FPDPhysics>();
        if (physics)
		{
			physics.SetVelocity(Vector3.zero);
        }
	}
}