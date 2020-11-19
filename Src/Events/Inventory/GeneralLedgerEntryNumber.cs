using System;
using System.Linq;

namespace Events.Inventory
{
    public class GeneralLedgerEntryNumber
    {
        public const int MaxPrefixLength = 25;
        public string Prefix { get; }
        public int SequenceNumber { get; }

        public GeneralLedgerEntryNumber(string prefix, int sequenceNumber)
        {
            if (prefix == string.Empty)
            {
                throw new ArgumentException("Prefix may not be empty.", nameof(prefix));
            }

            if (prefix.Length > MaxPrefixLength)
            {
                throw new ArgumentException($"Prefix may not exceed {MaxPrefixLength} characters.", nameof(prefix));
            }

            if (prefix.Any(char.IsWhiteSpace))
            {
                throw new ArgumentException("Prefix may not contain whitespace.", nameof(prefix));
            }

            if (sequenceNumber <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(sequenceNumber));
            }

            Prefix = prefix;
            SequenceNumber = sequenceNumber;
        }
    }
}