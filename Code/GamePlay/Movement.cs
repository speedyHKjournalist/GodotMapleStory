
namespace MapleStory
{
    public struct Movement
    {
        public enum Type
        {
            NONE,
            ABSOLUTE,
            RELATIVE,
            CHAIR,
            JUMPDOWN
        }
        public Type type;
        public int command;
        public int xPosition;
        public int yPosition;
        public int lastX;
        public int lastY;
        public int footHold;
        public int newState;
        public int duration;

        public Movement()
        {
            type = Type.NONE;
            command = 0;
            xPosition = 0;
            yPosition = 0;
            lastX = 0;
            lastY = 0;
            footHold = 0;
            newState = 0;
            duration = 0;
        }

        public Movement(PhysicsObject physicsObject, int newState)
        {
            type = Type.ABSOLUTE;
            command = 0;
            xPosition = physicsObject.GetX();
            yPosition = physicsObject.GetY();
            lastX = physicsObject.GetLastX();
            lastY = physicsObject.GetLastY();
            footHold = physicsObject.footHoldId;
            this.newState = newState;
            duration = 1;
        }

        public bool HasMoved(Movement newMove)
        {
            return newMove.newState != newState
                || newMove.xPosition != xPosition
                || newMove.yPosition != yPosition
                || newMove.lastX != lastX
                || newMove.lastY != lastY;
        }
    }
}