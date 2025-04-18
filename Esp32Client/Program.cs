using nanoFramework.Networking;
using nanoFramework.SignalR.Client;
using System.Threading;


namespace Esp32Client
{
	public class Program
	{
		public static void Main()
		{
			Util.Init();
			var lcd = Util.lcd;
			lcd.Clear();
			lcd.Write("Connecting wifi...");
			if (WifiNetworkHelper.ConnectDhcp("VIETTEL 2.4G", "12345678@"))
			{
				lcd.Clear();
				lcd.Write("Wifi OK !");
			}
			else
			{
				lcd.Clear();
				lcd.Write("Wifi failed !");
				throw new System.Exception();
			}

			var connection = new HubConnection("http://192.168.1.250/ChatHub", options: new() { Reconnect = true });
			AsyncResult result = null;
			connection.Closed += (sender, message) =>
			{
				result = null;
				lcd.Clear();
				lcd.Write("Connection Closed !");
				connection.Start();
			};

			connection.Reconnecting += (sender, message) =>
			{
				lcd.Clear();
				lcd.Write("Reconnecting...");
			};

			connection.Reconnected += (sender, message) =>
			{
				lcd.Clear();
				lcd.Write("Reconnected !");
			};

			connection.On("ReceiveMessage", new[] { typeof(string) }, (sender, args) =>
			{
				lcd.Clear();
				lcd.Write($"Receive {args[0]}");
			});

			Util.touchEvent += (padNum, touch) =>
			{
				if (!touch || connection.State != HubConnectionState.Connected
				|| (result != null && !result.Completed)) return;

				lcd.Clear();
				lcd.Write($"Sending {padNum}");
				result = connection.InvokeCoreAsync("SendMessage", null, new[] { padNum.ToString() });
			};

			lcd.Clear();
			lcd.Write("Connecting Server...");
			connection.Start();
			while (connection.State != HubConnectionState.Connected) Thread.Sleep(1000);
			lcd.Clear();
			lcd.Write("Connected Server !");
			Thread.Sleep(Timeout.Infinite);
		}
	}
}
