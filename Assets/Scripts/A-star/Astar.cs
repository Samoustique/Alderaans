using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SettlersEngine;

public class MyPathNode : IPathNode<Object>{
	public int X { get; set; }
	public int Y { get; set; }
	public bool IsTower {get; set;}

	public bool IsWalkable(Object unused)
	{
		return !IsTower;
	}
}

public class MySolver<TPathNode, TUserContext> : SettlersEngine.SpatialAStar<TPathNode, 
TUserContext> where TPathNode : SettlersEngine.IPathNode<TUserContext>
{
	protected override double Heuristic(PathNode inStart, PathNode inEnd)
	{
		return Mathf.Abs(inStart.X - inEnd.X) + Mathf.Abs(inStart.Y - inEnd.Y);
	}

	protected override double NeighborDistance(PathNode inStart, PathNode inEnd)
	{
		return Heuristic(inStart, inEnd);
	}

	public MySolver(TPathNode[,] inGrid)
		: base(inGrid)
	{
	}
} 


public class Astar {


	private MyPathNode[,] grid;
	private int width;
	private int height;
	private Point departurePoint;
	private Point arrivalPoint;
	private SpatialAStar<MyPathNode, Object> aStar;

	public Astar(int w, int h, int departureX, int departureY, int arrivalX, int arrivalY)
	{
		width = w;
		height = h;
		FillGrid ();
		departurePoint = new Point(departureX, departureY);
		arrivalPoint = new Point(arrivalX, arrivalY);

		aStar = new SpatialAStar<MyPathNode, Object>(grid); 
	}

	private void FillGrid ()
	{
		// setup grid with walls
		grid = new MyPathNode[width, height];

		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				grid[x, y] = new MyPathNode()
				{
					IsTower = false,
					X = x,
					Y = y,
				};
			}
		}  
	}

	public void SetGrid(int x, int y, bool isTower)
	{
		grid [x, y].IsTower = isTower;
	}

	public bool isWayFree()
	{
		return aStar.Search (departurePoint, arrivalPoint, null) != null;
	}
}
