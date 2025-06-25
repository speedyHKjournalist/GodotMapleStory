using Godot;
using System;
using System.Collections.Generic;

namespace MapleStory
{
    public partial class CharLook : Node2D
    {
        private Nominal<Stance.Id> stance = new();
        private Nominal<int> stanceFrame = new();
        private int stanceElapsed;

        private Nominal<Expression.Id> expression = new();
        private Nominal<int> expressionFrame = new();
        private int expressionElapsed;

        private bool flip;
        private bool IsAnimationEnd = false;

        private Body? currentBody = null;
        private Hair? currentHair = null;
        private Face? currentFace = null;
        private CharEquips equips = new();

        private BodyDrawInfo drawInfo = new();
        private Dictionary<int, Hair> hairStyles = [];
        private Dictionary<int, Face> faceTypes = [];
        private Dictionary<int, Body> bodyTypes = [];

        private BodyAction? action = null;
        private string actionString = string.Empty;
        private int actionFrame;
        private ValueTuple<DrawArgument, Stance.Id, Expression.Id, int, int> drawArguments = new (new(), Stance.Id.NONE, Expression.Id.DEFAULT, 0, 0);

        private TimedBool expressionCoolDown = new();
        private TimedBool alerted = new();

        RandomNumberGenerator randomizer = new();

        public override void _Ready()
        {
        }

        public void Init(LookEntry entry)
        {
            Reset();

            SetBody(entry.skin);
            SetHair(entry.hairId);
            SetFace(entry.faceId);

            //if top is null, add default top
            if (!entry.equips.TryGetValue((int)EquipSlot.Id.TOP, out int top))
                AddEquipment(Clothing.TOP_DEFAULT_ID);

            // if bottom is null, add default bottom
            if (!entry.equips.TryGetValue((int)EquipSlot.Id.BOTTOM, out int bottom))
                AddEquipment(Clothing.BOTTOM_DEFAULT_ID);

            foreach (var equip in entry.equips)
                AddEquipment(equip.Value);
        }

        public void Reset()
        {
            flip = true;

            action = null;
            actionString = string.Empty;
            actionFrame = 0;

            SetStance(Stance.Id.STAND1);
            stanceFrame.Set(0);
            stanceElapsed = 0;

            SetExpression(Expression.Id.DEFAULT);
            expressionFrame.Set(0);
            expressionElapsed = 0;
        }

        public void SetBody(int skinId)
        {
            if (!bodyTypes.TryGetValue(skinId, out var existingBody) || existingBody == null)
            {
                currentBody = new Body(skinId, drawInfo);
                bodyTypes.Add(skinId, currentBody);
            }
            else
                currentBody = existingBody;
        }

        public void SetHair(int hairId)
        {
            if (!hairStyles.TryGetValue(hairId, out var existingHair) || existingHair == null)
            {
                currentHair = new Hair(hairId, drawInfo);
                hairStyles.Add(hairId, currentHair);
            }
            else
            {
                currentHair = existingHair;
            }
        }

        public void SetFace(int faceId)
        {
            if (!faceTypes.TryGetValue(faceId, out var existingFace) || existingFace == null)
            {
                currentFace = new Face(faceId);
                faceTypes.Add(faceId, currentFace);
            } 
            else
            {
                currentFace = existingFace;
            }
        }

        public int GetDelay(Stance.Id stance, int frame)
        {
            return drawInfo.GetDelay(stance, frame);
        }

        public int GetNextFrame(Stance.Id stance, int frame)
        {
            return drawInfo.NextFrame(stance, frame);
        }

        public void SetExpression(Expression.Id newExpression)
        {
            if (expression != newExpression && !expressionCoolDown.Value)
            {
                expression.Set(newExpression);
                expressionFrame.Set(0);

                expressionElapsed = 0;
                expressionCoolDown.SetFor(5000);
            }
        }

