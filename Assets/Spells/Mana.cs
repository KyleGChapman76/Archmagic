using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Mana : MonoBehaviour
{
	public int maxMana;
	private int currentMana;
	public Health healthManager;

	public float manaGainPerSecond;
	public float manaExhaustTime;
	public float manaRegainStopTime;
	private float timeBetweenManaGain;
	private float regenTimer;

	public Text manaText;
	public Image manaPanel;

	public Color fullManaColor;
	public Color lowManaColor;
	public Color noManaColor;

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

	private void OnGUI()
	{
		if (manaText != null)
			manaText.text = currentMana.ToString();

		if (manaPanel != null)
		{
			float percentMana = .2f + .8f * ((currentMana) / (float)maxMana);
			percentMana = Mathf.Clamp(percentMana, 0f, 1f);

			Color panelColor;

			if (currentMana > 0)
			{
				panelColor = fullManaColor * percentMana + lowManaColor * (1 - percentMana);
			}
			else
			{
				panelColor = noManaColor;
			}

			manaPanel.color = panelColor;
		}
	}

	public void GainMana(int manaGained)
	{
		currentMana += manaGained;
		if (currentMana > maxMana)
			currentMana = maxMana;
	}

	public bool SpendMana (int manaSpent)
	{
		if (manaSpent < 0)
		{
			return false;
		}
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

	public void ResetMana()
	{
		currentMana = maxMana;
	}

	public bool RestoreMana(int manaRestore)
	{
		if (manaRestore < 0)
		{
			return false;
		}

		if (currentMana == maxMana)
		{
			return false;
		}

		currentMana += manaRestore;

		if (currentMana > maxMana)
			currentMana = maxMana;

		return true;
	}
}
