using Godot;
using System.Collections.Generic;

namespace MapleStory
{
    public partial class Player : Character
    {
        private static PlayerNullState nullState = new();
        private PlayerStandState standState = new();
        private PlayerWalkState walkState = new();
        private PlayerFallState fallState = new();
        private PlayerProneState proneState = new();
        private PlayerClimbState climbState = new();
        private PlayerSitState sittingState = new();
        private PlayerFlyState flyingState = new();

        private Dictionary<KeyAction.Id, bool> keysDown = [];
        private Ladder? ladder = null;
        private CharStats? stats;
        private bool underWater = false;
        private TimedBool climbCoolDown = new();
        private Movement lastMove;
        private RandomNumberGenerator randomizer = new();

        public override void _Ready()
        {
            base._Ready();
        }

        public void Init(CharEntry entry)
        {
            base.Init(entry.id, entry.look, entry.stats.name);
            stats = new CharStats(entry.stats);
            SetState(Character.State.STAND);
            SetDirection(true);
        }

        public bool IsAttacking()
        {
            return attacking;
        }

        public float GetWalkForce()
        {
            return 0.05f + 0.11f * stats!.GetTotal(EquipStat.Id.SPEED) / 100;
        }

        public float GetJumpForce()
        {
            return 1.0f + 3.5f * stats!.GetTotal(EquipStat.Id.JUMP) / 100;
        }

        public float GetClimbForce()
        {
            return stats!.GetTotal(EquipStat.Id.SPEED) / 100;
        }

        public float GetFlyForce()
        {
            return 0.25f;
        }

        public bool IsUnderWater()
        {
            return underWater;
        }

        public bool IsKeyDown(KeyAction.Id action)
        {
            return keysDown.ContainsKey(action) && keysDown[action];
        }

        public override bool IsInvincible()
	    {
		    if (state == Character.State.DIED)
			    return true;

/*		    if (HasBuff(Buffstat.Id.DARKSIGHT))
			    return true;*/

		    return base.IsInvincible();
	    }

        public BasePlayerState? GetState(State state)
        {
            switch (state)
            {
                case State.STAND:
                    return standState;
                case State.WALK:
                    return walkState;
                case State.FALL:
                    return fallState;
                case State.PRONE:
                    return proneState;
                case State.LADDER:
                case State.ROPE:
                    return climbState;
                case State.SIT:
                    return sittingState;
                case State.SWIM:
                    return flyingState;
                default:
                    return null;
            }
        }

        public MapleRectangle<int> GetPlayerRect()
        {
            MaplePoint<int> position = physicsObject.GetPosition();

            switch (state)
            {
                case State.WALK:
                case State.STAND:
                case State.FALL:
                case State.ALERT:
                case State.SWIM:
                case State.LADDER:
                case State.ROPE:
                    return new (new MaplePoint<int>(position.X - 10, position.Y - 50),
                        new MaplePoint<int>(position.X + 10, position.Y));
                case State.PRONE:
                    if (facingRight)
                        return new(new MaplePoint<int>(position.X - 15, position.Y - 40),
                            new MaplePoint<int>(position.X + 40, position.Y));
                    else
                        return new(new MaplePoint<int>(position.X - 45, position.Y - 40),
                            new MaplePoint<int>(position.X + 10, position.Y));
                case State.DIED:
                    return new();
                case State.SIT:
                    return new(new MaplePoint<int>(position.X - 10, position.Y - 50),
                        new MaplePoint<int>(position.X + 10, position.Y));
                default:
                    return new();
            }
        }

        public Ladder? GetLadder()
        {
            return ladder;
        }

        public override void SetState(State state)
        {
            if (!attacking)
            {
                base.SetState(state);
                BasePlayerState? playerState = GetState(state);

                if (playerState != null)
                {
                    playerState.Initialize(this);
                }
            }
        }

        public void SetLadder(Ladder? ladder)
        {
            this.ladder = ladder;
            if (ladder != null)
            {
                physicsObject.SetX(ladder.GetX());

                physicsObject.hspeed = 0.0;
                physicsObject.vspeed = 0.0;

                SetState(ladder.IsLadder() ? State.LADDER : State.ROPE);
            }
        }

        public void SetSeat(Seat seat)
        {
            if (seat != null)
            {
                SetPosition(seat.GetPosition());
                SetState(State.SIT);
            }
        }

        public void SetClimbCooldown()
        {
            climbCoolDown.SetFor(100);
        }

        public bool CanClimb()
        {
            return !climbCoolDown;
        }

        public void SendAction(KeyAction.Id action, bool down)
        {
            BasePlayerState? playerState = GetState(state);
            playerState?.SendAction(this, action, down);
            keysDown[action] = down;
        }

        public MobAttackResult Damage(MobAttack attack)
        {
            int damage = stats!.CalculateDamage(attack.watk);
            ShowDamage(damage);

            bool fromLeft = attack.origin.X > physicsObject.GetX();

            bool missed = damage <= 0;
            bool imMovable = (ladder != null) || (state == Character.State.DIED);
            bool knockBack = !missed && !imMovable;

            if (knockBack && randomizer.RandfRange(0, 1) > stats.GetStance())
            {
                physicsObject.hspeed = fromLeft ? -1.5 : 1.5;
                physicsObject.vForce -= 3.5;
            }

            int direction = fromLeft ? 0 : 1;

            return new MobAttackResult(attack, damage, direction);
        }

        public override void _Process(double delta)
        {
            base._Process(delta);
        }

        public override void _PhysicsProcess(double delta)
        {
            base._PhysicsProcess(delta);

            BasePlayerState? playerState = GetState(state);

            playerState?.Update(this);
            bool animationEnd = look!.IsAnimationEnded();
            if (animationEnd && attacking)
            {
                attacking = false;
                nullState.UpdateState(this);
            }
            else
                playerState?.UpdateState(this);

            int stanceByte = facingRight ? (int)state: (int)state + 1;
            Movement newMove = new(physicsObject, stanceByte);
            bool needUpdate = lastMove.HasMoved(newMove);

            if (needUpdate)
            {
                /*                MovePlayerPacket(newmove).dispatch();*/
                lastMove = newMove;
            }

            climbCoolDown.Update((uint)(delta * 1000));
        }
    }
}