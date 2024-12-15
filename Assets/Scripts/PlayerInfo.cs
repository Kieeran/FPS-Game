using System;
using Unity.Collections;
using Unity.Netcode;

namespace PlayerInfoNameSpace
{
    [GenerateSerializationForGenericParameterAttribute(0)]
    [GenerateSerializationForTypeAttribute(typeof(PlayerInfoNameSpace.PlayerInfo))]
    public struct PlayerInfo : IEquatable<PlayerInfo>
    {
        public ulong ClientId;  // Unique client identifier
        public FixedString32Bytes Name;     // Player name
        public int Kills;       // Kill count
        public int Deaths;      // Death count

        public PlayerInfo(ulong clientId, FixedString32Bytes name, int kills, int deaths)
        {
            ClientId = clientId;
            Name = name;
            Kills = kills;
            Deaths = deaths;
        }

        // Required for IEquatable<T>
        public bool Equals(PlayerInfo other)
        {
            return ClientId == other.ClientId &&
                Name == other.Name &&
                Kills == other.Kills &&
                Deaths == other.Deaths;
        }

        public override bool Equals(object obj)
        {
            return obj is PlayerInfo other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(ClientId, Name, Kills, Deaths);
        }

        public static bool operator ==(PlayerInfo left, PlayerInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(PlayerInfo left, PlayerInfo right)
        {
            return !(left == right);
        }
    }

}
