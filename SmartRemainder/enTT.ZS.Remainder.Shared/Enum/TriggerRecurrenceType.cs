using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace enTT.ZS.Remainder.Shared.Enum
{
    /// <summary>
    /// Defines recurrence type.
    /// </summary>
    public enum TriggerRecurrenceType
    {
        AtASpecificTime,
        EveryNMinutes,
        EveryNHours,
        EveryDayAt,
        EveryNDaysAt,
        EveryWeekDay,
        EverySpecificWeekDayAt,
        EverySpecificDayEveryNMonthAt,
        EverySpecificSeqWeekDayEveryNMonthAt,
        EverySpecificDayOfMonthAt,
        EverySpecificSeqWeekDayOfMonthAt,

    }
}
