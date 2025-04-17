using nanoFramework.Networking;
using System.Threading;


namespace Esp32Client
{
	public class Program
	{
		public static void Main()
		{
			Util.Init();
			Util.lcd.Clear();
			Util.lcd.Write("Touch 0 to Connect Wifi \"VIETTEL 2.4G\"");

			bool connecting = false;
			Util.touchEvent += TouchEvent;
			Thread.Sleep(Timeout.Infinite);


			void TouchEvent(int padNum, bool isTouched)
			{
				if (connecting || padNum != 0 || !isTouched) return;

				Util.lcd.Clear();
				Util.lcd.Write("Connecting...");
				connecting = true;
				if (WifiNetworkHelper.ConnectDhcp("VIETTEL 2.4G", "12345678@"))
				{
					Util.lcd.Clear();
					Util.lcd.Write("wifi OK!");
					Util.touchEvent -= TouchEvent;
				}


				connecting = false;
			}
		}
	}
}
