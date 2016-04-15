using UnityEngine;

public class AstarSingleton{

	private static AstarSingleton instance = null; 
	private static readonly object myLock = new object();

	private Astar _astar; 
	public Astar astar { get { return _astar; } set { _astar = astar; } }

	private AstarSingleton ()
	{
		_astar = new Astar ();
	}

	public static AstarSingleton getInstance() 
	{ 
		lock (myLock) 
		{ 
			if (instance == null) {
				instance = new AstarSingleton ();
			}
			return instance; 
		} 
	}
}