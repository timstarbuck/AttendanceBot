using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace AttendanceBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            // calculate something for us to return
            //int length = (activity.Text ?? string.Empty).Length;

            // return our reply to the user
            //await context.PostAsync($"You sent {activity.Text} which was {length} characters");

            if (activity.Text.Contains("arrive"))
            {
                await context.PostAsync($"You asked about arriving?");
            }
            if (activity.Text.Contains("in school") || activity.Text.Contains("in class"))
            {
                await context.PostAsync($"You asked if your child was in school or in class?");

                //if (thisStudent.Arrived != "")
                //{
                //    reply.Text = thisStudent.Name + " arrived at school at " + thisStudent.Arrived + ".";
                //}
                //else
                //{
                //    reply.Text = thisStudent.Name + " did not arrive at school today.";
                //}
            }
            if (activity.Text.Contains("arrive") || activity.Text.Contains("present"))
            {
                await context.PostAsync($"You asked if your child arrived at school?");
                //if (thisStudent.Arrived != "")
                //{
                //    reply.Text = thisStudent.Name + " arrived at school at " + thisStudent.Arrived + ".";
                //}
                //else
                //{
                //    reply.Text = thisStudent.Name + " did not arrive at school today.";
                //}
            }
            if (activity.Text.Contains("leave") || activity.Text.Contains("home"))
            {
                await context.PostAsync($"You asked when your child left school?");
                //if (thisStudent.Arrived != "")
                //{
                //    if (thisStudent.Departed != "")
                //    {
                //        reply.Text = thisStudent.Name + " left school at " + thisStudent.Departed + ".";
                //    }
                //    else
                //    {
                //        reply.Text = thisStudent.Name + " has not left school today.";
                //    }
                //}
                //else
                //{
                //    reply.Text = thisStudent.Name + " did not arrive at school today.";
                //}
            }
            if (activity.Text.Contains("bus"))
            {
                await context.PostAsync($"You asked if your child took the bus home?");
                //if (thisStudent.Bus != "")
                //{
                //    reply.Text = thisStudent.Name + " took the bus today.";
                //}
                //else
                //{
                //    reply.Text = thisStudent.Name + " did not take the bus today.";
                //}
            }


            context.Wait(MessageReceivedAsync);
        }
    }
}