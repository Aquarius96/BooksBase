using System.Collections.Generic;

namespace BooksBase.Shared
{
    public class EmailSettings
    {
        public string QueueName { get; set; }        
        public List<TemplateSettings> Templates { get; set; }
    }    

    public class TemplateSettings
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
