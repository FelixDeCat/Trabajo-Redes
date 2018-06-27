using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using UnityEngine.Networking;

// ¿como se usa?

// primero seguir los pasos 1,2,3,4,5 de esta clase

// luego desde afuera para usarlo hay que crear un PacketBase
// asignarle por constructor el "PacketIDs", por ejemplo.  new PacketBase(PacketIDs.BlaBlaBlaCommand)
// luego ir agregandole datos con .Add(blablabla)
// y NO OLVIDAR cerrarlo con .Send()

// [paso 1] Agregar el enum
public enum PacketIDs : short
{
    Instantiate_Players,
    ConnectToServer,
    Move_Command,
    Attack_Command,
    Select_Command,
    Old_School_Command,
    BasicMessage,
    allCanMove,

    Cmd_MyPosToServer,
    Rpc_PosForAll,

    Cmd_Shoot,
    Rpc_InstBullet,

    Cmd_BulletCollision,
    Rpc_BulletCollision,

    Cmd_ReciveWinner,
    Rpc_ReciveWinner,

    Count
}

public class ConfigPackets {

    static void print(object p) { Debug.Log(p.ToString()); }

    public Dictionary<PacketIDs, Action<PacketBase>> packetActions = new Dictionary<PacketIDs, Action<PacketBase>>();

    // [paso 2] Crear el Action 
    // [paso 5] rellenar el action con la funcion correspondiente
    public static Action<PacketBase> action_basicMessage_Command =  x => BasicMessageCommand(x.stringInfo[0]);
    public static Action<PacketBase> action_ConnectServer =         x => ConnectToServer((int)x.floatInfo[0]);
    public static Action<PacketBase> action_Instantiate =           x => InstantiateClients(x.stringInfo[0]);
    public static Action<PacketBase> action_AllCanMove =            x => AllCanMove();
    //to server
    public static Action<PacketBase> action_MyPosToServer =         x => Server_ReceivePositionForUpdate(x.stringInfo[0],x.vectorInfo[0]);
    public static Action<PacketBase> action_Shoot =                 x => Server_ReceiveShoot(x.stringInfo[0]);
    public static Action<PacketBase> action_BulletCollision =       x => Server_ReceiveBulletCollision(x.stringInfo[0],x.vectorInfo[0]);
    public static Action<PacketBase> action_S_ReciveWinner =        x => Server_ReceiveWinner(x.stringInfo[0]); 
    //to client
    public static Action<PacketBase> action_ServerSendMeAPosition = x => Client_ReceivePositionForUpdate(x.stringInfo[0],x.vectorInfo[0]);
    public static Action<PacketBase> action_instbullet =            x => Client_InstShoot(x.stringInfo[0]);
    public static Action<PacketBase> action_ReceiveBulletColl =     x => Client_ReceiveBulletCollision(x.stringInfo[0], x.vectorInfo[0]);
    public static Action<PacketBase> action_C_ReciveWinner =        x => Client_ReceiveWinner(x.stringInfo[0]);



    // [paso 3] Rellenar el Diccionario con el Enum y el Action
    public void Config_PacketActions()
    {
        //Relleno el diccionario con algo...
        //Esto se Ejecuta en el MultiplayerManager cuando el jugador selecciono "Server" o "Client"
        packetActions.Add(PacketIDs.BasicMessage,           action_basicMessage_Command);
        packetActions.Add(PacketIDs.ConnectToServer,        action_ConnectServer);
        packetActions.Add(PacketIDs.Instantiate_Players,    action_Instantiate);
        packetActions.Add(PacketIDs.allCanMove,             action_AllCanMove);

        packetActions.Add(PacketIDs.Cmd_MyPosToServer,      action_MyPosToServer);
        packetActions.Add(PacketIDs.Rpc_PosForAll,          action_ServerSendMeAPosition);

        packetActions.Add(PacketIDs.Cmd_Shoot,              action_Shoot);
        packetActions.Add(PacketIDs.Rpc_InstBullet,         action_instbullet);

        packetActions.Add(PacketIDs.Cmd_BulletCollision,    action_BulletCollision);
        packetActions.Add(PacketIDs.Rpc_BulletCollision,    action_ReceiveBulletColl);

        packetActions.Add(PacketIDs.Cmd_ReciveWinner,       action_S_ReciveWinner);
        packetActions.Add(PacketIDs.Rpc_ReciveWinner,       action_C_ReciveWinner);

    }

