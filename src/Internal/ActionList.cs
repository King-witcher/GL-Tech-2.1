using System;

namespace GLTech2
{
    internal class ActionList
    {
        Action[] actions;
        int actionCount;

        public ActionList(int initialMax)
        {
            actions = new Action[initialMax];
            actionCount = 0;
        }

        public void Invoke()
        {
            for (int i = 0; i < actionCount; i++)
                actions[i]();
        }

        public void Add(Action action)
        {
            if (actionCount == actions.Length)
                Array.Resize<Action>(ref actions, actionCount * 2);
            actions[actionCount++] = action;
        }
    }
}
