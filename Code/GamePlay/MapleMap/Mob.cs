using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using WzComparerR2.WzLib;
using RandomNumberGenerator = Godot.RandomNumberGenerator;

namespace MapleStory
{
    public partial class Mob : MapObject
    {
        public enum Stance : int
        {
            MOVE = 2,
            STAND = 4,
            JUMP = 6,
            HIT = 8,
            DIE = 10
        };

        private readonly string[] StanceNames =
        [
            "move",
            "stand",
            "jump",
            "hit1",
            "die1",
            "fly"
        ];

        public string NameOf(Stance stance)
        {
            int index = ((int)stance - 1) / 2;
            return StanceNames[index];
        }

        public static int ValueOf(Stance stance, bool flip)
        {
            return flip ? (int)stance : (int)stance + 1;
        }

        private enum FlyDirection
        {
            STRAIGHT,
            UPWARDS,
            DOWNWARDS,
            NUM_DIRECTIONS
        };

        private Dictionary<Stance, MapleAnimation> animations = [];
        private EffectLayer effects = new();
        private Label nameLabel = new();
        private MobHpBar hpBar = new();
        private TimedBool showHp = new();
        private List<Movement> movements = [];
        private Stance stance;
        private FlyDirection flyDirection;
        private Linear<float> opacity = new();
        RandomNumberGenerator randomizer = new();

        private string name;
        /*    Sound hitsound;
            Sound diesound;*/
        private int level;
        private float speed;
        private float flySpeed;
        private int watk;
        private int matk;
        private int wdef;
        private int mdef;
        private int accuracy;
        private int avoid;
        private int knockBack;
        private bool undead;
        private bool touchDamage;
        private bool noFlip;
        private bool notAttack;
        private bool canMove;
        private bool canJump;
        private bool canFly;
        private int counter;
        private int mobId;
        private int effect;
        private int team;
        private bool dying;
        private bool dead;
        private bool control;
        private bool aggro;
        private bool flip;
        private float walkforce;
        private int hpPercent;
        private bool fading;
        private bool fadein;

        private Camera? camera;

        public override void _Ready()
        {
            camera = GetNode<Camera>($"/root/Root/ViewportContainer/SubViewport/Stage/Camera");
            LayerChanged += OnLayerChanged;

            base._Ready();

            AddChild(effects);
            AddChild(nameLabel);
            AddChild(hpBar);

            ConfigureLabels();

            foreach (var anim in animations)
            {
                anim.Value.ZIndex = 0;
                anim.Value.Visible = false;
                AddChild(anim.Value);
            }

            SetStance(stance);

            nameLabel.ZIndex = 1;
            hpBar.ZIndex = 2;
        }

