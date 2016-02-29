using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Mana : MonoBehaviour
{
	public int maxMana;
	private int currentMana;
	public Text manaText;
	public Health healthManager;

	public float manaGainPerSecond;
	public float manaExhaustTime;
	public float manaRegainStopTime;
	private float timeBetweenManaGain;
	private float regenTimer;

	private void Start ()
	{
		currentMana = maxMana;
		timeBetweenManaGain = 1f / manaGainPerSecond;
    }

	private void Update ()
	{
		regenTimer += Time.deltaTime;
		if (regenTimer > timeBetweenManaGain)
		{
			regenTimer = 0;
			GainMana(1);
		}
		manaText.text = currentMana + "";
	}

	public void GainMana(int manaGained)
	{
		currentMana += manaGained;
		if (currentMana > maxMana)
			currentMana = maxMana;
	}

	public bool SpendMana (int manaSpent)
	{
		if (currentMana >= manaSpent)
		{
			currentMana -= manaSpent;
			if (regenTimer > -manaExhaustTime)
				regenTimer = -manaRegainStopTime;
            return true;
        }
		int healthSpend = manaSpent - currentMana;
		currentMana = 0;
		healthManager.Damage(healthSpend);
		regenTimer = -manaExhaustTime;
        return false;
    }
}
