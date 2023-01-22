﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.Text.Json.Serialization;

namespace BibliothecaApi.Models;

public class User
{

    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("Name")]
    [JsonPropertyName("Name")]
    public string UserName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string NIN { get; set; } = null!;

    public string Address { get; set; } = null!;
}