        public void SetAction(string actionString)
        {
            if (this.actionString == actionString || actionString == string.Empty)
                return;

            Stance.Id actionStance = Stance.StanceUtils.ByString(actionString);
            if (actionStance != Stance.Id.NONE)
                SetStance(actionStance);
            else
            {
                action = drawInfo.GetAction(actionString, 0);

                if (action != null)
                {
                    actionFrame = 0;
                    stanceElapsed = 0;
                    this.actionString = actionString;

                    stance.Set(action.GetStance());
                    stanceFrame.Set(action.GetFrame());
                }
            }
        }

        public void Attack(bool degenerate)
        {
            if (equips == null)
                return;

            int weaponId = equips.GetWeapon();
            if (weaponId <= 0)
                return;

            WeaponData weapon = WeaponData.Get(weaponId);

            int attackType = weapon.GetAttack();

            if (attackType == 9 && !degenerate)
            {
                stance.Set(Stance.Id.SHOT);
                SetAction("handgun");
            }
            else
            {
                stance.Set(GetAttackStance(attackType, degenerate));
                stanceFrame.Set(0);
                stanceElapsed = 0;
            }

            /*        weapon.get_usesound(degenerate).play();*/
        }

        public void Attack(Stance.Id newStance)
        {
            if (action == null || newStance == Stance.Id.NONE)
                return;

            switch (newStance)
            {
                case Stance.Id.SHOT:
                    SetAction("handgun");
                    break;
                default:
                    SetStance(newStance);
                    break;
            }
        }
        public void SetStance(Stance.Id newStance)
        {
            if (action != null || newStance == Stance.Id.NONE)
                return;

            Stance.Id adjustedStance = equips.AdjustStance(newStance);

            if (stance != adjustedStance)
            {
                stance.Set(adjustedStance);
                stanceFrame.Set(0);
                stanceElapsed = 0;
            }
        }

        public void SetDirection(bool flipped)
        {
            flip = flipped;
        }

        public void SetAlerted(int milliSecond)
        {
            alerted.SetFor(milliSecond);
        }

        public bool GetAlerted()
        {
            return alerted.Value;
        }

        public bool IsTwoHanded(Stance.Id stance)
        {
            switch (stance)
            {
                case Stance.Id.STAND1:
                case Stance.Id.WALK1:
                    return false;
                case Stance.Id.STAND2:
                case Stance.Id.WALK2:
                    return true;
                default:
                    return equips.IsTwoHanded();
            }
        }
        public int GetAttackDelay(int no, int firstFrame)
        {
            if (action != null)
            {
                return drawInfo.GetAttackDelay(actionString, no);
            }
            else
            {
                int delay = 0;
                for (int frame = 0; frame < firstFrame; frame++)
                    delay += GetDelay(stance.Get(), frame);

                return delay;
            }
        }

        public void UpdateTwoHanded()
        {
            Stance.Id baseStance = Stance.StanceUtils.BaseOf(stance.Get());
            SetStance(baseStance);
        }

        public void AddEquipment(int itemId)
        {
            equips.AddEquip(itemId, drawInfo);
            UpdateTwoHanded();
        }

        public CharEquips GetEquips()
        {
            return equips;
        }

        public int GetFrame()
        {
            return stanceFrame.Get();
        }

        public Stance.Id GetStance()
        {
            return stance.Get();
        }

        enum AttackType : int
        {
            NONE = 0,
            S1A1M1D = 1,
            SPEAR = 2,
            BOW = 3,
            CROSSBOW = 4,
            S2A2M2 = 5,
            WAND = 6,
            CLAW = 7,
            GUN = 9,
            NUM_ATTACKS
        };