        public Mob(int objectId, int mobId, int mode, int stance, int foothold, bool newSpawn, int team, MaplePoint<int> position)
        {
            Init(objectId, position);

            string strId = mobId.ToString().PadLeft(7, '0');
            Wz_Node src = WzLib.wzs.WzNode.FindNodeByPath(true, "Mob", $"{strId}.img");
            Wz_Node info = src.FindNodeByPath("info");

            level = info.FindNodeByPath("level").GetValue<int>();
            watk = info.FindNodeByPath("PADamage").GetValue<int>();
            matk = info.FindNodeByPath("MADamage").GetValue<int>();
            wdef = info.FindNodeByPath("PDDamage").GetValue<int>();
            mdef = info.FindNodeByPath("MDDamage").GetValue<int>();
            accuracy = info.FindNodeByPath("acc").GetValue<int>();
            avoid = info.FindNodeByPath("eva").GetValue<int>();
            knockBack = info.FindNodeByPath("pushed").GetValue<int>();
            speed = info.FindNodeByPath("speed").GetValue<int>();
            flySpeed = info.FindNodeByPath("flySpeed")?.GetValue<int>() ?? 0;
            touchDamage = (info.FindNodeByPath("bodyAttack")?.GetValue<int>() ?? 0) == 1;
            undead = (info.FindNodeByPath("undead")?.GetValue<int>() ?? 0) == 1;
            noFlip = (info.FindNodeByPath("noFlip")?.GetValue<int>() ?? 0) == 1;
            notAttack = (info.FindNodeByPath("notAttack")?.GetValue<int>() ?? 0) == 1;
            canJump = src.FindNodeByPath("jump")?.Nodes.Count > 0;
            canFly = src.FindNodeByPath("fly")?.Nodes.Count > 0;
            canMove = src.FindNodeByPath("move")?.Nodes.Count > 0 || canFly;

            string linkId = info.FindNodeByPath("link")?.GetValue<string>() ?? string.Empty;

            Wz_Node link;
            if (linkId != string.Empty)
                link = WzLib.wzs.WzNode.FindNodeByPath(true, "Mob", $"{strId}.img");
            else
                link = src;

            Wz_Node fly = link.FindNodeByPath("fly");
            if (canFly)
            {
                animations[Stance.STAND] = new MapleAnimation(fly);
                animations[Stance.MOVE] = new MapleAnimation(fly);
            }
            else
            {
                animations[Stance.STAND] = new MapleAnimation(link.FindNodeByPath("stand"));
                animations[Stance.MOVE] = new MapleAnimation(link.FindNodeByPath("move"));
            }

            if (canJump)
                animations[Stance.JUMP] = new MapleAnimation(link.FindNodeByPath("jump"));

            if (link.FindNodeByPath("hit1") != null)
                animations[Stance.HIT] = new MapleAnimation(link.FindNodeByPath("hit1"));

            if (link.FindNodeByPath("die1") != null)
                animations[Stance.DIE] = new MapleAnimation(link.FindNodeByPath("die1"));

            name = WzLib.wzs.WzNode.FindNodeByPath(true, "String", "Mob.img", $"{mobId}", "name").GetValue<string>();

            /*        var sndSrc = NL.NX.Sound["Mob.img"][strId];
                    HitSound = sndSrc["Damage"].As<AudioStreamPlayer>();
                    DieSound = sndSrc["Die"].As<AudioStreamPlayer>();*/

            speed = (speed + 100) * 0.001f;
            flySpeed = (flySpeed + 100) * 0.0005f;

            if (canFly)
                physicsObject.objectType = PhysicsObject.Type.Flying;

            this.mobId = mobId;
            this.team = team;
            SetControl(mode);
            physicsObject.footHoldId = foothold;
            physicsObject.SetFlag(PhysicsObject.Flag.TurnAtEdges);

            dying = false;
            dead = false;
            fading = false;
            SetStance(stance);
            counter = 0;

            nameLabel.Text = name;

            // Handle fade in effect
            if (newSpawn)
            {
                fadein = true;
                opacity = new Linear<float>();
                opacity.SetAllValue(0.0f);
            }
            else
            {
                fadein = false;
                opacity = new Linear<float>();
                opacity.SetAllValue(1.0f);
            }

            if (control && stance == (int)Stance.STAND)
                NextMove();
        }

        // Set the stance by byte value
        public void SetStance(int stanceByte)
        {
            flip = (stanceByte % 2) == 0;

            if (!flip)
                stanceByte -= 1;

            if (stanceByte < (int)Stance.MOVE)
                stanceByte = (int)Stance.MOVE;

            SetStance((Stance)stanceByte);
        }

        // Set the stance by enumeration value
        public void SetStance(Stance newStance)
        {
            if (animations.TryGetValue(stance, out var anim))
                anim.Visible = false;

            if (stance != newStance)
            {
                stance = newStance;
                animations[stance].Reset();
            }

            animations[stance].Visible = true;
        }

