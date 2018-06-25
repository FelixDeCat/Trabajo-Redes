using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

// ¿como se usa?

// primero seguir los pasos 1,2,3,4,5 de esta clases

// luego desde afuera para usarlo hay que crear un PacketBase
// asignarle por constructor el "PacketIDs", por ejemplo.  new PacketBase(PacketIDs.BlaBlaBlaCommand)
// luego ir agregandole datos con .Add(blablabla)
// y NO OLVIDAR cerrarlo con .Send()

// [paso 1] Agregar el enum
public enum PacketIDs : short { Move_Command, Attack_Command, Select_Command, Old_School_Command, lalala, Count }

public class ConfigPackets {

    static void print(object p) { Debug.Log(p.ToString()); }

    public Dictionary<PacketIDs, Action<PacketBase>> packetActions = new Dictionary<PacketIDs, Action<PacketBase>>();

    // [paso 2] Crear el Action 
    // [paso 5] rellenar el action con la funcion correspondiente
    public static Action<PacketBase> Move_Command = x => MoveCommand(x.stringInfo, x.vectorInfo[0]);
    public static Action<PacketBase> Attack_Command = x => AttackCommand(x.stringInfo, (int)x.floatInfo[0]);
    public static Action<PacketBase> Select_Command = x => SelectCommand(x.stringInfo);
    public static Action<PacketBase> Old_School_Command = x => Move(x.stringInfo[0]);

    // [paso 3] Rellenar el Diccionario con el Enum y el Action
    public void Config_PacketActions()
    {
        //Relleno el diccionario con algo...
        //Esto se Ejecuta en el MultiplayerManager cuando el jugador selecciono "Server" o "Client"
        packetActions.Add(PacketIDs.Move_Command, Move_Command);
        packetActions.Add(PacketIDs.Attack_Command, Attack_Command);
        packetActions.Add(PacketIDs.Select_Command, Select_Command);
        packetActions.Add(PacketIDs.Old_School_Command, Old_School_Command);
    }

    // [paso 4] Crear la funcion a la cual le asignamos al Action

    ///////////////////////////////////////////////////////////////////////////////////
    /// TODAS LAS FUNCIONES QUE VAN A EJECUTAR LOS ACTIONS CUANDO RECIBO UN PACKET
    ///////////////////////////////////////////////////////////////////////////////////
    public static void MoveCommand(string[] units, Vector3 pos)
    {
        string[] ids = units[0].Split(',');
        foreach (var item in ids)
            print("Moving unit " + item + " to pos " + pos);
    }
    public static void AttackCommand(string[] units, int target)
    {
        string[] ids = units[0].Split(',');
        foreach (var item in ids)
            print("Unit " + item + " attacking unit " + target);
    }
    public static void SelectCommand(string[] units)
    {
        string[] ids = units[0].Split(',');
        foreach (var item in ids)
            print("Selecting unit " + item);
    }
    public static void Move(string info)
    {
        string[] complete = info.Split('-');
        string[] units = complete[0].Split(',');
        string[] vector = complete[1].Split(',');
        Vector3 pos = new Vector3(float.Parse(vector[0]), float.Parse(vector[1]), float.Parse(vector[2]));
        foreach (var item in units)
            print("old school moving unit " + item + " to pos " + pos);
    }
}
