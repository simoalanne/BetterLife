namespace Player
{
    public class StoryProperties
    {
        public bool HasTalkedToLoanShark { get; set; }
        public bool HasTalkedToShopkeeper { get; set; }
        public bool HasReadGoodbyeNote { get; set; }

        public void ResetProperties()
        {
            HasTalkedToLoanShark = false;
            HasTalkedToShopkeeper = false;
            HasReadGoodbyeNote = false;
        }
    }
}
