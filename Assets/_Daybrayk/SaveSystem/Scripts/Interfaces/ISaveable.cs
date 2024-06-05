namespace Daybrayk.PersistentData
{
	public interface ISaveable
    {
        public object GetState();

        public void SetState(object o);
    }
}