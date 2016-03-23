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
	private int Width = 10;
	private int Height = 10;

	void FillGrid ()
	{
		// setup grid with walls
		grid = new MyPathNode[Width, Height];

		for (int x = 0; x < Width; x++)
		{
			for (int y = 0; y < Height; y++)
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

	void Search()
	{
		SpatialAStar<MyPathNode, Object> aStar = new SpatialAStar<MyPathNode, Object>(grid); 
		LinkedList<MyPathNode> path = aStar.Search(new Point(0, 0), new Point(Width - 2, Height - 2), null);
	}
}