        // Decide on the next state
        public void NextMove()
        {
            if (canMove)
            {
                switch (stance)
                {
                    case Stance.HIT:
                        SetStance(Stance.MOVE);
                        break;
                    case Stance.STAND:
                        SetStance(Stance.MOVE);
                        flip = randomizer.RandiRange(0, 1) == 1;
                        break;
                    case Stance.MOVE:
                    case Stance.JUMP:
                        if (canJump && physicsObject.onGround && randomizer.RandfRange(0, 1) <= 0.25f)
                        {
                            SetStance(Stance.JUMP);
                        }
                        else
                        {
                            switch (randomizer.RandiRange(0, 2))
                            {
                                case 0:
                                    SetStance(Stance.STAND);
                                    break;
                                case 1:
                                    SetStance(Stance.MOVE);
                                    flip = false;
                                    break;
                                case 2:
                                    SetStance(Stance.MOVE);
                                    flip = true;
                                    break;
                            }
                        }

                        break;
                }

                if (stance == Stance.MOVE && canFly)
                    flyDirection = (FlyDirection)randomizer.RandiRange(0, 2);
            }
            else
            {
                SetStance(Stance.STAND);
            }
        }

        public List<(int, bool)> CalculateDamage(Attack attack)
        {
            double minDamage = 0;
            double maxDamage = 0;
            float hitChance = 0;
            float critical = 0;
            int levelDelta = level - attack.playerLevel;

            if (levelDelta < 0)
                levelDelta = 0;

            Attack.DamageType damageType = attack.damageType;

            switch (damageType)
            {
                case Attack.DamageType.DMG_WEAPON:
                case Attack.DamageType.DMG_MAGIC:
                    minDamage = CalculateMinDamage(levelDelta, attack.minDamage, damageType == Attack.DamageType.DMG_MAGIC);
                    maxDamage = CalculateMaxDamage(levelDelta, attack.maxDamage, damageType == Attack.DamageType.DMG_MAGIC);
                    hitChance = CalculateHitChance(levelDelta, attack.accuracy);
                    critical = attack.critical;
                    break;
                case Attack.DamageType.DMG_FIXED:
                    minDamage = attack.fixDamage;
                    maxDamage = attack.fixDamage;
                    hitChance = 1.0f;
                    critical = 0.0f;
                    break;
            }

            List<(int, bool)> result = [];
            result = [.. Enumerable.Range(0, attack.hitCount).Select(_ => NextDamage(minDamage, maxDamage, hitChance, critical))];

            UpdateMovement();

            return result;
        }

        public (int, bool) NextDamage(double minDamage, double maxDamage, float hitChance, float critical)
        {
            bool hit = randomizer.RandfRange(0, 1) < hitChance;

            if (!hit)
                return (0, false);

            double DAMAGECAP = 999999.0d;

            double damage = randomizer.RandfRange((float)minDamage, (float)maxDamage);
            bool isCritical = randomizer.RandfRange(0, 1) < critical;

            if (isCritical)
                damage *= 1.5;

            if (damage < 1)
                damage = 1;
            else if (damage > DAMAGECAP)
                damage = DAMAGECAP;
            int intDamage = (int)damage;

            return (intDamage, isCritical);
        }

        public float CalculateHitChance(int levelDelta, int playerAccuracy)
        {
            float floatAccuracy = (float)playerAccuracy;
            float hitChance = floatAccuracy / (((1.84f + 0.07f * levelDelta) * avoid) + 1.0f);

            if (hitChance < 0.01f)
                hitChance = 0.01f;

            return hitChance;
        }
        public double CalculateMinDamage(int levelDelta, double damage, bool magic)
        {

            double mindamage = magic ?
                damage - (1 + 0.01 * levelDelta) * mdef * 0.6 :
                damage * (1 - 0.01 * levelDelta) - wdef * 0.6;

            return mindamage < 1.0 ? 1.0 : mindamage;
        }

        public double CalculateMaxDamage(int levelDelta, double damage, bool magic)
        {

            double maxdamage = magic ?
                damage - (1 + 0.01 * levelDelta) * mdef * 0.5 :
                damage * (1 - 0.01 * levelDelta) - wdef * 0.5;

            return maxdamage < 1.0 ? 1.0 : maxdamage;
        }

        // Change this mob's control mode:
        // 0 - no control, 1 - control, 2 - aggro
        public void SetControl(int mode)
        {
            control = mode > 0;
            aggro = mode == 2;
        }

        // Send the current position and state to the server
        public void UpdateMovement()
        {
/*            MoveMobPacket(
                oid, 1, 0, 0, 0, 0, 0, 0,
                get_position(),
                Movement(phobj, value_of(stance, flip))
            ).dispatch();*/
        }

