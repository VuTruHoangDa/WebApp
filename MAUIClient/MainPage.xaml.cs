using Microsoft.AspNetCore.SignalR.Client;


namespace MAUIClient
{
	public partial class MainPage : ContentPage
	{
		private HubConnection connection;
		public MainPage()
		{
			InitializeComponent();

			connection = new HubConnectionBuilder()
				.WithUrl("http://192.168.1.250/ChatHub") // Server running on Linux Armbian Box
				.Build();

			connection.Closed += async error =>
			{
				entry.IsEnabled = false;
				await Task.Delay(new Random().Next(0, 5) * 1000);
				await connection.StartAsync();
				if (connection.State == HubConnectionState.Connected) entry.IsEnabled = true;
			};

			connection.On<string>("ReceiveMessage", message =>
			{
				Dispatcher.Dispatch(() => label.Text = message);
			});


			entry.IsEnabled = false;
			Connect();


			async void Connect()
			{
				await Task.Yield();
				await connection.StartAsync();
				if (connection.State == HubConnectionState.Connected) entry.IsEnabled = true;
			}
		}


		private async void entry_Completed(object sender, EventArgs e)
		{
			await connection.InvokeAsync("SendMessage", entry.Text);
		}
	}
}
