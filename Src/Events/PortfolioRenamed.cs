using System;

namespace Events
{
    public class PortfolioRenamed
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}