        public MaplePoint<int> GetHeadPosition(MaplePoint<int> position)
        {
            MaplePoint<int> head = animations[stance].GetHead();

            position.ShiftX((flip && !noFlip) ? -head.X : head.X);
            position.ShiftY(head.Y);

            return position;
        }

        public void SendMovement(MaplePoint<int> start, List<Movement> inMovements)
        {
            if (control)
                return;

            SetPosition(start);
            movements = [.. inMovements];

            if (movements.Count == 0)
                return;

            Movement lastMove = movements[0];

            int lastStance = lastMove.newState;
            SetStance(lastStance);

            physicsObject.footHoldId = lastMove.footHold;
        }

        public void ShowHp(int percent, int playerLevel)
        {
            if (hpPercent == 0)
            {
                int delta = playerLevel - level;

                if (delta > 9)
                    nameLabel.AddThemeColorOverride("font_color", Colors.Yellow);
                else if (delta < -9)
                    nameLabel.AddThemeColorOverride("font_color", Colors.Red);
                else
                    nameLabel.RemoveThemeColorOverride("font_color");
            }

            if (percent > 100)
                percent = 100;
            else if (percent < 0)
                percent = 0;

            hpPercent = percent;
            showHp.SetFor(2000);
        }

        public MaplePoint<int> GetHeadPosition()
	    {
            MaplePoint<int> position = GetPosition();

		    return GetHeadPosition(position);
	    }

        public MobAttack CreateTouchAttack()
	    {
            if (!touchDamage)
                return new();

            int minAttack = (int)(watk * 0.8f);
            int maxAttack = watk;
            int attack = randomizer.RandiRange(minAttack, maxAttack);

		    return new MobAttack(attack, GetPosition(), mobId, objectId);
	    }

        private void ConfigureLabels()
        {
            Font font = GD.Load<Font>("res://Fonts/arial.ttf");

            nameLabel.AddThemeFontOverride("font", font);
            nameLabel.AddThemeFontSizeOverride("font_size", 13);
            nameLabel.HorizontalAlignment = HorizontalAlignment.Center;
            nameLabel.VerticalAlignment = VerticalAlignment.Center;
            nameLabel.Modulate = Colors.White;

            StyleBoxFlat nameTagBackground = new()
            {
                BgColor = new Color(0, 0, 0, 0.5f),
                CornerRadiusBottomLeft = 4,
                CornerRadiusBottomRight = 4,
                CornerRadiusTopLeft = 4,
                CornerRadiusTopRight = 4,
                ContentMarginLeft = 8,
                ContentMarginRight = 8,
                ContentMarginTop = 2,
                ContentMarginBottom = 2
            };

            nameLabel.AddThemeStyleboxOverride("normal", nameTagBackground);
        }

        public void OnLayerChanged(int objectId, int oldLayer, int newLayer)
        {
            Node? previousPlayerNode = GetParent<Node>();
            CanvasLayer targetPlayerNode = GetNode<CanvasLayer>($"/root/Root/ViewportContainer/SubViewport/Stage/Layer{newLayer}/MapMobs_3");

            previousPlayerNode?.RemoveChild(this);
            targetPlayerNode.AddChild(this);
        }

        public void ApplyDamage(int damage, bool toleft)
        {
/*            hitsound.play();*/

            if (dying && stance != Stance.DIE)
            {
                ApplyDeath();
            }
            else if (control && IsAlive() && damage >= knockBack)
            {
                flip = toleft;
                counter = 170;
                SetStance(Stance.HIT);

                UpdateMovement();
            }
        }

        public void ApplyDeath()
        {
            SetStance(Stance.DIE);
/*            diesound.play();*/
            dying = true;
        }

        public bool IsAlive()
	    {
		    return active && !dying;
	    }

        public bool IsInRange(MapleRectangle<int> range)
        {
            if (!active)
                return false;

            MapleRectangle<int>? mobRect = null;
            if (animations.TryGetValue(stance, out var anim))
                mobRect = anim.GetBounds();

            if (mobRect != null)
            {
                mobRect.Shift(GetPosition());
                return range.Overlaps(mobRect);
            }

            return false;
        }

