﻿// <auto-generated />
//
// To parse this JSON data, add NuGet 'Newtonsoft.Json' then do:
//
//    var grid = CellData.FromJson(jsonString);

    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using System.Numerics;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class CellData
    {
        public CellData()
        {
            room = Cell.RoomType.A;
            type = Cell.CellType.Floor;
        }

        [JsonProperty("X", Required = Required.Always)]
        public int X { get; set; }

        [JsonProperty("Y", Required = Required.Always)]
        public int Y { get; set; }

        [JsonProperty("isRobot", Required = Required.Always)]
        public bool isRobot { get; set; }

        [JsonProperty("dirtLevel", Required = Required.Always)]
        public int dirtLevel { get; set; }

        [JsonProperty("type", Required = Required.Always)]
        public Cell.CellType type { get; set; }

        [JsonProperty("room", Required = Required.Always)]
        public Cell.RoomType room { get; set; }
    }

    public partial class CellData
    {
        public static CellData[] FromJson(string json) => JsonConvert.DeserializeObject<CellData[]>(json, Converter.Settings);
    }


    public static class SerializeCells
    {
        public static string ToJson(this CellData[] self) => JsonConvert.SerializeObject(self, Converter.Settings);
    }

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }