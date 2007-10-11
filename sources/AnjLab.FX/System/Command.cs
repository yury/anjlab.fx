namespace AnjLab.FX.System
{
    public class Command: ICommand
    {
        private readonly VoidAction _action;

        public Command(VoidAction action)
        {
            Guard.NotNull("action", action);
            _action = action;
        }

        public void Execute()
        {
            _action();
        }

        public static ICommand FromAction(VoidAction action)
        {
            return new Command(action);
        }
    }
}