        public override void _Process(double delta)
        {
            float alpha = (float)Engine.GetPhysicsInterpolationFraction();
            MaplePoint<double> realPosition = camera!.RealPosition(alpha);
            MaplePoint<int> absPosition = physicsObject.GetAbsolute(realPosition.X, realPosition.Y);
            GlobalPosition = absPosition.ToVector2();
            MaplePoint<int> headPosition = GetHeadPosition(absPosition);

            effects?.Interpolate(new());

            if (!dead)
            {
                float interOpacity = opacity.Get(alpha);
                animations[stance].GlobalPosition = GlobalPosition;
                animations[stance].Interpolate(new DrawArgument(new(), flip && !noFlip, interOpacity));

                if (showHp.Value && !dying && hpPercent > 0)
                {
                    hpBar.Visible = true;
                    hpBar.GlobalPosition = headPosition.ToVector2();
                    hpBar.Interpolate(hpPercent);
                }
                else
                    hpBar.Visible = false;
            }
        }

        public override void _PhysicsProcess(double delta)
        {
            if (!active)
                return;

            bool animationEnd = animations[stance].IsAnimationEnd();
            if (animationEnd && stance == Stance.DIE)
                dead = true;

            if (fading)
            {
                opacity.UpdateValue(opacity - 0.025f);
                if (opacity.Last() < 0.025f)
                {
                    fading = false;
                    dead = true;
                }
            }
            else if (fadein)
            {
                opacity.Add(0.025f);
                if (opacity.Last() > 0.975f)
                {
                    opacity.SetAllValue(1.0f);
                    fadein = false;
                }
            }

            if (dead)
            {
                Deactivate();
                return;
            }

            showHp.Update((uint)(delta * 1000));

            if (!dying)
            {
                if (!canFly)
                {
                    if (physicsObject.IsFlagNotSet(PhysicsObject.Flag.TurnAtEdges))
                    {
                        flip = !flip;
                        physicsObject.SetFlag(PhysicsObject.Flag.TurnAtEdges);

                        if (stance == Stance.HIT)
                            SetStance(Stance.STAND);
                    }
                }

                switch (stance)
                {
                    case Stance.MOVE:
                        if (canFly)
                        {
                            physicsObject.hForce = flip ? flySpeed : -flySpeed;

                            switch (flyDirection)
                            {
                                case FlyDirection.UPWARDS:
                                    physicsObject.vForce = -flySpeed;
                                    break;
                                case FlyDirection.DOWNWARDS:
                                    physicsObject.vForce = flySpeed;
                                    break;
                            }
                        }
                        else
                        {
                            physicsObject.hForce = flip ? speed : -speed;
                        }

                        break;
                    case Stance.HIT:
                        if (canMove)
                        {
                            double KBFORCE = physicsObject.onGround ? 0.2 : 0.1;
                            physicsObject.hForce = flip ? -KBFORCE : KBFORCE;
                        }

                        break;
                    case Stance.JUMP:
                        physicsObject.vForce = -5.0;
                        break;
                }

                int oldLayer = physicsObject.footHoldLayer;
                physics?.MoveObject(physicsObject);
                int newLayer = physicsObject.footHoldLayer;

                if (newLayer != oldLayer)
                    EmitSignal(SignalName.LayerChanged, objectId, oldLayer, newLayer);

                if (control)
                {
                    counter++;
                    var next = stance switch
                    {
                        Stance.HIT => counter > 200,
                        Stance.JUMP => physicsObject.onGround,
                        Stance.MOVE or Stance.STAND or Stance.DIE => animationEnd && counter > 200,
                        _ => animationEnd && counter > 200,
                    };
                    if (next)
                    {
                        NextMove();
                        UpdateMovement();
                        counter = 0;
                    }
                }
            }
            else
            {
                physicsObject.Normalize();
                physics?.GetFootholdTree()?.UpdateFootHold(physicsObject);
            }
        }
    }
}