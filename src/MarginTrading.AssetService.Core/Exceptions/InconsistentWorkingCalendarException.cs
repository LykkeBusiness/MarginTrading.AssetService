using System;

namespace MarginTrading.AssetService.Core.Exceptions
{
    public class InconsistentWorkingCalendarException: Exception
    {
        public InconsistentWorkingCalendarException(string message) : base(message)
        {
            
        }
    }
}