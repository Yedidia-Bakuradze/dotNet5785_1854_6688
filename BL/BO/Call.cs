
    using Helpers;

    namespace BO;

    public class Call
    {
        /// <summary>
        /// From DO.Call
        /// </summary>
        public int Id { get; init; }

        /// <summary>
        /// From DO.Call
        /// </summary>
        public CallType TypeOfCall { get; set; }

        /// <summary>
        /// From DO.Call
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// A fine full addrss in the following format:
        /// address, number, city
        /// Get: From DO.Call
        /// Set: Check in the logigcal layer if the address is a real addrees which means if there are legit logitude and lantitue values
        /// If the address is invalid / the user didn't give any valid values, then the address shall be null
        /// </summary>
        public string CallAddress { get; set; }

        /// <summary>
        /// Get: From DO.Call
        /// Set: Every time the address has been updated, the Latitude value would be updated as well
        /// NFD - Not For Displayment
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// Get: From DO.Call
        /// Set: Every time the address has been updated, the Longitude value would be updated as well
        /// NFD - Not For Displayment
        /// </summary>
        public double? Longitude{ get; set; }

        /// <summary>
        /// Get: From DO.Call
        /// Set: From DO.Call which is calculated by the system clock
        /// </summary>
        public DateTime CallStartTime { get; init; }

        /// <summary>
        /// Get: From DO.Call
        /// Set: Check with the logical layer (If the date is after the start time of the call)
        /// </summary>
        public DateTime? CallDeadLine { get; set; }

        /// <summary>
        /// Calculated by theses factors:
        /// Type of closing from the DO.Call
        /// CallDeadLine
        /// System Clock
        /// </summary>
        public CallStatus Status { get; set; }

        /// <summary>
        /// Every assingment which has been made due to this call, if none then null
        /// </summary>
        public List<BO.CallAssignInList>? MyAssignments { get; set; }

        public override string ToString() => this.ToStringProperty();
    }
