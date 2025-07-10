namespace MapleStory
{
    public abstract class BasePlayerState
    {
        // Actions taken when transitioning into the state
        public abstract void Initialize(Player player);
        // How to handle inputs while in the state
        public abstract void SendAction(Player player, KeyAction.Id action, bool pressed);
        // Actions taken in the player's update method, before physics are applied.
        public abstract void Update(Player player);
        // Transition into a new state after physics have been applied
        public abstract void UpdateState(Player player);
        public void PlayJumpSound() { /* TODO */ }
        public bool HasWalkInput(Player player)
        {
            return player.IsKeyDown(KeyAction.Id.LEFT) || player.IsKeyDown(KeyAction.Id.RIGHT);
        }

        public bool HasLeftInput(Player player)
        {
            return player.IsKeyDown(KeyAction.Id.LEFT) && !player.IsKeyDown(KeyAction.Id.RIGHT);
        }

        public bool HasRightInput(Player player)
        {
            return player.IsKeyDown(KeyAction.Id.RIGHT) && !player.IsKeyDown(KeyAction.Id.LEFT);
        }
    }
    public class PlayerNullState : BasePlayerState
    {
        public override void Initialize(Player player) { }
        public override void SendAction(Player player, KeyAction.Id action, bool pressed) { }

        public override void Update(Player player) { }

        public override void UpdateState(Player player)
        {
            Character.State state;

            if (player.GetPhysicObject().onGround)
            {
                if (player.IsKeyDown(KeyAction.Id.LEFT))
                {
                    state = Character.State.WALK;
                    player.SetDirection(false);
                }
                else if (player.IsKeyDown(KeyAction.Id.RIGHT))
                {
                    state = Character.State.WALK;
                    player.SetDirection(true);
                }
                else if (player.IsKeyDown(KeyAction.Id.DOWN))
                {
                    state = Character.State.PRONE;
                }
                else
                {
                    state = Character.State.STAND;
                }
            }
            else
            {
                Ladder? ladder = player.GetLadder();
                if (ladder != null)
                    state = ladder.IsLadder() ? Character.State.LADDER : Character.State.ROPE;
                else
                    state = Character.State.FALL;
            }

            player.GetPhysicObject().ClearFlags();
            player.SetState(state);
        }
    }
    public class PlayerStandState : BasePlayerState
    {
        public override void Initialize(Player player)
        {
            player.GetPhysicObject().objectType = PhysicsObject.Type.Normal;
        }
        public override void SendAction(Player player, KeyAction.Id action, bool pressed)
        {
            if (player.IsAttacking())
                return;

            if (pressed && action == KeyAction.Id.JUMP)
            {
                /*            play_jumpsound();*/
                player.GetPhysicObject().vForce = -player.GetJumpForce();
            }
        }

        public override void Update(Player player)
        {
            if (player.GetPhysicObject().enableJumpDown == false)
            {
                player.GetPhysicObject().SetFlag(PhysicsObject.Flag.CheckBelow);
            }
            if (player.IsAttacking())
            {
                return;
            }

            if (HasRightInput(player))
            {
                player.SetDirection(true);
                player.SetState(Character.State.WALK);
            }
            else if (HasLeftInput(player))
            {
                player.SetDirection(false);
                player.SetState(Character.State.WALK);
            }

            if (player.IsKeyDown(KeyAction.Id.DOWN) && !player.IsKeyDown(KeyAction.Id.UP) && !HasWalkInput(player))
            {
                player.SetState(Character.State.PRONE);
            }
        }
        public override void UpdateState(Player player)
        {
            if (!player.GetPhysicObject().onGround)
            {
                player.SetState(Character.State.FALL);
            }
        }
    }
    public class PlayerWalkState : BasePlayerState
    {
        public override void Initialize(Player player)
        {
            player.GetPhysicObject().objectType = PhysicsObject.Type.Normal;
        }
        public override void SendAction(Player player, KeyAction.Id action, bool pressed)
        {
            if (player.IsAttacking())
                return;

            if (pressed && action == KeyAction.Id.JUMP)
            {
                /*            play_jumpsound();*/

                player.GetPhysicObject().vForce = -player.GetJumpForce();
            }

            if (pressed && action == KeyAction.Id.JUMP && player.IsKeyDown(KeyAction.Id.DOWN) && player.GetPhysicObject().enableJumpDown)
            {
                /*            play_jumpsound();*/

                player.GetPhysicObject().y.UpdateValue(player.GetPhysicObject().groundBelow);
                player.SetState(Character.State.FALL);
            }
        }
        public override void Update(Player player)
        {
            if (player.GetPhysicObject().enableJumpDown == false)
                player.GetPhysicObject().SetFlag(PhysicsObject.Flag.CheckBelow);

            if (player.IsAttacking())
                return;

            if (HasWalkInput(player))
            {
                if (HasRightInput(player))
                {
                    player.SetDirection(true);
                    player.GetPhysicObject().hForce += player.GetWalkForce();
                }
                else if (HasLeftInput(player))
                {
                    player.SetDirection(false);
                    player.GetPhysicObject().hForce += -player.GetWalkForce();
                }
            }
            else
            {
                if (player.IsKeyDown(KeyAction.Id.DOWN))
                    player.SetState(Character.State.PRONE);
            }
        }
        public override void UpdateState(Player player)
        {
            if (player.GetPhysicObject().onGround)
            {
                if (!HasWalkInput(player) || player.GetPhysicObject().hspeed == 0)
                    player.SetState(Character.State.STAND);
            }
            else
            {
                player.SetState(Character.State.FALL);
            }
        }
    }
    public class PlayerFallState : BasePlayerState
    {
        public override void Initialize(Player player)
        {
            player.GetPhysicObject().objectType = PhysicsObject.Type.Normal;

        }
        public override void SendAction(Player player, KeyAction.Id action, bool pressed)
        {
        }
        public override void Update(Player player)
        {
            if (player.IsAttacking())
                return;

            double hspeed = player.GetPhysicObject().hspeed;

            if (HasLeftInput(player) && hspeed > 0)
                hspeed -= 0.025;
            else if (HasRightInput(player) && hspeed < 0.0)
                hspeed += 0.025;

            if (HasLeftInput(player))
                player.SetDirection(false);
            else if (HasRightInput(player))
                player.SetDirection(true);
        }
        public override void UpdateState(Player player)
        {
            if (player.GetPhysicObject().onGround)
            {
                if (player.IsKeyDown(KeyAction.Id.DOWN) && !HasWalkInput(player))
                    player.SetState(Character.State.PRONE);
                else
                    player.SetState(Character.State.STAND);
            }
            else if (player.IsUnderWater())
            {
                player.SetState(Character.State.SWIM);
            }
        }
    }
    public class PlayerProneState : BasePlayerState
    {
        public override void Initialize(Player player)
        {
        }
        public override void SendAction(Player player, KeyAction.Id action, bool pressed)
        {
            if (pressed && action == KeyAction.Id.JUMP)
            {
                if (player.GetPhysicObject().enableJumpDown && player.IsKeyDown(KeyAction.Id.DOWN))
                {
                    /*                play_jumpsound();*/

                    player.GetPhysicObject().y.UpdateValue(player.GetPhysicObject().groundBelow);
                    player.SetState(Character.State.FALL);
                }
            }
        }
        public override void Update(Player player)
        {
            if (player.GetPhysicObject().enableJumpDown == false)
                player.GetPhysicObject().SetFlag(PhysicsObject.Flag.CheckBelow);

            if (player.IsKeyDown(KeyAction.Id.UP) || !player.IsKeyDown(KeyAction.Id.DOWN))
                player.SetState(Character.State.STAND);

            if (player.IsKeyDown(KeyAction.Id.LEFT))
            {
                player.SetDirection(false);
                player.SetState(Character.State.WALK);
            }

            if (player.IsKeyDown(KeyAction.Id.RIGHT))
            {
                player.SetDirection(true);
                player.SetState(Character.State.WALK);
            }
        }
        public override void UpdateState(Player player)
        {
        }
    }
    public class PlayerSitState : BasePlayerState
    {
        public override void Initialize(Player player)
        {
        }
        public override void SendAction(Player player, KeyAction.Id action, bool down)
        {
            if (down)
            {
                switch (action)
                {
                    case KeyAction.Id.LEFT:
                        player.SetDirection(false);
                        player.SetState(Character.State.WALK);
                        break;
                    case KeyAction.Id.RIGHT:
                        player.SetDirection(true);
                        player.SetState(Character.State.WALK);
                        break;
                    case KeyAction.Id.JUMP:
                        player.SetState(Character.State.STAND);
                        break;
                }
            }
        }
        public override void Update(Player player)
        {
        }
        public override void UpdateState(Player player)
        {
        }
    }
    public class PlayerFlyState : BasePlayerState
    {
        public override void Initialize(Player player)
        {
            player.GetPhysicObject().objectType = player.IsUnderWater() ? PhysicsObject.Type.Swimming : PhysicsObject.Type.Flying;
        }
        public override void SendAction(Player player, KeyAction.Id action, bool down)
        {
            if (down)
            {
                switch (action)
                {
                    case KeyAction.Id.LEFT:
                        player.SetDirection(false);
                        break;
                    case KeyAction.Id.RIGHT:
                        player.SetDirection(true);
                        break;
                }
            }
        }
        public override void Update(Player player)
        {
            if (player.IsAttacking())
                return;

            if (player.IsKeyDown(KeyAction.Id.LEFT))
                player.GetPhysicObject().hForce = -player.GetFlyForce();
            else if (player.IsKeyDown(KeyAction.Id.RIGHT))
                player.GetPhysicObject().hForce = player.GetFlyForce();

            if (player.IsKeyDown(KeyAction.Id.UP))
                player.GetPhysicObject().vForce = -player.GetFlyForce();
            else if (player.IsKeyDown(KeyAction.Id.DOWN))
                player.GetPhysicObject().vForce = player.GetFlyForce();
        }
        public override void UpdateState(Player player)
        {
            if (player.GetPhysicObject().onGround && player.IsUnderWater())
            {
                Character.State state;

                if (player.IsKeyDown(KeyAction.Id.LEFT))
                {
                    state = Character.State.WALK;

                    player.SetDirection(false);
                }
                else if (player.IsKeyDown(KeyAction.Id.RIGHT))
                {
                    state = Character.State.WALK;

                    player.SetDirection(true);
                }
                else if (player.IsKeyDown(KeyAction.Id.DOWN))
                {
                    state = Character.State.PRONE;
                }
                else
                {
                    state = Character.State.STAND;
                }

                player.SetState(state);
            }
        }
    }
    public class PlayerClimbState : BasePlayerState
    {
        private bool climbContinue;
        public override void Initialize(Player player)
        {
            player.GetPhysicObject().objectType = PhysicsObject.Type.Fixated;
            climbContinue = true;
        }
        public override void SendAction(Player player, KeyAction.Id action, bool pressed)
        {
        }
        public override void Update(Player player)
        {
            if (player.IsKeyDown(KeyAction.Id.UP) && !player.IsKeyDown(KeyAction.Id.DOWN))
            {
                if (climbContinue)
                {
                    player.GetPhysicObject().vspeed = -player.GetClimbForce();
                }
                else
                {

                    player.GetPhysicObject().vspeed = 0.0;
                }
            }
            else if (player.IsKeyDown(KeyAction.Id.DOWN) && !player.IsKeyDown(KeyAction.Id.UP))
            {
                player.GetPhysicObject().vspeed = player.GetClimbForce();
            }
            else
            {
                player.GetPhysicObject().vspeed = 0.0;
            }

            if (player.IsKeyDown(KeyAction.Id.JUMP) && HasWalkInput(player))
            {
                /*            play_jumpsound();*/

                float walkforce = player.GetWalkForce() * 8;

                player.SetDirection(player.IsKeyDown(KeyAction.Id.RIGHT));

                player.GetPhysicObject().hspeed = player.IsKeyDown(KeyAction.Id.LEFT) ? -walkforce : walkforce;
                player.GetPhysicObject().vspeed = -player.GetJumpForce() / 1.5;

                CancelLadder(player);
            }
        }
        public override void UpdateState(Player player)
        {
            int y = player.GetPhysicObject().GetY();
            bool upwards = player.IsKeyDown(KeyAction.Id.UP);
            bool downwards = player.IsKeyDown(KeyAction.Id.DOWN);
            Ladder? ladder = player.GetLadder();

            if (ladder != null)
            {
                climbContinue = true;
                if (upwards)
                {
                    if (ladder.FellOff(y, downwards) && ladder.HaveUpperFloor())
                    {
                        CancelLadder(player);
                    }
                    else if (ladder.FellOff(y, downwards) && !ladder.HaveUpperFloor())
                    {
                        climbContinue = false;
                    }
                }
                else if (downwards)
                {
                    if (ladder.FellOff(y, downwards))
                    {
                        CancelLadder(player);
                    }
                }
            }
        }
        public void CancelLadder(Player player)
        {
            player.SetState(Character.State.FALL);
            player.SetLadder(null);
            player.SetClimbCooldown();
        }
    }
}