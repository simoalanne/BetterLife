using System;

namespace UI
{
    public class GoToSleepPrompt : HUDAttachablePanel
    {
        public Action OnGoToSleepAccepted;

        public void OnClickNo() => Close();
        public void OnClickYes() => OnGoToSleepAccepted?.Invoke();
    }
}
