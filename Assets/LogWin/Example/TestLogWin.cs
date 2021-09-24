using UnityEngine;
namespace Tests
{
	//This is a small test script for Logwin.Log()
	//It simulates the behaviour of a player being hit by an ennemy

	public class TestLogWin : MonoBehaviour
	{
		int mHealthMax = 100;
		int mHealthCurrent;

		Vector2Int mEnnemyAtkDamagesMinMax = new Vector2Int(10, 20);
		Vector2 mEnnemyAtkDelay = new Vector2(.5f, 1f);
		float mRemainingTimeBeforeHit;

		private void Start()
		{
			mHealthCurrent = mHealthMax;
			mRemainingTimeBeforeHit = Random.Range(mEnnemyAtkDelay.x, mEnnemyAtkDelay.y);
		}

		private void Update()
		{
			//ennemy behaviour
			if (mHealthCurrent > 0)
			{
				mRemainingTimeBeforeHit -= Time.deltaTime;
				if (mRemainingTimeBeforeHit <= 0)
					EnnemyHit(Random.Range(mEnnemyAtkDamagesMinMax.x, mEnnemyAtkDamagesMinMax.y));
			}

			//The LogwinParam.Color() parameter will assign a color to the log. Here, we want to display green if the health is >= 25 hp, and red otherwise
			Logwin.Log("Health", mHealthCurrent + "/" + mHealthMax, "PlayerData",
				mHealthCurrent >= 25 ? LogwinParam.Color(Color.green) : LogwinParam.Color(Color.red));

			//The LogwinParam.Pause(mHealthCurrent <= 0) parameter will pause the game when the health will be <= 0
			Logwin.Log("State", mHealthCurrent > 0 ? "Alive" : "Dead", "PlayerData", LogwinParam.Pause(mHealthCurrent <= 0));
		}

		void EnnemyHit(int damage)
		{
			mHealthCurrent -= damage;
			if (mHealthCurrent < 0)
				mHealthCurrent = 0;
			mRemainingTimeBeforeHit = Random.Range(mEnnemyAtkDelay.x, mEnnemyAtkDelay.y);

			//The character has been hit! Here we display the amount of damage he took
			Logwin.Log("Damage received", damage, "PlayerData");
		}
	}
}