        public Stance.Id GetAttackStance(int attack, bool degenerate)
        {
            if (stance == Stance.Id.PRONE)
                return Stance.Id.PRONESTAB;

            List<Stance.Id>[] degenStances =
            [
                [Stance.Id.NONE],
                [Stance.Id.NONE],
                [Stance.Id.NONE],
                [Stance.Id.SWINGT1, Stance.Id.SWINGT3],
                [Stance.Id.SWINGT1, Stance.Id.STABT1],
                [Stance.Id.NONE],
                [Stance.Id.NONE],
                [Stance.Id.SWINGT1, Stance.Id.STABT1],
                [Stance.Id.NONE],
                [Stance.Id.SWINGP1, Stance.Id.STABT2]
            ];

            List<Stance.Id>[] attackStances =
            [
                [Stance.Id.NONE],
                [Stance.Id.STABO1, Stance.Id.STABO2, Stance.Id.SWINGO1, Stance.Id.SWINGO2, Stance.Id.SWINGO3],
                [Stance.Id.STABT1, Stance.Id.SWINGP1],
                [Stance.Id.SHOOT1],
                [Stance.Id.SHOOT2],
                [Stance.Id.STABO1, Stance.Id.STABO2, Stance.Id.SWINGT1, Stance.Id.SWINGT2, Stance.Id.SWINGT3],
                [Stance.Id.SWINGO1, Stance.Id.SWINGO2],
                [Stance.Id.SHOT],
                [Stance.Id.SWINGO1, Stance.Id.SWINGO2],
                [Stance.Id.NONE],
            ];

            if (attack <= (int)AttackType.NONE || attack >= (int)AttackType.NUM_ATTACKS)
                return Stance.Id.STAND1;

            var stances = degenerate ? degenStances[attack] : attackStances[attack];

            if (stances.Count == 0)
                return Stance.Id.STAND1;

            int index = randomizer.RandiRange(0, stances.Count);
            return stances[index];
        }

        public void Interpolate(DrawArgument args)
        {
            if (currentBody == null || currentHair == null || currentFace == null)
                return;

            MaplePoint<int> actionMove = new();

            if (action != null)
                actionMove = action.GetMove();

            DrawArgument relargs = new DrawArgument(actionMove, flip);
            float alpha = (float)Engine.GetPhysicsInterpolationFraction();
            Stance.Id interStance = stance.Get(alpha);
            Expression.Id interExpression = expression.Get(alpha);
            int interFrame = stanceFrame.Get(alpha);
            int interExpressionFrame = expressionFrame.Get(alpha);

            switch (interStance)
            {
                case Stance.Id.STAND1:
                case Stance.Id.STAND2:
                    {
                        if (alerted.Value)
                            interStance = Stance.Id.ALERT;
                        break;
                    }
            }

            drawArguments = new(relargs + args, interStance, interExpression, interFrame, interExpressionFrame);
            QueueRedraw();
        }

        public void Interpolate(MaplePoint<int> position, bool flipped, Stance.Id interStance, Expression.Id interExpression)
        {
            interStance = equips.AdjustStance(interStance);
            drawArguments = new(new DrawArgument(position, flipped), interStance, interExpression, 0, 0);
            QueueRedraw();
        }

