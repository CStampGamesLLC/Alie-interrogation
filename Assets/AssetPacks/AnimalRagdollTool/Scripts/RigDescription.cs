//***********************************************************************************************
//** RigDescription.cs                                                                       
                  
//** by Eric P Smith (battlecrafters.com)                                                        
//***********************************************************************************************

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BattleCrafters
{
	public enum ColliderTypes
	{
		None = 0,
		Box,
		Capsule,
		Sphere,
		SphereCentered,
	}

	public enum BendTypes
	{
		None = 0,
		Forwards,
		Backwards,
	}

	public enum CapsuleDirection
	{
		XAxis = 0,
		YAxis,
		ZAxis,
	}

	public enum CoordinateSystem
	{
		Y_Up = 0,
		Z_Up,
	}

	[System.Serializable]
	public class BaseRagdollValues
	{
		public float totalMass = 60f;
		public bool distributeMass = false;
		public float lateralAngle = 135f;
		public float bendAngle = 15f;
		public CoordinateSystem coordSystem;

		public void CopyFrom(BaseRagdollValues values)
		{
			totalMass = values.totalMass;
			distributeMass = values.distributeMass;
			lateralAngle = values.lateralAngle;
			bendAngle = values.bendAngle;
			coordSystem = values.coordSystem;
		}
	}

	[System.Serializable]
	public class RigDescription
	{
		public string label;
		public ColliderTypes colType;
		public BendTypes bendType;
		public CapsuleDirection direction;

		public bool dirty = false;

		public bool required = true;
		public bool hasRB = false;
		public bool hasJoint = false;
		public bool reverseLateralAngle = false;

		public int connectsToInt;
		public int runsToInt;

		public int category;
		public Transform source;
		public Transform connectedTo;
		public Transform runsTo;
		public Rigidbody rb;
		public Collider col;
		public ConfigurableJoint joint;
		public AnimalRagdollRig ragdollRig;

		public void CopyFrom(RigDescription fromrig)
		{
			label = fromrig.label;
			colType = fromrig.colType;
			bendType = fromrig.bendType;
			direction = fromrig.direction;
			required = fromrig.required;
			hasRB = fromrig.hasRB;
			hasJoint = fromrig.hasJoint;
			connectsToInt = fromrig.connectsToInt;
			runsToInt = fromrig.runsToInt;
			category = fromrig.category;
			source = fromrig.source;
			connectedTo = fromrig.connectedTo;
			runsTo = fromrig.runsTo;
			rb = fromrig.rb;
			col = fromrig.col;
			joint = fromrig.joint;
			ragdollRig = fromrig.ragdollRig;
		}
	}
}