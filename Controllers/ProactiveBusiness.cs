using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

using Microsoft.Azure;  //  Namespace for CloudConfigurationManager
using Microsoft.WindowsAzure.Storage;  //  Namespace for CloudStorageAccount

using Microsoft.WindowsAzure.Storage.Table;  //  Namespace for Table storage types
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;

namespace TimeSolution.Controllers
{
	[Serializable]
	public class QueueMessage
	{
		public ResumptionCookie ResumptionCookie;
		public string MessageText;

	}

	[Serializable]
	public class ProactiveBusiness : IDialog<object>
	{
		public async Task StartAsync(IDialogContext context)
		{
			context.Wait(MessageReceivedAsync);

		}

		QueueMessage queueMessage;
		private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
		{
			var msg = await argument;

			ResumptionCookie resCookie = new ResumptionCookie(msg);
			queueMessage = new QueueMessage
			{
				ResumptionCookie = resCookie,
				MessageText = msg.Text
			};

			CloudStorageAccount storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

			//  Create the queue client.
			CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();

			//  Retrieve a reference to a container.
			CloudQueue queue = queueClient.GetQueueReference("myqueue");

			//  Create the queue if it doesn't already exist
			queue.CreateIfNotExists();

			CloudQueueMessage messageNew = new CloudQueueMessage(JsonConvert.SerializeObject(queueMessage));
			queue.AddMessage(messageNew);
		}
	}
}