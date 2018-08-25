﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SIS.Map;

//Must Be Created Dynamically By Dungeon Generator

namespace SIS.Waypoints
{
	//API that allows for characters to move through waypoint system
	//Use CurrentWaypoint to find what to navigate to
	public class WaypointNavigator : MonoBehaviour
	{

		public Dungeon dungeon;
		public float nextWaypointDistCheck = 1.2f;

		WaypointGraph waypointGraph;
		List<Waypoint> path;
		int pathIndex = -1; //current waypoint index

		public Waypoint CurrentWaypoint
		{
			get
			{
				if (pathIndex == -1)
					return new Waypoint();
				return path[pathIndex];
			}
		}

		private void Awake()
		{
			waypointGraph = new WaypointGraph(dungeon.waypointSystem, dungeon);
		}

		//TODO: Replace with FSM/BT
		private void Update()
		{
			CheckNextWaypoint();

			if (Input.GetKeyDown(KeyCode.C))
			{
				int roomIndex = Random.Range(0, dungeon.RoomCount);
				StartNavigation(roomIndex);
			}

			//Draw Debug Path Lines
			if (path != null)
			{
				for (int i = 0; i < path.Count - 1; ++i)
				{
					Debug.DrawLine(new Vector3(path[i].X, 0f, path[i].Y),
						new Vector3(path[i + 1].X, 0f, path[i + 1].Y), Color.red);
				}
			}
		}

		//Move Towards Current Waypoint
		void CheckNextWaypoint()
		{
			if (path == null) return;
			if (pathIndex < 0 || pathIndex >= path.Count) return;

			/*
			Waypoint waypoint = path[pathIndex];

			Vector3 target = new Vector3(waypoint.X, 0, waypoint.Y);
			Vector3 direction = (target - transform.position).normalized;

			transform.rotation = Quaternion.Slerp(transform.rotation,
				Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z)), 2f * Time.deltaTime);


			Vector3 motion = transform.forward * speed * Time.deltaTime;
			characterController.Move(motion);
			*/

			//Progressing Waypoints
			Vector3 target = new Vector3(CurrentWaypoint.X, 0, CurrentWaypoint.Y);
			if (Vector3.Distance(transform.position, target) < nextWaypointDistCheck)
			{
				++pathIndex;
			}
		}

		public void StartNavigation(int goalX, int goalY)
		{
			Waypoint start = waypointGraph.FindClosestWaypoint(transform.position);
			Waypoint goal = waypointGraph.FindClosestWaypoint(new Vector3(goalX, 0f, goalY));

			path = waypointGraph.AStar(start, goal);
			pathIndex = 0;
		}
		#region StartNavigation Overloads

		public void StartNavigation(Vector3 goalPos)
		{
			StartNavigation((int)goalPos.x, (int)goalPos.z);
		}

		public void StartNavigation(Vector2Int goalPos)
		{
			StartNavigation(goalPos.x, goalPos.y);
		}

		public void StartNavigation(Waypoint wp)
		{
			StartNavigation(wp.X, wp.Y);
		}

		public void StartNavigation(int roomIndex)
		{
			if (roomIndex < 0 || roomIndex >= dungeon.RoomCount)
			{
				Debug.LogWarning("Room Index Out of Bound");
				return;
			}
			StartNavigation(waypointGraph.GetCenterRoomWaypoint(roomIndex));
		}
		#endregion


	}
}