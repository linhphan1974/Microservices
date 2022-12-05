using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookOnline.EventLogService.Entities
{
    public enum EventStatus
    {
        NotPublished = 0,
        InProgress = 1,
        Published = 2,
        PublishedFailed = 3

    }
}
