namespace Cashflow.Domain.DataTransferObjects
{
    public class InboxDto
    {
        public int InboxId { get; set; }
        public string Name { get; set; }

        public bool SeenAll { get; set; }
        public bool Active { get; set; }
    }
}
