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

	private int currentHealth;
	
	private bool  alive;
	
	private static RespawnLocation[] locations;

	public Text healthText;
	public Image healthPanel;
	public Color fullHealthColor;
	public Color lowHealthColor;
	public Color noHealthColor;

	private void  Start ()
	{
		alive = true;
		currentHealth = maxHealth;
	}

	private void Update ()
	{
		if (Input.GetButtonDown("Damage"))
		{
			currentHealth -= 5;
		}

		if (currentHealth <= 0)
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
			healthText.text = currentHealth.ToString();

		if (healthPanel != null)
		{
			float percentHealth = .2f + .8f * ((currentHealth) / (float)maxHealth);
			percentHealth = Mathf.Clamp(percentHealth, 0f, 1f);
			Color panelColor;

			if (currentHealth > 0)
			{
				panelColor = fullHealthColor * percentHealth + lowHealthColor * (1 - percentHealth);
			}
			else
			{
				panelColor = noHealthColor;
            }

			healthPanel.color = panelColor;
		}
	}

	public int GetHealth ()
	{
		return currentHealth;
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
		currentHealth -= amount;
		return true;
	}
	
	public bool Heal (int amount)
	{
		if (amount < 0)
		{
			print("You are not allowed to damage with the heal function: " + gameObject.name);
			return false;
		}
		
		currentHealth += amount;
		
		if (currentHealth > maxHealth && !allowOverheal)
			currentHealth = maxHealth;
			
		if (currentHealth > maxOverhealedHealth && allowOverheal)
			currentHealth = maxOverhealedHealth;
		
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
		currentHealth += amount;
		if (currentHealth > maxOverhealedHealth)
			currentHealth = maxOverhealedHealth;
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
		currentHealth = maxHealth;
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