    // [paso 4] Crear la funcion a la cual le asignamos al Action

    ///////////////////////////////////////////////////////////////////////////////////
    /// TODAS LAS FUNCIONES QUE VAN A EJECUTAR LOS ACTIONS CUANDO RECIBO UN PACKET
    ///////////////////////////////////////////////////////////////////////////////////
    public static void InstantiateClients(string players)
    {
        PlayerSpawner.instancia.FindAndDestroyGameObjects();

        var _players = players.Split('-');

        Console.WriteLine("Voy a instanciar: " + _players.Length + " Players");

        for (int i = 0; i < _players.Length; i++) {
            var id = int.Parse(_players[i].Split(',')[0]);
            var index = int.Parse(_players[i].Split(',')[1]);
            PlayerSpawner.instancia.SpawnPlayer(id, index);
        }
    }

    public static void AllCanMove()
    {
        Console.WriteLine("All Can Move");
        MultiplayerManager.instance.players.ForEach(x => x.Receive_CanMove());
    }

    public static void ConnectToServer(int _id)
    {
        Console.WriteLine("On Packet Received");
        GameObject.FindObjectOfType<MultiplayerManager>().PlayerConnected(_id);
    }
    public static void BasicMessageCommand(string msg)
    {
        Console.WriteLine(msg);
        print("recibo un mensaje " );
    }

    //FUNCIONES QUE RECIBE EL SERVER

    //Aca recibe la posicion de alguien y luego la manda a todos
    public static void Server_ReceivePositionForUpdate(string id,Vector3 pos) {
        new PacketBase(PacketIDs.Rpc_PosForAll).Add(id).Add(pos).Send(false);
    }
    public static void Server_ReceiveShoot(string bulletInfo) {
        Console.WriteLine("Server: Recibí un Shoot");
        new PacketBase(PacketIDs.Rpc_InstBullet).Add(bulletInfo).Send(false);
    }
    public static void Server_ReceiveBulletCollision(string id, Vector3 dir)
    {
        new PacketBase(PacketIDs.Rpc_BulletCollision).Add(id).Add(dir).Send();
    }
    public static void Server_ReceiveWinner(string id)
    {
        new PacketBase(PacketIDs.Rpc_ReciveWinner).Add(id).Send();
    }

    //FUNCIONES QUE RECIBE EL CLIENTE

    //Aca el servidor me manda la posicion de alguien para que la actualice en mi pantalla
    public static void Client_ReceivePositionForUpdate(string id, Vector3 pos)
    {
        MultiplayerManager.instance.players.Where(x => x.ID == int.Parse(id)).First().UpdateMyPositionFromServer(pos);
        
    }
    public static void Client_InstShoot(string bulletInfo)
    {
        string[] info = bulletInfo.Split('_');
        Console.WriteLine("Info " + bulletInfo);
        string id = info[0];
        Console.WriteLine("ID " + id);
        Vector3 pos = new Vector3(
            float.Parse(info[1].Split(',')[0]), 
            float.Parse(info[1].Split(',')[1]), 
            float.Parse(info[1].Split(',')[2]));
        Console.WriteLine("POS " + pos);
        Quaternion rot = new Quaternion(float.Parse(info[2].Split(',')[0]), float.Parse(info[2].Split(',')[1]), float.Parse(info[2].Split(',')[2]), float.Parse(info[2].Split(',')[3]));
        Console.WriteLine("Rot " + rot);
        Console.WriteLine("Client<" + id +">" + " Pos: " + pos + " Rot: " + rot);
        GameManager.instancia.TheAutority.InstanciateBullet(pos, rot);

    }
    public static void Client_ReceiveBulletCollision(string id, Vector3 dir)
    {
        var col = MultiplayerManager.instance.players.Where(x => x.ID == int.Parse(id)).ToList();
        if (col.Count > 0) col.First().RecibirEmpuje(dir);
    }
    public static void Client_ReceiveWinner(string s)
    {
        GameManager.instancia.anim_mensaje.Animar("El Ganador es \n" + s);
    }

}
