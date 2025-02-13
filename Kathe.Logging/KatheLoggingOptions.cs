namespace Kathe
{
    public class KatheLoggingOptions
    {
        /// <summary>
        /// Should incoming requests be logged?
        /// Setting this flag to false short circuits the
        /// ShouldLogIncomingRequests delegate
        /// </summary>
        public bool LogIncomingRequests { get; set; }

        /// <summary>
        /// After how many seconds request handling will be logged as a warning
        /// </summary>
        public int RequestSecondsDurationThreshold { get; set; } = 3;
        
        /// <summary>
        /// Should outgoing requests be logged?
        /// Setting this flag to false short circuits the
        /// ShouldLogOutgoingRequest delegate
        /// </summary>
        public bool LogOutgoingRequests { get; set; }

        /// <summary>
        /// Should unhandled exceptions be caught and logged.
        /// Setting this to false causes exceptions not to be logged. 
        /// </summary>
        public bool LogUnhandledExceptions { get; set; } = true;

        /// <summary>
        /// Turns all flags on (default setting). 
        /// </summary>
        /// <returns></returns>
        public KatheLoggingOptions All()
        {
            LogIncomingRequests = true;
            LogOutgoingRequests = true;
            LogUnhandledExceptions = true;
            return this;
        }

        /// <summary>
        /// Turns all flags off
        /// </summary>
        /// <returns></returns>
        public KatheLoggingOptions None()
        {
            LogIncomingRequests = false;
            LogOutgoingRequests = false;
            LogUnhandledExceptions = false;
            return this;
        }
    }
}