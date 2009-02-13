namespace AnjLab.FX.Sys
{
    public class Command: ICommand
    {
        private readonly VoidAction _action;

        public Command(VoidAction action)
        {
            Guard.ArgumentNotNull("action", action);
            _action = action;
        }

        public void Execute()
        {
            _action();
        }

        public static ICommand FromAction(VoidAction action)
        {
            Guard.ArgumentNotNull("action", action);

            return new Command(action);
        }
    }
}
