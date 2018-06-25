using UnityEngine;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;
using System.Linq;

////////////////////////////////////////////////////////////////////////////////////////
/// MULTIPLAYER MANAGER ////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////

public class MultiplayerManager : MonoBehaviour {
    public static int connectionID;
    public ConnectionType connectionType;
    public static NetworkClient myClient;
    public static MultiplayerManager instance;
    public ConfigPackets packetConfigurer = new ConfigPackets();
    public enum ConnectionType { None, Server, Client, Both }
    private void Awake() { instance = this; }
    private void OnGUI()
    {
        if (!NetworkServer.active && !NetworkClient.active)
        {
            if (GUILayout.Button("Host Game")) SetupServer();
            if (GUILayout.Button("Start Client")) SetupClient();
            if (GUILayout.Button("Host & Launch Client")) SetupServer(true);
        }
        if (NetworkClient.active) { if (GUILayout.Button("Disconnect")) myClient.Disconnect(); }
    }
    public void SetupServer(bool launchClient = false) {
        connectionType = launchClient ? ConnectionType.Both : ConnectionType.Server;
        AddPacketActions();
        NetworkServer.Listen(8080);
        myClient = ClientScene.ConnectLocalServer();
    }
    public void SetupClient() {
        connectionType = ConnectionType.Client;
        myClient = new NetworkClient();
        AddPacketActions();
        myClient.Connect("127.0.0.1", 8080);

        Invoke("ConectClient", 0.1f);
    }

    void ConectClient() {
        Console.WriteLine("ConectClient");
        new PacketBase(PacketIDs.ConnectToServer).Add(connectionID.ToString()).Send();
    }

    public void PlayerConnected(string id)
    {
        Console.WriteLine("El player " + id + " se ha conectado");
        Debug.Log("El player " + id + " se ha conectado");
    }

    private void OnPlayerDisconnected(NetworkPlayer player)
    {
        Console.WriteLine("On Player Disconnected");
    }

    private void OnPlayerConnected(NetworkPlayer player)
    {
        Console.WriteLine("On Player Connected");
    }

    private void OnDisconnectedFromServer(NetworkDisconnection info)
    {
        Console.WriteLine("Disconnected fron Server");
    }

    private void OnConnectedToServer()
    {
        Debug.Log("Estoy Conectado");
    }

    void AddPacketActions()
    {
        if(connectionType == ConnectionType.Server)
            for (short i = 1000; i < 1000+(short)PacketIDs.Count; i++)
                NetworkServer.RegisterHandler(i, OnPacketReceived);
        else if (connectionType == ConnectionType.Client)
            for (short i = 1000; i < 1000+(short)PacketIDs.Count; i++)
                myClient.RegisterHandler(i, OnPacketReceived);
        else if (connectionType == ConnectionType.Both)
            for (short i = 1000; i < 1000+(short)PacketIDs.Count; i++)
            {
                myClient.RegisterHandler(i, OnPacketReceived);
                NetworkServer.RegisterHandler(i, OnPacketReceived);
            }

        packetConfigurer.Config_PacketActions();
    }
	void OnPacketReceived(NetworkMessage ms)
    {
        var msg = ms.ReadMessage<PacketBase>();
        msg.connectionID = ms.conn.connectionId;
        if (packetConfigurer.packetActions.ContainsKey((PacketIDs)msg.messageID))
            packetConfigurer.packetActions[(PacketIDs)msg.messageID](msg);
    }
}

////////////////////////////////////////////////////////////////////////////////////////
/// PACKET MANAGER /////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////

public static class PacketManager {
    public static PacketBase Add<T>(this PacketBase packet, T info)
    {
        if (info.GetType().Equals(typeof(string)))
            return packet.AddString((string)(object)info);
        else if (info.GetType().Equals(typeof(float)))
            return packet.AddFloat((float)(object)info);
        else if (info.GetType().Equals(typeof(Vector3)))
            return packet.AddVector((Vector3)(object)info);
        else if (info.GetType().Equals(typeof(NetworkInstanceId)))
            return packet.AddNetwork((NetworkInstanceId)(object)info);
        else if (info.GetType().Equals(typeof(bool)))
            return packet.AddBool((bool)(object)info);
        else return packet;
    }
    static PacketBase AddFloat(this PacketBase packet, float info)
    {
        var list = packet.floatInfo != null ? packet.floatInfo.ToList() : new List<float>();
        list.Add(info);
        packet.floatInfo = list.ToArray();
        return packet;
    }
    static PacketBase AddVector(this PacketBase packet, Vector3 info)
    {
        var list = packet.vectorInfo != null ? packet.vectorInfo.ToList() : new List<Vector3>();
        list.Add(info);
        packet.vectorInfo = list.ToArray();
        return packet;
    }
    static PacketBase AddNetwork(this PacketBase packet, NetworkInstanceId info)
    {
        var list = packet.networkInfo != null ? packet.networkInfo.ToList() : new List<NetworkInstanceId>();
        list.Add(info);
        packet.networkInfo = list.ToArray();
        return packet;
    }
    static PacketBase AddString(this PacketBase packet, string info)
    {
        var list = packet.stringInfo != null ? packet.stringInfo.ToList() : new List<string>();
        list.Add(info);
        packet.stringInfo = list.ToArray();
        return packet;
    }
    static PacketBase AddBool(this PacketBase packet, bool info)
    {
        var list = packet.boolInfo != null ? packet.boolInfo.ToList() : new List<bool>();
        list.Add(info);
        packet.boolInfo = list.ToArray();
        return packet;
    }
    public static void SendAsServer(this PacketBase packet, bool reliable = true, int connId = -1)
    {
        short packetID = 1000;
        packetID += packet.messageID;

        if (connId != -1)
            NetworkServer.SendToClient(connId, packetID, packet);
        else
            if (reliable)
            NetworkServer.SendToAll(packetID, packet);
        else
            NetworkServer.SendUnreliableToAll(packetID, packet);
    }
    public static void SendAsClient(this PacketBase packet, bool reliable = true)
    {
        short packetID = 1000;
        packetID += packet.messageID;

        if (reliable)
            MultiplayerManager.myClient.Send(packetID, packet);
        else
            MultiplayerManager.myClient.SendUnreliable(packetID, packet);
    }
    public static void Send(this PacketBase packet, bool reliable = true, int connId = -1)
    {
        short packetID = 1000;
        packetID += packet.messageID;

        if (MultiplayerManager.instance.connectionType == MultiplayerManager.ConnectionType.Server)
        {
            if (connId != -1)
                NetworkServer.SendToClient(connId, packetID, packet);
            else
                if (reliable)
                NetworkServer.SendToAll(packetID, packet);
            else
                NetworkServer.SendUnreliableToAll(packetID, packet);
        }
        else if (MultiplayerManager.instance.connectionType == MultiplayerManager.ConnectionType.Client)
            if (reliable)
                MultiplayerManager.myClient.Send(packetID, packet);
            else
                MultiplayerManager.myClient.SendUnreliable(packetID, packet);
    }
}

////////////////////////////////////////////////////////////////////////////////////////
/// PACKET BASE ////////////////////////////////////////////////////////////////////////
////////////////////////////////////////////////////////////////////////////////////////

public class PacketBase : MessageBase {
    public PacketBase(PacketIDs id) { messageID = (short)id; }
    public PacketBase() { }
    public short messageID;
    public int connectionID;
    public float[] floatInfo;
    public string[] stringInfo;
    public bool[] boolInfo;
    public NetworkInstanceId[] networkInfo;
    public Vector3[] vectorInfo;
}
