namespace BooksBase.Shared
{
    public class EmailMessageData
    {
        public string TemplateId { get; set; }
        public dynamic Data { get; set; }
        public RecipientData To { get; set; }
    }

    public class RecipientData
    {
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
