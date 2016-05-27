using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using System.Text;
using System;

public class Spawn : MonoBehaviour
{
    public List<GameObject> referenceMobs;

    public static List<Dictionary<GameObject, int>> mobsPerRound;

	private List<string> waves;

    // Use this for initialization
    void Start()
    {
		LoadMobsPerRound();
    }

	private void TempHardCodeLoading(Dictionary<GameObject, int> round)
	{
		waves = new List<string> (new string[] { /*"200", "210", "320", "531", "642", "754"*/"600" });
		foreach (string wave in waves) 
		{
			char[] characters = wave.ToCharArray();
			for(int i = 0 ; i < wave.Length ; ++i)
			{
				int val = (int)Char.GetNumericValue(characters[i]);
				if(val > 0)
				{
				   round.Add(referenceMobs[i], val);
				}
			}
			mobsPerRound.Add(round);
			round = new Dictionary<GameObject, int>();
		}
	}

	public void LoadMobsPerRound()
	{
		mobsPerRound = new List<Dictionary<GameObject, int>>();
		Dictionary<GameObject, int> round = new Dictionary<GameObject, int>();
		TempHardCodeLoading (round);
		/*try
		{
			string line;
			StreamReader theReader = new StreamReader(".\\Assets\\Documents\\Spawn.txt", Encoding.Default);
			using (theReader)
			{
				do
				{
					line = theReader.ReadLine();
					
					if (line != null)
					{
						if (line.Length == 0)
						{
							mobsPerRound.Add(round);
							round = new Dictionary<GameObject, int>();
						}
						else
						{
							string[] entries = line.Split(' ');
							round.Add(referenceMobs[System.Convert.ToInt32(entries[0])], System.Convert.ToInt32(entries[1]));
						}
					}
				}
				while (line != null); 
				theReader.Close();
			}
		}
		catch (System.Exception e)
		{
			print("Exception during load");
		}*/
	}

    public void SpawnMob(int nbRound)
    {
        Dictionary<GameObject, int> mobsToSpawn = mobsPerRound[nbRound - 1];

        // Select a random type of mob
        int randID = UnityEngine.Random.Range(0, mobsToSpawn.Count);

        List<GameObject> keyList = new List<GameObject>(mobsToSpawn.Keys);
        GameObject objRand = keyList[randID];
        Quaternion quater = Quaternion.identity;
        quater.y += 180;
        GameObject _mob = (GameObject)Instantiate(objRand, transform.position, quater);

        GameManager.mobsAlive.Add(_mob);
        mobsToSpawn[objRand]--;
        if (mobsToSpawn[objRand] == 0)
        {
            // when no more of this type of mob to instantiate, remove it
            mobsToSpawn.Remove(objRand);
        }
    }

    public static void notifyMobIsDestroyed(GameObject mobToDelete)
    {
        GameManager.mobsAlive.Remove(mobToDelete);
    }
}
