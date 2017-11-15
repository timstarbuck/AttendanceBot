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
        [LuisIntent("None")]
        public async Task NoIntentFound(IDialogContext context, LuisResult luisResult)
        {
            await context.PostAsync("No intent found. Maybe some more training is required?");
        }

        [LuisIntent("AttendanceIn")]
        public async Task ArrivalIntent(IDialogContext context, LuisResult luisResult)
        {
            bool foundRecord = false;
            var records = LocationRecordLogs.GetByPerson(luisResult.Entities[0].Entity, "group");
            foreach (var record in records)
            {
                if (record.Location == "School")
                {
                    foundRecord = true;
                    StringBuilder sb = new StringBuilder();
                    sb.Append("Yes, ");
                    sb.Append(luisResult.Entities[0].Entity);
                    sb.Append(" arrived at school at ");
                    sb.Append(record.Timestamp);
                    sb.Append(".");
                    await context.PostAsync(sb.ToString());
                }
                if (foundRecord == false)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(luisResult.Entities[0].Entity);
                    sb.Append(" is not at school.");
                    await context.PostAsync(sb.ToString());
                }
            }
        }

        [LuisIntent("AttendanceOut")]
        public async Task DepartIntent(IDialogContext context, LuisResult luisResult)
        {
            // Call API to get student's record
            // if arrived:
            //      if departed:
            //
            //      else
            //
            // else:


            StringBuilder sb = new StringBuilder();
            sb.Append(luisResult.Entities[0].Entity);
            sb.Append(" left school at ");
            sb.Append("<get from data>");
            sb.Append(".");
            await context.PostAsync(sb.ToString());
        }

        [LuisIntent("TravelType")]
        public async Task TravelTypeIntent(IDialogContext context, LuisResult luisResult)
        {
            // Call API to get student's record
            // if bus:

            // else:


            StringBuilder sb = new StringBuilder();
            sb.Append(luisResult.Entities[0].Entity);
            sb.Append(" took the bus today.");
            await context.PostAsync(sb.ToString());
        }

    }
}