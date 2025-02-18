namespace VisualPinball.Engine.Mpf.Unity.MediaController.Messages.Mode
{
    public readonly struct Mode
    {
        public readonly string Name;
        public readonly int Priority;

        public Mode(string name, int priority)
        {
            Name = name;
            Priority = priority;
        }
    }
}
