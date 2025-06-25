using Godot;
using System;
using WzComparerR2.WzLib;

namespace MapleStory
{
    public partial class Physics : Node2D
    {
        private FootholdTree? footholdTree;

        public override void _Ready()
        {
            footholdTree = GetNode<FootholdTree>("FootholdTree");
        }

        public void Init(Wz_Node source)
        {
            footholdTree?.Init(source);
        }
        public FootholdTree? GetFootholdTree()
        {
            return footholdTree;
        }
        public void MoveObject(PhysicsObject physicsObject)
        {
            // Determine which platform the object is currently on
            footholdTree?.UpdateFootHold(physicsObject);
            // Use the appropriate physics for the terrain the object is on
            switch (physicsObject.objectType)
            {
                case PhysicsObject.Type.Normal:
                    MoveNormal(physicsObject);
                    footholdTree?.LimitMovement(physicsObject);
                    break;
                case PhysicsObject.Type.Flying:
                    MoveFlying(physicsObject);
                    footholdTree?.LimitMovement(physicsObject);
                    break;
                case PhysicsObject.Type.Swimming:
                    MoveSwimming(physicsObject);
                    footholdTree?.LimitMovement(physicsObject);
                    break;
                case PhysicsObject.Type.Fixated:
                default:
                    break;
            }
        }
        private void MoveNormal(PhysicsObject physicsObject)
        {
            physicsObject.vAcceleration = 0;
            physicsObject.hAcceleration = 0;

            if (physicsObject.onGround)
            {
                physicsObject.vAcceleration += physicsObject.vForce;
                physicsObject.hAcceleration += physicsObject.hForce;

                if (physicsObject.hAcceleration == 0 && physicsObject.hspeed < 0.1 && physicsObject.hspeed > -0.1)
                {
                    physicsObject.hspeed = 0;
                }
                else
                {
                    double inertia = physicsObject.hspeed / Constants.Constant.GROUNDSLIP;
                    double slopeFriction = physicsObject.footHoldSlope;

                    if (slopeFriction > 0.5)
                    {
                        slopeFriction = 0.5;
                    }
                    else if (slopeFriction < -0.5)
                    {
                        slopeFriction = -0.5;
                    }

                    physicsObject.hAcceleration -= (Constants.Constant.FRICTION + Constants.Constant.SLOPEFACTOR * (1.0 + slopeFriction * -inertia)) * inertia;
                }
            }
            else if (physicsObject.IsFlagNotSet(PhysicsObject.Flag.NoGravity))
            {
                physicsObject.vAcceleration += Constants.Constant.GRAVFORCE;
            }

            physicsObject.hForce = 0;
            physicsObject.vForce = 0;

            physicsObject.hspeed += physicsObject.hAcceleration;
            physicsObject.vspeed += physicsObject.vAcceleration;
        }
        public void MoveFlying(PhysicsObject physicsObject)
        {

            physicsObject.hAcceleration = physicsObject.hForce;
            physicsObject.vAcceleration = physicsObject.vForce;
            physicsObject.hForce = 0;
            physicsObject.vForce = 0;

            physicsObject.hAcceleration -= Constants.Constant.FLYFRICTION * physicsObject.hspeed;
            physicsObject.vAcceleration -= Constants.Constant.FLYFRICTION * physicsObject.vspeed;

            physicsObject.hspeed += physicsObject.hAcceleration;
            physicsObject.vspeed += physicsObject.vAcceleration;

            if (physicsObject.hAcceleration == 0 && physicsObject.hspeed < 0.1 && physicsObject.hspeed > -0.1)
                physicsObject.hspeed = 0;

            if (physicsObject.vAcceleration == 0 && physicsObject.vspeed < 0.1 && physicsObject.vspeed > -0.1)
                physicsObject.vspeed = 0;
        }
        public void MoveSwimming(PhysicsObject physicsObject)
        {

            physicsObject.hAcceleration = physicsObject.hForce;
            physicsObject.vAcceleration = physicsObject.vForce;
            physicsObject.hForce = 0;
            physicsObject.vForce = 0;

            physicsObject.hAcceleration -= Constants.Constant.SWIMFRICTION * physicsObject.hspeed;
            physicsObject.vAcceleration -= Constants.Constant.SWIMFRICTION * physicsObject.vspeed;

            if (physicsObject.IsFlagNotSet(PhysicsObject.Flag.NoGravity))
                physicsObject.vAcceleration += Constants.Constant.SWIMGRAVFORCE;

            physicsObject.hspeed += physicsObject.hAcceleration;
            physicsObject.vspeed += physicsObject.vAcceleration;

            if (physicsObject.hAcceleration == 0 && physicsObject.hspeed < 0.1 && physicsObject.hspeed > -0.1)
                physicsObject.hspeed = 0;

            if (physicsObject.vAcceleration == 0 && physicsObject.vspeed < 0.1 && physicsObject.vspeed > -0.1)
                physicsObject.vspeed = 0;
        }
        public MaplePoint<int> GetYBelow(MaplePoint<int> position)
        {
            int ground = footholdTree?.GetYBelow(position) ?? throw new Exception("footholdTree is null");
            return new MaplePoint<int>(position.X, ground - 1);
        }
    }
}