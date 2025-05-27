namespace amogus
{
    public abstract class DoorTrigger : TimelinePlayerTrigger
    {
        public string unlockCode;
        public bool isLocked = false;
        public abstract void Unlock();

    }
}
