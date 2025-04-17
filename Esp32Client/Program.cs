using nanoFramework.Networking;
using nanoFramework.SignalR.Client;
using System.Threading;


namespace Esp32Client
{
	public class Program
	{
		public static void Main()
		{
			// Connect Wifi
			Util.Init();
			Util.lcd.Clear();
			Util.lcd.Write("Connecting wifi...");
			// Replace ssid and password yourself
			if (WifiNetworkHelper.ConnectDhcp("VIETTEL 2.4G", "12345678@"))
			{
				Util.lcd.Clear();
				Util.lcd.Write("Wifi OK !");
			}

			var connection = new HubConnection("http://192.168.1.250/ChatHub");

			// Register Receive
			connection.On("ReceiveMessage", new[] { typeof(string) }, (sender, args) =>
			{
				Util.lcd.Clear();
				Util.lcd.Write($"Receive {args[0]}");
			});

			// Register Send
			AsyncResult result = null;
			Util.touchEvent += (padNum, isTouched) =>
			{
				if (!isTouched || (result != null && !result.Completed)) return;

				Util.lcd.Clear();
				Util.lcd.Write($"Sending {padNum}");
				result = connection.InvokeCoreAsync("SendMessage", null, new[] { padNum.ToString() });
			};

			// Connect Server
			Util.lcd.Clear();
			Util.lcd.Write("Connecting Server...");
			while (true)
			{
				switch (connection.State)
				{
					case HubConnectionState.Connected: goto CONNECTED;
					case HubConnectionState.Disconnected: connection.Start(); break;
					default: Thread.Sleep(3000); break;
				}
			}

		CONNECTED:
			Util.lcd.Clear();
			Util.lcd.Write("Server OK !");
			Thread.Sleep(Timeout.Infinite);
		}
	}
}
