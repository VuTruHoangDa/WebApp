using Iot.Device.CharacterLcd;
using nanoFramework.Hardware.Esp32.Touch;
using System.Device.I2c;


namespace Esp32Client
{
	/// <summary>
	/// LCD, Touch <para/>
	/// Nhớ dùng <c>Thread.Sleep(Timeout.Infinite)</c> ở cuối Main()<para/>
	/// .NET Nano chưa hỗ trợ: C# Generic, Static Constructor
	/// </summary>
	public static class Util
	{
		public static bool IsTouched(int padNum) => touchpads[padNum].Read() < touchpads[padNum].Threshold;


		public delegate void TouchDelegate(int padNum, bool isTouched);
		public static event TouchDelegate touchEvent;


		private static readonly I2cDevice i2c = I2cDevice.Create(new(2, 0x27));
		private static readonly LcdInterface lcdInterface = LcdInterface.CreateI2c(i2c, false);
		public static readonly Lcd2004 lcd = new(lcdInterface)
		{
			UnderlineCursorVisible = false
		};
		private static readonly TouchPad[] touchpads = new TouchPad[10];


		public static void Init()
		{
			TouchPad.SetVoltage(TouchHighVoltage.Volt2V7, TouchLowVoltage.Volt0V5, TouchHighVoltageAttenuation.Volt1V0);
			TouchPad.MeasurementMode = MeasurementMode.Timer;

			lcd.Clear();
			lcd.Write("Calibrating Touch...");
			for (int padNum = 0; padNum < 10; ++padNum)
			{
				var touchpad = touchpads[padNum] = new(padNum);
				touchpad.ValueChanged += (s, e) => touchEvent?.Invoke(e.PadNumber, !e.Touched);
				touchpad.GetCalibrationData();
				touchpad.Threshold = (uint)(2 / 3f * touchpad.CalibrationData);
			}

			lcd.Clear();
			lcd.Write("OK");
		}


		public static void Dispose()
		{
			lcd.Dispose();
			lcdInterface.Dispose();
		}
	}
}