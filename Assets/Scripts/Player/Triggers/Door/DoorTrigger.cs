namespace amogus
{
    public abstract class DoorTrigger : CutsceneTrigger<PlayerFSM, Door>
    {
        public string unlockCode;
        public bool isLocked = false;
        public abstract void Unlock();

    }
}
