using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public enum PacketIDs : short { Move_Command, Attack_Command, Select_Command, Old_School_Command, lalala, Count }

public class ConfigPackets {

    public Dictionary<PacketIDs, Action<PacketBase>> packetActions = new Dictionary<PacketIDs, Action<PacketBase>>();

    static void print(object p) { Debug.Log(p.ToString()); }

    public static Action<PacketBase> Move_Command = x => MoveCommand(x.stringInfo, x.vectorInfo[0]);
    public static Action<PacketBase> Attack_Command = x => AttackCommand(x.stringInfo, (int)x.floatInfo[0]);
    public static Action<PacketBase> Select_Command = x => SelectCommand(x.stringInfo);
    public static Action<PacketBase> Old_School_Command = x => Move(x.stringInfo[0]);

    public void Config_PacketActions()
    {
        packetActions.Add(PacketIDs.Move_Command, Move_Command);
        packetActions.Add(PacketIDs.Attack_Command, Attack_Command);
        packetActions.Add(PacketIDs.Select_Command, Select_Command);
        packetActions.Add(PacketIDs.Old_School_Command, Old_School_Command);
    }

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