        private bool Update(int timeStep)
        {
            if (timeStep == 0)
            {
                stance.Normalize();
                stanceFrame.Normalize();
                expression.Normalize();
                expressionFrame.Normalize();
                return false;
            }

            uint physicTimeDelta = (uint)(GetPhysicsProcessDeltaTime() * 1000);
            alerted.Update(physicTimeDelta);
            expressionCoolDown.Update(physicTimeDelta);

            bool animationEnd = false;
            if (action == null)
            {
                int delay = GetDelay(stance.Get(), stanceFrame.Get());
                int delta = delay - stanceElapsed;

                if (timeStep >= delta)
                {
                    stanceElapsed = timeStep - delta;

                    int nextFrame = GetNextFrame(stance.Get(), stanceFrame.Get());
                    float threshold = (float)(delta) / (float)timeStep;
                    stanceFrame.Next(nextFrame, threshold);

                    if (stanceFrame == 0)
                        animationEnd = true;
                }
                else
                {
                    stance.Normalize();
                    stanceFrame.Normalize();

                    stanceElapsed += timeStep;
                }
            }
            else
            {
                int delay = action.GetDelay();
                int delta = delay - stanceElapsed;

                if (timeStep >= delta)
                {
                    stanceElapsed = timeStep - delta;
                    actionFrame = drawInfo.NextActionFrame(actionString, actionFrame);

                    if (actionFrame > 0)
                    {
                        action = drawInfo.GetAction(actionString, actionFrame);

                        if (action != null)
                        {
                            float threshold = (float)delta / (float)timeStep;
                            stance.Next(action.GetStance(), threshold);
                            stanceFrame.Next(action.GetFrame(), threshold);
                        }
                    }
                    else
                    {
                        animationEnd = true;
                        action = null;
                        actionString = string.Empty;
                        SetStance(Stance.Id.STAND1);
                    }
                }
                else
                {
                    stance.Normalize();
                    stanceFrame.Normalize();

                    stanceElapsed += timeStep;
                }
            }

            int expressionDelay = currentFace!.GetDelay(expression.Get(), expressionFrame.Get());
            int expressionDelta = expressionDelay - expressionElapsed;

            if (timeStep >= expressionDelta)
            {
                expressionElapsed = timeStep - expressionDelta;

                int nextExpressionFrame = currentFace!.NextFrame(expression.Get(), expressionFrame.Get());
                float faceThreshold = (float)expressionDelta / timeStep;
                expressionFrame.Next(nextExpressionFrame, faceThreshold);

                if (expressionFrame == 0)
                {
                    if (expression == Expression.Id.DEFAULT)
                        expression.Next(Expression.Id.BLINK, faceThreshold);
                    else
                        expression.Next(Expression.Id.DEFAULT, faceThreshold);
                }
            }
            else
            {
                expression.Normalize();
                expressionFrame.Normalize();

                expressionElapsed += timeStep;
            }

            return animationEnd;
        }

