using NetworkApi.NetworkManagement;

namespace SignalRCommunicator.Proto
{
	public class ClientDeviceInfoDTO
	{
		public ClientNetworkInfoDTO? Network { get; set; }
		public ClientComputerInfoDTO? Computer { get; set; }
		public ClientInfoDTO? Client { get; set; }

		public override bool Equals(object? obj)
		{
			var item = obj as ClientDeviceInfoDTO;
			var selfIsNull = this == null ? 1 : 0;
			var itemIsNull = item == null ? 1 : 0;
			if (selfIsNull + itemIsNull == 2) return true;
			if (selfIsNull + itemIsNull == 1) return false;
			return (Network?.Equals(item?.Network) ?? false) && (Computer?.Equals(item?.Computer) ?? false);
		}

		public override int GetHashCode() => base.GetHashCode();
	}

	public class ClientInfoDTO
	{
		/// <summary>
		/// 上次在线时间
		/// </summary>
		public long LastOnline { get; set; }
	}

	public class ClientComputerInfoDTO
	{
		public string? MachineName { get; set; }
		public string? UserName { get; set; }
		public string? OsVersion { get; set; }
		public string? Version { get; set; }
		public long TicketCount { get; set; }

		public override bool Equals(object? obj)
		{
			var item = obj as ClientComputerInfoDTO;
			if (this == null && item == null) return true;
			if (this == null || item == null) return false;
			return MachineName == item.MachineName && UserName == item.UserName && OsVersion == item.OsVersion && Version == item.Version;
		}

		public override int GetHashCode() => base.GetHashCode();
	}

	public class ClientNetworkInfoDTO
	{
		public IEnumerable<NetworkInterfaceDTO?>? Interfaces { get; set; }

		/// <summary>
		/// 客户端主用
		/// </summary>
		public NetworkInterfaceDTO? ActiveInterface { get; set; }

		public override bool Equals(object? obj)
		{
			// may cause performance problem
			return System.Text.Json.JsonSerializer.Serialize(obj) == System.Text.Json.JsonSerializer.Serialize(this);
		}

		public override int GetHashCode() => base.GetHashCode();
	}
}