using Microsoft.Azure.EventGrid.Models;
using Newtonsoft.Json;
using System;

namespace Web.AzureFunctions.Framework
{
    public class EventHandlerBase : FunctionBase
    {
        protected TPayload GetPayload<TPayload>(EventGridEvent eventGridEvent)
        {
            if (eventGridEvent.Data == null || string.IsNullOrEmpty(eventGridEvent.Data.ToString()))
            {
                throw new InvalidOperationException("EventGridEvent.Data property cannot be null or empty");
            }

            return JsonConvert.DeserializeObject<TPayload>(eventGridEvent.Data.ToString());
        }
    }
}