        public override void _Draw()
        {
            DrawArgument args = drawArguments.Item1;
            Stance.Id interStance = drawArguments.Item2;
            Expression.Id interExpression = drawArguments.Item3;
            int interFrame = drawArguments.Item4;
            int interExpFrame = drawArguments.Item5;

            MaplePoint<int> faceShift = drawInfo.GetFacePostionShift(interStance, interFrame);
            DrawArgument faceargs = args + new DrawArgument(faceShift, false, new MaplePoint<int>());

            if (Stance.StanceUtils.IsClimbing(interStance))
            {
                currentBody!.Render(this, Body.Layer.BODY, interStance, interFrame, args);
                equips.Render(this, EquipSlot.Id.GLOVES, interStance, Clothing.Layer.GLOVE, interFrame, args);
                equips.Render(this, EquipSlot.Id.SHOES, interStance, Clothing.Layer.SHOES, interFrame, args);
                equips.Render(this, EquipSlot.Id.BOTTOM, interStance, Clothing.Layer.PANTS, interFrame, args);
                equips.Render(this, EquipSlot.Id.TOP, interStance, Clothing.Layer.TOP, interFrame, args);
                equips.Render(this, EquipSlot.Id.TOP, interStance, Clothing.Layer.MAIL, interFrame, args);
                equips.Render(this, EquipSlot.Id.CAPE, interStance, Clothing.Layer.CAPE, interFrame, args);
                currentBody!.Render(this, Body.Layer.HEAD, interStance, interFrame, args);
                equips.Render(this, EquipSlot.Id.EARACC, interStance, Clothing.Layer.EARRINGS, interFrame, args);

                switch (equips.GetCapType())
                {
                    case CharEquips.CapType.NONE:
                        currentHair!.Render(this, Hair.Layer.BACK, interStance, interFrame, args);
                        break;
                    case CharEquips.CapType.HEADBAND:
                        equips.Render(this, EquipSlot.Id.HAT, interStance, Clothing.Layer.CAP, interFrame, args);
                        currentHair!.Render(this, Hair.Layer.BACK, interStance, interFrame, args);
                        break;
                    case CharEquips.CapType.HALFCOVER:
                        currentHair!.Render(this, Hair.Layer.BELOWCAP, interStance, interFrame, args);
                        equips.Render(this, EquipSlot.Id.HAT, interStance, Clothing.Layer.CAP, interFrame, args);
                        break;
                    case CharEquips.CapType.FULLCOVER:
                        equips.Render(this, EquipSlot.Id.HAT, interStance, Clothing.Layer.CAP, interFrame, args);
                        break;
                }

                equips.Render(this, EquipSlot.Id.SHIELD, interStance, Clothing.Layer.BACKSHIELD, interFrame, args);
                equips.Render(this, EquipSlot.Id.WEAPON, interStance, Clothing.Layer.BACKWEAPON, interFrame, args);
            }
            else
            {
                currentHair!.Render(this, Hair.Layer.BELOWBODY, interStance, interFrame, args);
                equips.Render(this, EquipSlot.Id.CAPE, interStance, Clothing.Layer.CAPE, interFrame, args);
                equips.Render(this, EquipSlot.Id.SHIELD, interStance, Clothing.Layer.SHIELD_BELOW_BODY, interFrame, args);
                equips.Render(this, EquipSlot.Id.WEAPON, interStance, Clothing.Layer.WEAPON_BELOW_BODY, interFrame, args);
                equips.Render(this, EquipSlot.Id.HAT, interStance, Clothing.Layer.CAP_BELOW_BODY, interFrame, args);
                currentBody!.Render(this, Body.Layer.BODY, interStance, interFrame, args);
                equips.Render(this, EquipSlot.Id.GLOVES, interStance, Clothing.Layer.WRIST_OVER_BODY, interFrame, args);
                equips.Render(this, EquipSlot.Id.GLOVES, interStance, Clothing.Layer.GLOVE_OVER_BODY, interFrame, args);
                equips.Render(this, EquipSlot.Id.SHOES, interStance, Clothing.Layer.SHOES, interFrame, args);
                currentBody!.Render(this, Body.Layer.ARM_BELOW_HEAD, interStance, interFrame, args);

                if (equips.HasOverAll())
                {
                    equips.Render(this, EquipSlot.Id.TOP, interStance, Clothing.Layer.MAIL, interFrame, args);
                }
                else
                {
                    equips.Render(this, EquipSlot.Id.BOTTOM, interStance, Clothing.Layer.PANTS, interFrame, args);
                    equips.Render(this, EquipSlot.Id.TOP, interStance, Clothing.Layer.TOP, interFrame, args);
                }

                currentBody!.Render(this, Body.Layer.ARM_BELOW_HEAD_OVER_MAIL, interStance, interFrame, args);
                equips.Render(this, EquipSlot.Id.SHIELD, interStance, Clothing.Layer.SHIELD_OVER_HAIR, interFrame, args);
                equips.Render(this, EquipSlot.Id.EARACC, interStance, Clothing.Layer.EARRINGS, interFrame, args);
                currentBody!.Render(this, Body.Layer.HEAD, interStance, interFrame, args);
                currentBody!.Render(this, Body.Layer.HUMAN_EAR, interStance, interFrame, args + new MaplePoint<int>(0, 1));
                currentHair!.Render(this, Hair.Layer.SHADE, interStance, interFrame, args);
                currentHair!.Render(this, Hair.Layer.DEFAULT, interStance, interFrame, args);
                currentFace!.Render(this, interExpression, interExpFrame, faceargs);
                equips.Render(this, EquipSlot.Id.FACE, interStance, Clothing.Layer.FACEACC, 0, faceargs);
                equips.Render(this, EquipSlot.Id.EYEACC, interStance, Clothing.Layer.EYEACC, interFrame, args);
                equips.Render(this, EquipSlot.Id.SHIELD, interStance, Clothing.Layer.SHIELD, interFrame, args);

                switch (equips.GetCapType())
                {
                    case CharEquips.CapType.NONE:
                        currentHair!.Render(this, Hair.Layer.OVERHEAD, interStance, interFrame, args);
                        break;
                    case CharEquips.CapType.HEADBAND:
                        equips.Render(this, EquipSlot.Id.HAT, interStance, Clothing.Layer.CAP, interFrame, args);
                        currentHair!.Render(this, Hair.Layer.DEFAULT, interStance, interFrame, args);
                        currentHair!.Render(this, Hair.Layer.OVERHEAD, interStance, interFrame, args);
                        equips.Render(this, EquipSlot.Id.HAT, interStance, Clothing.Layer.CAP_OVER_HAIR, interFrame, args);
                        break;
                    case CharEquips.CapType.HALFCOVER:
                        currentHair!.Render(this, Hair.Layer.DEFAULT, interStance, interFrame, args);
                        equips.Render(this, EquipSlot.Id.HAT, interStance, Clothing.Layer.CAP, interFrame, args);
                        break;
                    case CharEquips.CapType.FULLCOVER:
                        equips.Render(this, EquipSlot.Id.HAT, interStance, Clothing.Layer.CAP, interFrame, args);
                        break;
                }

                equips.Render(this, EquipSlot.Id.WEAPON, interStance, Clothing.Layer.WEAPON_BELOW_ARM, interFrame, args);

                if (IsTwoHanded(interStance))
                {
                    currentBody!.Render(this, Body.Layer.ARM, interStance, interFrame, args);
                    equips.Render(this, EquipSlot.Id.TOP, interStance, Clothing.Layer.MAILARM, interFrame, args);
                    equips.Render(this, EquipSlot.Id.WEAPON, interStance, Clothing.Layer.WEAPON, interFrame, args);
                }
                else
                {
                    equips.Render(this, EquipSlot.Id.WEAPON, interStance, Clothing.Layer.WEAPON, interFrame, args);
                    currentBody!.Render(this, Body.Layer.ARM, interStance, interFrame, args);
                    equips.Render(this, EquipSlot.Id.TOP, interStance, Clothing.Layer.MAILARM, interFrame, args);
                }

                equips.Render(this, EquipSlot.Id.GLOVES, interStance, Clothing.Layer.WRIST, interFrame, args);
                equips.Render(this, EquipSlot.Id.GLOVES, interStance, Clothing.Layer.GLOVE, interFrame, args);
                equips.Render(this, EquipSlot.Id.WEAPON, interStance, Clothing.Layer.WEAPON_OVER_GLOVE, interFrame, args);

                currentBody!.Render(this, Body.Layer.HAND_BELOW_WEAPON, interStance, interFrame, args);

                currentBody!.Render(this, Body.Layer.ARM_OVER_HAIR, interStance, interFrame, args);
                currentBody!.Render(this, Body.Layer.ARM_OVER_HAIR_BELOW_WEAPON, interStance, interFrame, args);
                equips.Render(this, EquipSlot.Id.WEAPON, interStance, Clothing.Layer.WEAPON_OVER_HAND, interFrame, args);
                equips.Render(this, EquipSlot.Id.WEAPON, interStance, Clothing.Layer.WEAPON_OVER_BODY, interFrame, args);
                currentBody!.Render(this, Body.Layer.HAND_OVER_HAIR, interStance, interFrame, args);
                currentBody!.Render(this, Body.Layer.HAND_OVER_WEAPON, interStance, interFrame, args);

                equips.Render(this, EquipSlot.Id.GLOVES, interStance, Clothing.Layer.WRIST_OVER_HAIR, interFrame, args);
                equips.Render(this, EquipSlot.Id.GLOVES, interStance, Clothing.Layer.GLOVE_OVER_HAIR, interFrame, args);
            }
        }

        public override void _PhysicsProcess(double delta)
        {
            /*            float speed = GetParent<Character>().GetStanceSpeed();*/
            float speed = 1.0f;
            IsAnimationEnd = Update((int)(speed * delta * 1000));
        }
    }
}