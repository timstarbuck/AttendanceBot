using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AttendanceBot.Dialogs
{
    //https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/70104130-ac92-4daf-afb5-9b6b092ce213?subscription-key=42282c8fe7fd4896b3f909472d4011f5&verbose=true&timezoneOffset=-6.0&q=

    [LuisModel("70104130-ac92-4daf-afb5-9b6b092ce213", "42282c8fe7fd4896b3f909472d4011f5")]
    [Serializable]
    public class LuisDialog : LuisDialog<object>
    {
        [LuisIntent("")]
        public async Task EmptyFound(IDialogContext context, LuisResult luisResult)
        {
            await context.PostAsync("I'm sorry. I don't understand what you are asking.");
        }

        [LuisIntent("None")]
        public async Task NoIntentFound(IDialogContext context, LuisResult luisResult)
        {
            await context.PostAsync("I'm sorry. I don't understand what you are asking.");
        }

        [LuisIntent("AttendanceIn")]
        public async Task ArrivalIntent(IDialogContext context, LuisResult luisResult)
        {
            await context.PostAsync("(Arrival question)");
            bool foundRecord = false;
            StringBuilder sb = new StringBuilder();
            var logs = new LocationRecordLogs();
            var records = await logs.GetByPerson(luisResult.Entities[0].Entity, "demo");
            foreach (var record in records)
            {
                if (record.Location == "School")
                {
                    foundRecord = true;
                    sb.Append("Yes, ");
                    sb.Append(luisResult.Entities[0].Entity);
                    sb.Append(" arrived at school at ");
                    sb.Append(record.Timestamp);
                    sb.Append(".");
                    await context.PostAsync(sb.ToString());
                }
                if (foundRecord == false)
                {
                    sb.Append(luisResult.Entities[0].Entity);
                    sb.Append(" is not at school.");
                    await context.PostAsync(sb.ToString());
                }
            }
        }

        [LuisIntent("AttendanceOut")]
        public async Task DepartIntent(IDialogContext context, LuisResult luisResult)
        {
            bool foundRecord = false;
            StringBuilder sb = new StringBuilder();
            var records = LocationRecordLogs.GetByPerson(luisResult.Entities[0].Entity, "demo");
            foreach (var record in records)
            {
                if (record.Location == "Home")
                {
                    foundRecord = true;
                    sb.Append(luisResult.Entities[0].Entity);
                    sb.Append(" left at school at ");
                    sb.Append(record.Timestamp);
                    sb.Append(".");
                    await context.PostAsync(sb.ToString());
                }
                if (foundRecord == false)
                {
                    sb.Append(luisResult.Entities[0].Entity);
                    sb.Append(" was not at school.");
                    await context.PostAsync(sb.ToString());
                }
            }
        }

        [LuisIntent("TravelType")]
        public async Task TravelTypeIntent(IDialogContext context, LuisResult luisResult)
        {
            bool foundRecord = false;
            StringBuilder sb = new StringBuilder();
            var records = LocationRecordLogs.GetByPerson(luisResult.Entities[0].Entity, "demo");
            foreach (var record in records)
            {
                if (record.Location == "Bus")
                {
                    foundRecord = true;
                    sb.Append(luisResult.Entities[0].Entity);
                    sb.Append(" took the bus home.");
                    await context.PostAsync(sb.ToString());
                }
                if (record.Location == "Car")
                {
                    foundRecord = true;
                    sb.Append(luisResult.Entities[0].Entity);
                    sb.Append(" left school by car.");
                    await context.PostAsync(sb.ToString());
                }
                if (record.Location == "Train")
                {
                    foundRecord = true;
                    sb.Append(luisResult.Entities[0].Entity);
                    sb.Append(" left school by train.");
                    await context.PostAsync(sb.ToString());
                }
                if (record.Location == "Walk")
                {
                    foundRecord = true;
                    sb.Append(luisResult.Entities[0].Entity);
                    sb.Append(" walked home.");
                    await context.PostAsync(sb.ToString());
                }
                if (foundRecord == false)
                {
                    sb.Append("I'm sorry, but I do not know how ");
                    sb.Append(luisResult.Entities[0].Entity);
                    sb.Append(" went home from school.");
                    await context.PostAsync(sb.ToString());
                }
            }
        }
    }
}