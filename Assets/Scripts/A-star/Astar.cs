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

public class Astar : MonoBehaviour{

	private int width;
	private int height;
	private Point departurePoint;
	private Point arrivalPoint;
	private SpatialAStar<MyPathNode, Object> aStar;
	private MyPathNode[,] aStarGrid;

	public Astar()
	{
		width = 7;
		height = 8;
		FillGrid ();
		departurePoint = new Point(3, 0);
		arrivalPoint = new Point(3, 7);

		aStar = new SpatialAStar<MyPathNode, Object>(aStarGrid); 
	}

	private void FillGrid ()
	{
		aStarGrid = new MyPathNode[width, height];
		// setup grid with walls
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				aStarGrid[x, y] = new MyPathNode()
				{
					IsTower = false,
					X = x,
					Y = y,
				};
			}
		}  
		print ("fill : " + aStarGrid.Length);
	}

	private bool IsFreeWay()
	{
		//print ("4 0 " + aStarGrid[4,0].IsTower);
		/*for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				print (x + " " + y + " = " + aStarGrid [x, y].IsTower);
			}
		}*/
		return aStar.Search (departurePoint, arrivalPoint, null) != null;
	}

	public bool IsFreeWayWithFakeTower (int aStarCoordX, int aStarCoordY)
	{
		//print ("IsFreeWayWithFakeTower");
		/*if (aStarCoordX == 4 && aStarCoordY == 0) {
			print ("AVANT");
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					print (x + " " + y + " = " + aStarGrid [x, y].IsTower);
				}
			}
		}*/
		if (aStarGrid [aStarCoordX, aStarCoordY].IsTower)
			return false;
		
		SetAstarGrid (aStarCoordX, aStarCoordY, true);
		/*if (aStarCoordX == 4 && aStarCoordY == 0) {
			print ("APRES");
			for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					print (x + " " + y + " = " + aStarGrid [x, y].IsTower);
				}
			}
		}*/
		bool result = IsFreeWay();
		SetAstarGrid (aStarCoordX, aStarCoordY, false);
		return result;
	}

	public void SetAstarGrid(int aStarCoordX, int aStarCoordY, bool isTower)
	{
		if (aStarCoordX == 4 && aStarCoordY == 0) {
			print ("SetAstarGrid AVANT " + isTower + " " + aStarGrid[aStarCoordX, aStarCoordY].IsTower);
			/*for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					print (x + " " + y + " = " + aStarGrid [x, y].IsTower);
				}
			}*/
		}
		aStarGrid[aStarCoordX, aStarCoordY].IsTower = isTower;
		if (aStarCoordX == 4 && aStarCoordY == 0) {
			print ("SetAstarGrid APRES " + isTower + " " + aStarGrid[aStarCoordX, aStarCoordY].IsTower);
			/*for (int x = 0; x < width; x++)
			{
				for (int y = 0; y < height; y++)
				{
					print (x + " " + y + " = " + aStarGrid [x, y].IsTower);
				}
			}*/
		}
	}
}
